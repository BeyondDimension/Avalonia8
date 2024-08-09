// This source file's Lempel-Ziv-Welch algorithm is derived from Chromium's Android GifPlayer
// as seen here (https://github.com/chromium/chromium/blob/master/third_party/gif_player/src/jp/tomorrowkey/android/gifplayer)
// Licensed under the Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
// Copyright (C) 2015 The Gifplayer Authors. All Rights Reserved.

// The rest of the source file is licensed under MIT License.
// Copyright (C) 2018 Jumar A. Macato, All Rights Reserved.

using static BD.Avalonia8.Image2.StreamExtensions;

namespace BD.Avalonia8.Image2.Decoding;

public sealed class GifDecoder : IDisposable
{
    static readonly TimeSpan FrameDelayThreshold = TimeSpan.FromMilliseconds(10);
    static readonly TimeSpan FrameDelayDefault = TimeSpan.FromMilliseconds(100);
    static readonly GifColor TransparentColor = new(0, 0, 0, 0);
    static readonly int MaxTempBuf = 768;
    static readonly int MaxStackSize = 4096;
    static readonly int MaxBits = 4097;

    readonly Stream _fileStream;
    readonly CancellationToken _currentCtsToken;
    readonly bool _hasFrameBackups;

    int _gctSize;
    int _bgIndex;
    int _prevFrame = -1;
    int _backupFrame = -1;
    bool _gctUsed;

    GifRect _gifDimensions;

    readonly int _backBufferBytes;
    GifColor[] _bitmapBackBuffer;

    short[] _prefixBuf;
    byte[] _suffixBuf;
    byte[] _pixelStack;
    byte[] _indexBuf;
    byte[]? _backupFrameIndexBuf;
    volatile bool _hasNewFrame;
    bool disposedValue;

    public GifHeader Header { get; private set; }

    public readonly List<GifFrame> Frames = [];

    public PixelSize Size => new(Header.Dimensions.Width, Header.Dimensions.Height);

    /// <summary>
    /// Initializes a new instance of the <see cref="GifDecoder"/> class.
    /// </summary>
    /// <param name="fileStream"></param>
    /// <param name="currentCtsToken"></param>
    public GifDecoder(Stream fileStream, CancellationToken currentCtsToken)
    {
        _fileStream = fileStream;
        _currentCtsToken = currentCtsToken;

        ProcessHeaderData();
        ProcessFrameData();

        Header!.IterationCount = Header.Iterations switch
        {
            -1 => new GifRepeatBehavior { Count = 1 },
            0 => new GifRepeatBehavior { LoopForever = true },
            > 0 => new GifRepeatBehavior { Count = Header.Iterations },
            _ => Header.IterationCount,
        };

        var pixelCount = _gifDimensions.TotalPixels;

        _hasFrameBackups = Frames
            .Any(f => f.FrameDisposalMethod == FrameDisposal.Restore);

        _bitmapBackBuffer = new GifColor[pixelCount];
        _indexBuf = new byte[pixelCount];

        if (_hasFrameBackups)
            _backupFrameIndexBuf = new byte[pixelCount];

        _prefixBuf = new short[MaxStackSize];
        _suffixBuf = new byte[MaxStackSize];
        _pixelStack = new byte[MaxStackSize + 1];

        _backBufferBytes = pixelCount * Marshal.SizeOf<GifColor>();
    }

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                Frames.Clear();
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            _bitmapBackBuffer = null!;
            _prefixBuf = null!;
            _suffixBuf = null!;
            _pixelStack = null!;
            _indexBuf = null!;
            _backupFrameIndexBuf = null;

            disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int PixCoord(int x, int y) => x + (y * _gifDimensions.Width);

    static readonly (int Start, int Step)[] Pass =
    [
        (0, 8),
        (4, 8),
        (2, 4),
        (1, 2),
    ];

    void ClearImage()
    {
        Array.Fill(_bitmapBackBuffer, TransparentColor);

        _prevFrame = -1;
        _backupFrame = -1;
    }

    public void RenderFrame(int fIndex, WriteableBitmap writeableBitmap, bool forceClear = false)
    {
        if (_currentCtsToken.IsCancellationRequested)
            return;

        if (fIndex < 0 | fIndex >= Frames.Count)
            return;

        if (_prevFrame == fIndex)
            return;

        if (fIndex == 0 || forceClear || fIndex < _prevFrame)
            ClearImage();

        DisposePreviousFrame();

        _prevFrame++;

        // render intermediate frame
        for (int idx = _prevFrame; idx < fIndex; ++idx)
        {
            var prevFrame = Frames[idx];

            if (prevFrame.FrameDisposalMethod == FrameDisposal.Restore)
                continue;

            if (prevFrame.FrameDisposalMethod == FrameDisposal.Background)
            {
                ClearArea(prevFrame.Dimensions);
                continue;
            }

            RenderFrameAt(idx, writeableBitmap);
        }

        RenderFrameAt(fIndex, writeableBitmap);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void RenderFrameAt(int idx, WriteableBitmap writeableBitmap)
    {
        var tmpB = ArrayPool<byte>.Shared.Rent(MaxTempBuf);

        var curFrame = Frames[idx];
        DecompressFrameToIndexBuffer(curFrame, _indexBuf, tmpB);

        if (_hasFrameBackups & curFrame.ShouldBackup)
        {
            Buffer.BlockCopy(_indexBuf, 0, _backupFrameIndexBuf.ThrowIsNull(), 0, curFrame.Dimensions.TotalPixels);
            _backupFrame = idx;
        }

        DrawFrame(curFrame, _indexBuf);

        _prevFrame = idx;
        _hasNewFrame = true;

        using var lockedBitmap = writeableBitmap.Lock();
        WriteBackBufToFb(lockedBitmap.Address);

        ArrayPool<byte>.Shared.Return(tmpB);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void DrawFrame(GifFrame curFrame, Memory<byte> frameIndexSpan)
    {
        var activeColorTable =
            curFrame.IsLocalColorTableUsed ? curFrame.LocalColorTable : Header.GlobarColorTable;

        var cX = curFrame.Dimensions.X;
        var cY = curFrame.Dimensions.Y;
        var cH = curFrame.Dimensions.Height;
        var cW = curFrame.Dimensions.Width;
        var tC = curFrame.TransparentColorIndex;
        var hT = curFrame.HasTransparency;

        if (curFrame.IsInterlaced)
        {
            int curSrcRow = 0;
            for (var i = 0; i < 4; i++)
            {
                var (start, step) = Pass[i];
                var y = start;
                while (y < cH)
                {
                    DrawRow(curSrcRow++, y);
                    y += step;
                }
            }
        }
        else
        {
            for (var i = 0; i < cH; i++)
                DrawRow(i, i);
        }

        //for (var row = 0; row < cH; row++)
        void DrawRow(int srcRow, int destRow)
        {
            // Get the starting point of the current row on frame's index stream.
            var indexOffset = srcRow * cW;

            // Get the target backbuffer offset from the frames coords.
            var targetOffset = PixCoord(cX, destRow + cY);
            var len = _bitmapBackBuffer.Length;

            for (var i = 0; i < cW; i++)
            {
                var indexColor = frameIndexSpan.Span[indexOffset + i];

                if (activeColorTable == null || targetOffset >= len ||
                    indexColor > activeColorTable.Length) return;

                if (!(hT & indexColor == tC))
                    _bitmapBackBuffer[targetOffset] = activeColorTable[indexColor];

                targetOffset++;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void DisposePreviousFrame()
    {
        if (_prevFrame == -1)
            return;

        var prevFrame = Frames[_prevFrame];

        switch (prevFrame.FrameDisposalMethod)
        {
            case FrameDisposal.Background:
                ClearArea(prevFrame.Dimensions);
                break;
            case FrameDisposal.Restore:
                if (_hasFrameBackups && _backupFrame != -1)
                    DrawFrame(Frames[_backupFrame], _backupFrameIndexBuf);
                else
                    ClearArea(prevFrame.Dimensions);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ClearArea(GifRect area)
    {
        for (var y = 0; y < area.Height; y++)
        {
            var targetOffset = PixCoord(area.X, y + area.Y);
            for (var x = 0; x < area.Width; x++)
                _bitmapBackBuffer[targetOffset + x] = TransparentColor;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void DecompressFrameToIndexBuffer(GifFrame curFrame, Span<byte> indexSpan, byte[] tempBuf)
    {
        _fileStream.Position = curFrame.LzwStreamPosition;
        var totalPixels = curFrame.Dimensions.TotalPixels;

        // Initialize GIF data stream decoder.
        var dataSize = curFrame.LzwMinCodeSize;
        var clear = 1 << dataSize;
        var endOfInformation = clear + 1;
        var available = clear + 2;
        var oldCode = -1;
        var codeSize = dataSize + 1;
        var codeMask = (1 << codeSize) - 1;

        for (var code = 0; code < clear; code++)
        {
            _prefixBuf[code] = 0;
            _suffixBuf[code] = (byte)code;
        }

        // Decode GIF pixel stream.
        int bits, first, top, pixelIndex;
        var datum = bits = first = top = pixelIndex = 0;

        while (pixelIndex < totalPixels)
        {
            var blockSize = _fileStream.ReadBlock(tempBuf);

            if (blockSize == 0)
                break;

            var blockPos = 0;

            while (blockPos < blockSize)
            {
                datum += tempBuf[blockPos] << bits;
                blockPos++;

                bits += 8;

                while (bits >= codeSize)
                {
                    // Get the next code.
                    var code = datum & codeMask;
                    datum >>= codeSize;
                    bits -= codeSize;

                    // Interpret the code
                    if (code == clear)
                    {
                        // Reset decoder.
                        codeSize = dataSize + 1;
                        codeMask = (1 << codeSize) - 1;
                        available = clear + 2;
                        oldCode = -1;
                        continue;
                    }

                    // Check for explicit end-of-stream
                    if (code == endOfInformation)
                        return;

                    if (oldCode == -1)
                    {
                        indexSpan[pixelIndex++] = _suffixBuf[code];
                        oldCode = code;
                        first = code;
                        continue;
                    }

                    var inCode = code;
                    if (code >= available)
                    {
                        _pixelStack[top++] = (byte)first;
                        code = oldCode;

                        if (top == MaxBits)
                            throw new LzwDecompressionException();
                    }

                    while (code >= clear)
                    {
                        if (code >= MaxBits || code == _prefixBuf[code])
                            throw new LzwDecompressionException();

                        _pixelStack[top++] = _suffixBuf[code];
                        code = _prefixBuf[code];

                        if (top == MaxBits)
                            throw new LzwDecompressionException();
                    }

                    first = _suffixBuf[code];
                    _pixelStack[top++] = unchecked((byte)first);

                    // Add new code to the dictionary
                    if (available < MaxStackSize)
                    {
                        _prefixBuf[available] = unchecked((short)oldCode);
                        _suffixBuf[available] = unchecked((byte)first);
                        available++;

                        if ((available & codeMask) == 0 && available < MaxStackSize)
                        {
                            codeSize++;
                            codeMask += available;
                        }
                    }

                    oldCode = inCode;

                    // Drain the pixel stack.
                    do
                    {
                        indexSpan[pixelIndex++] = _pixelStack[--top];
                    } while (top > 0);
                }
            }
        }

        while (pixelIndex < totalPixels)
            indexSpan[pixelIndex++] = 0; // clear missing pixels
    }

    /// <summary>
    /// Directly copies the <see cref="GifColor"/> struct array to a bitmap IntPtr.
    /// </summary>
    void WriteBackBufToFb(IntPtr targetPointer)
    {
        if (_currentCtsToken.IsCancellationRequested)
            return;

        if (!(_hasNewFrame & _bitmapBackBuffer != null))
            return;

        unsafe
        {
            fixed (void* src = &_bitmapBackBuffer![0])
                Buffer.MemoryCopy(src, targetPointer.ToPointer(), (uint)_backBufferBytes,
                    (uint)_backBufferBytes);
            _hasNewFrame = false;
        }
    }

    /// <summary>
    /// Processes GIF Header.
    /// </summary>
    void ProcessHeaderData()
    {
        var str = _fileStream;
        var tmpB = ArrayPool<byte>.Shared.Rent(MaxTempBuf);
        var tempBuf = tmpB.AsSpan();

        str.Read(tmpB, 0, 6);

        var g87AMagic = "GIF87a"u8;

        if (!tempBuf[..3].SequenceEqual(g87AMagic[..3]))
            throw new InvalidGifStreamException("Not a GIF stream.");

        var g89AMagic = "GIF89a"u8;

        if (!(tempBuf[..6].SequenceEqual(g87AMagic) |
              tempBuf[..6].SequenceEqual(g89AMagic)))
            throw new InvalidGifStreamException("Unsupported GIF Version: " +
                                                Encoding.ASCII.GetString(tempBuf[..6].ToArray()));

        ProcessScreenDescriptor(tmpB);

        Header = new GifHeader
        {
            Dimensions = _gifDimensions,
            HasGlobalColorTable = _gctUsed,
            // GlobalColorTableCacheID = _globalColorTable,
            GlobarColorTable = ProcessColorTable(ref str, tmpB, _gctSize),
            GlobalColorTableSize = _gctSize,
            BackgroundColorIndex = _bgIndex,
            HeaderSize = _fileStream.Position,
        };

        ArrayPool<byte>.Shared.Return(tmpB);
    }

    /// <summary>
    /// Parses colors from file stream to target color table.
    /// </summary>
    static GifColor[] ProcessColorTable(ref Stream stream, byte[] rawBufSpan, int nColors)
    {
        var nBytes = 3 * nColors;
        var target = new GifColor[nColors];

        var n = stream.Read(rawBufSpan, 0, nBytes);

        if (n < nBytes)
            throw new InvalidOperationException("Wrong color table bytes.");

        int i = 0, j = 0;

        while (i < nColors)
        {
            var r = rawBufSpan[j++];
            var g = rawBufSpan[j++];
            var b = rawBufSpan[j++];
            target[i++] = new GifColor(r, g, b);
        }

        return target;
    }

    /// <summary>
    /// Parses screen and other GIF descriptors.
    /// </summary>
    void ProcessScreenDescriptor(byte[] tempBuf)
    {
        var width = _fileStream.ReadUShortS(tempBuf);
        var height = _fileStream.ReadUShortS(tempBuf);

        var packed = _fileStream.ReadByteS(tempBuf);

        _gctUsed = (packed & 0x80) != 0;
        _gctSize = 2 << (packed & 7);
        _bgIndex = _fileStream.ReadByteS(tempBuf);

        _gifDimensions = new GifRect(0, 0, width, height);
        _fileStream.Skip(1);
    }

    /// <summary>
    /// Parses all frame data.
    /// </summary>
    void ProcessFrameData()
    {
        _fileStream.Position = Header.HeaderSize;

        var tempBuf = ArrayPool<byte>.Shared.Rent(MaxTempBuf);

        var terminate = false;
        var curFrame = 0;

        Frames.Add(new());

        do
        {
            var blockType = (BlockTypes)_fileStream.ReadByteS(tempBuf);

            switch (blockType)
            {
                case BlockTypes.Empty:
                    break;

                case BlockTypes.Extension:
                    ProcessExtensions(ref curFrame, tempBuf);
                    break;

                case BlockTypes.ImageDescriptor:
                    ProcessImageDescriptor(ref curFrame, tempBuf);
                    _fileStream.SkipBlocks(tempBuf);
                    break;

                case BlockTypes.Trailer:
                    Frames.RemoveAt(Frames.Count - 1);
                    terminate = true;
                    break;

                default:
                    _fileStream.SkipBlocks(tempBuf);
                    break;
            }

            // Break the loop when the stream is not valid anymore.
            if (_fileStream.Position >= _fileStream.Length & terminate == false)
                throw new InvalidProgramException("Reach the end of the filestream without trailer block.");
        } while (!terminate);

        ArrayPool<byte>.Shared.Return(tempBuf);
    }

    /// <summary>
    /// Parses GIF Image Descriptor Block.
    /// </summary>
    void ProcessImageDescriptor(ref int curFrame, byte[] tempBuf)
    {
        var str = _fileStream;
        var currentFrame = Frames[curFrame];

        // Parse frame dimensions.
        var frameX = str.ReadUShortS(tempBuf);
        var frameY = str.ReadUShortS(tempBuf);
        var frameW = str.ReadUShortS(tempBuf);
        var frameH = str.ReadUShortS(tempBuf);

        frameW = unchecked((ushort)Math.Min(frameW, _gifDimensions.Width - frameX));
        frameH = unchecked((ushort)Math.Min(frameH, _gifDimensions.Height - frameY));

        currentFrame.Dimensions = new GifRect(frameX, frameY, frameW, frameH);

        // Unpack interlace and lct info.
        var packed = str.ReadByteS(tempBuf);
        currentFrame.IsInterlaced = (packed & 0x40) != 0;
        currentFrame.IsLocalColorTableUsed = (packed & 0x80) != 0;
        currentFrame.LocalColorTableSize = unchecked((int)Math.Pow(2, (packed & 0x07) + 1));

        if (currentFrame.IsLocalColorTableUsed)
            currentFrame.LocalColorTable =
                ProcessColorTable(ref str, tempBuf, currentFrame.LocalColorTableSize);

        currentFrame.LzwMinCodeSize = str.ReadByteS(tempBuf);
        currentFrame.LzwStreamPosition = str.Position;

        curFrame += 1;
        Frames.Add(new());
    }

    /// <summary>
    /// Parses GIF Extension Blocks.
    /// </summary>
    void ProcessExtensions(ref int curFrame, byte[] tempBuf)
    {
        var extType = (ExtensionType)_fileStream.ReadByteS(tempBuf);

        switch (extType)
        {
            case ExtensionType.GraphicsControl:

                _fileStream.ReadBlock(tempBuf);
                var currentFrame = Frames[curFrame];
                var packed = tempBuf[0];

                currentFrame.FrameDisposalMethod = (FrameDisposal)((packed & 0x1c) >> 2);

                if (currentFrame.FrameDisposalMethod != FrameDisposal.Restore
                    && currentFrame.FrameDisposalMethod != FrameDisposal.Background)
                    currentFrame.ShouldBackup = true;

                currentFrame.HasTransparency = (packed & 1) != 0;

                currentFrame.FrameDelay =
                    TimeSpan.FromMilliseconds(SpanToShort(tempBuf.AsSpan(1)) * 10);

                if (currentFrame.FrameDelay <= FrameDelayThreshold)
                    currentFrame.FrameDelay = FrameDelayDefault;

                currentFrame.TransparentColorIndex = tempBuf[3];
                break;

            case ExtensionType.Application:
                var blockLen = _fileStream.ReadBlock(tempBuf);
                tempBuf.AsSpan(0, blockLen);

                var netscapeMagic = "NETSCAPE2.0"u8;

                var blockHeader = tempBuf.AsSpan(0, netscapeMagic.Length);

                if (blockHeader.SequenceEqual(netscapeMagic))
                {
                    var count = 1;

                    while (count > 0)
                        count = _fileStream.ReadBlock(tempBuf);

                    Header.Iterations = SpanToShort(tempBuf.AsSpan(1));
                }
                else
                    _fileStream.SkipBlocks(tempBuf);

                break;

            default:
                _fileStream.SkipBlocks(tempBuf);
                break;
        }
    }
}