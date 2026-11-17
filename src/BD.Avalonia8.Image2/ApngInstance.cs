using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Buffers;

namespace BD.Avalonia8.Image2;

public sealed class ApngInstance : IImageInstance, IDisposable
{
    public Stream? Stream { get; private set; }

    public IterationCount IterationCount { get; set; }

    public bool AutoStart { get; private set; } = true;

    public PixelSize ApngPixelSize { get; private set; }

    public bool IsSimplePNG { get; private set; }

    /// <inheritdoc/>
    public bool IsDisposed => disposedValue;

    /// <inheritdoc/>
    public double Height => ApngPixelSize.Height;

    /// <inheritdoc/>
    public double Width => ApngPixelSize.Width;

    public CancellationTokenSource CurrentCts { get; } = new();

    private APNG _apng = null!;
    private Frame[] _frames = [];
    private WriteableBitmap? _targetBitmap;
    private WriteableBitmap? _compositeBitmap; // 用于合成帧的位图
    private WriteableBitmap? _prevCompositeBitmap; // 保存上一帧合成后的结果
    private TimeSpan _totalTime;
    private TimeSpan[] _frameTimes = [];
    private DisposeOps _prevDisposeOp = DisposeOps.APNGDisposeOpNone;
    private AvaRect _prevFrameRect; // 上一帧的区域
    public AvaPoint _targetOffset;
    private int _currentFrameIndex = -1;
    private uint _iterationCount;
    public bool _hasNewFrame;
    private bool disposedValue;

    // Large animations must not retain every decoded frame. FIFO matches sequential
    // playback; evicted frames remain available in the compressed source for seeks.
    const long MaxDecodedFrameCacheBytes = 32 * 1024 * 1024;
    private WriteableBitmap?[] _frameCache = [];
    private readonly Queue<int> _cachedFrameOrder = new();
    private long _decodedFrameCacheBytes;

    // 是否正在前台显示（性能优化用）
    private bool _isVisible = true;

    public ApngInstance(Stream stream, bool leaveOpen = false)
    {
        try
        {
            Stream = leaveOpen || stream is not Microsoft.IO.RecyclableMemoryStream
                ? stream.SafeCopyToRecyclableMemoryStream("ApngInstance.Ctor", !leaveOpen)
                : stream;

            if (!Stream.CanSeek)
                throw new InvalidDataException("The provided stream is not seekable.");
            if (!Stream.CanRead)
                throw new InvalidOperationException("Can't read the stream provided.");

            Stream.Seek(0, SeekOrigin.Begin);
            try
            {
                _apng = new APNG(Stream);
                IsSimplePNG = _apng.IsSimplePNG;
            }
            finally
            {
                Stream.Position = 0;
            }

            if (IsSimplePNG)
            {
                _targetBitmap = WriteableBitmap.Decode(Stream);
                return;
            }

            _frames = _apng.Frames;
            if (_frames.Length != 0)
            {
                _frameCache = new WriteableBitmap?[_frames.Length];
                var firstFrame = _frames[0];
                var iHDRChunk = firstFrame.IHDRChunk.ThrowIsNull();
                ApngPixelSize = new PixelSize(iHDRChunk.Width, iHDRChunk.Height);

                // 初始化合成帧和目标帧
                _compositeBitmap = new WriteableBitmap(
                    new PixelSize(ApngPixelSize.Width, ApngPixelSize.Height),
                    new AvaVector(96, 96), AvaPixelFormat.Bgra8888, AlphaFormat.Premul);
                _prevCompositeBitmap = new WriteableBitmap(
                    new PixelSize(ApngPixelSize.Width, ApngPixelSize.Height),
                    new AvaVector(96, 96), AvaPixelFormat.Bgra8888, AlphaFormat.Premul);

                // 用透明色填充初始帧
                using (var context = _compositeBitmap.Lock())
                {
                    unsafe
                    {
                        var pixelData = (byte*)context.Address;
                        var totalBytes = ApngPixelSize.Width * ApngPixelSize.Height * 4;
                        for (int i = 0; i < totalBytes; i += 4)
                        {
                            pixelData[i] = 0;
                            pixelData[i + 1] = 0;
                            pixelData[i + 2] = 0;
                            pixelData[i + 3] = 0;
                        }
                    }
                }
                _targetBitmap = _compositeBitmap;
            }

            _frameTimes = new TimeSpan[_frames.Length];
            for (var i = 0; i < _frames.Length; i++)
            {
                _totalTime = _totalTime.Add(_frames[i].FrameDelay);
                _frameTimes[i] = _totalTime;
            }
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public WriteableBitmap? GetBitmap() => _targetBitmap;

    public void AutoStartChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool newVal)
            AutoStart = newVal;
    }

    /// <inheritdoc/>
    public AvaSize GetSize(double scaling) => ApngPixelSize.ToSize(scaling);

    /// <inheritdoc/>
    public AvaBitmap? ProcessFrameTime(TimeSpan stopwatchElapsed)
    {
        if (!IterationCount.IsInfinite && _iterationCount > IterationCount.Value)
            return null;

        if (CurrentCts.IsCancellationRequested || _compositeBitmap is null || _frameTimes.Length == 0)
            return null;

        var elapsedTicks = stopwatchElapsed.Ticks;
        var timeModulus = TimeSpan.FromTicks(elapsedTicks % _totalTime.Ticks);
        // Find the first frame whose end is strictly after the current time, without
        // a captured predicate and two linear scans on every compositor tick.
        var low = 0;
        var high = _frameTimes.Length;
        while (low < high)
        {
            var middle = low + (high - low) / 2;
            if (timeModulus < _frameTimes[middle])
                high = middle;
            else
                low = middle + 1;
        }
        var currentFrame = low < _frameTimes.Length ? low : 0;

        if (_currentFrameIndex == currentFrame)
            return _compositeBitmap;

        // 帧跳跃处理：向前跳时补齐中间帧，回绕时重置并从头补齐
        if (_currentFrameIndex == -1)
        {
            ResetComposite();
            for (int i = 0; i <= currentFrame; i++)
                ProcessFrameIndex(i, true);
        }
        else if (currentFrame > _currentFrameIndex)
        {
            for (int i = _currentFrameIndex + 1; i <= currentFrame; i++)
                ProcessFrameIndex(i, true);
        }
        else if (currentFrame < _currentFrameIndex)
        {
            ResetComposite();
            for (int i = 0; i <= currentFrame; i++)
                ProcessFrameIndex(i, true);
        }

        _iterationCount = unchecked((uint)(elapsedTicks / _totalTime.Ticks));

        return ProcessFrameIndex(currentFrame, false);
    }

    private void ResetComposite()
    {
        if (_compositeBitmap != null)
        {
            using (var context = _compositeBitmap.Lock())
            {
                unsafe
                {
                    var pixelData = (byte*)context.Address;
                    var totalBytes = ApngPixelSize.Width * ApngPixelSize.Height * 4;
                    for (int i = 0; i < totalBytes; i += 4)
                    {
                        pixelData[i] = 0; // B
                        pixelData[i + 1] = 0; // G
                        pixelData[i + 2] = 0; // R
                        pixelData[i + 3] = 0; // A (透明)
                    }
                }
            }
        }

        _prevDisposeOp = DisposeOps.APNGDisposeOpNone;
        _prevFrameRect = new AvaRect();
    }

    internal AvaBitmap ProcessFrameIndex(int frameIndex, bool silent = false)
    {
        try
        {
            // 如果控件不可见且不是静默模式，可以跳过复杂处理，直接返回上一帧
            if (!_isVisible && !silent && _compositeBitmap != null)
            {
                _currentFrameIndex = frameIndex;
                return _compositeBitmap;
            }

            var currentFrame = _frames[frameIndex];
            var fcTLChunk = currentFrame.fcTLChunk.ThrowIsNull();
            var blendOp = fcTLChunk.BlendOp;
            var disposeOp = fcTLChunk.DisposeOp;

            _targetOffset = new AvaPoint(fcTLChunk.XOffset, fcTLChunk.YOffset);
            var frameWidth = fcTLChunk.Width;
            var frameHeight = fcTLChunk.Height;

            var currentFrameRect = new AvaRect(
                fcTLChunk.XOffset,
                fcTLChunk.YOffset,
                frameWidth,
                frameHeight);

            // 应用前一帧的处置操作
            ApplyDisposeOperation();

            var frameBitmap = _frameCache[frameIndex];
            var transientFrame = false;
            if (frameBitmap == null)
            {
                using var frameStream = currentFrame.GetStream();
                frameBitmap = WriteableBitmap.Decode(frameStream);
                var frameBytes = GetFrameBytes(frameBitmap);
                if (frameBytes <= MaxDecodedFrameCacheBytes)
                {
                    while (_decodedFrameCacheBytes + frameBytes > MaxDecodedFrameCacheBytes)
                    {
                        var evictedIndex = _cachedFrameOrder.Dequeue();
                        var evicted = _frameCache[evictedIndex]!;
                        _decodedFrameCacheBytes -= GetFrameBytes(evicted);
                        _frameCache[evictedIndex] = null;
                        evicted.Dispose();
                    }
                    _frameCache[frameIndex] = frameBitmap;
                    _cachedFrameOrder.Enqueue(frameIndex);
                    _decodedFrameCacheBytes += frameBytes;
                }
                else
                    transientFrame = true;
            }

            try
            {
                ComposeFrame(frameBitmap, currentFrameRect, blendOp);
            }
            finally
            {
                if (transientFrame)
                    frameBitmap.Dispose();
            }

            // 保存当前帧的处置操作和区域，用于下一帧
            _prevDisposeOp = disposeOp;
            _prevFrameRect = currentFrameRect;

            // 保存合成结果，用于处置操作
            SwapCompositeBitmaps();

            // 更新当前帧索引
            _currentFrameIndex = frameIndex;

            return _compositeBitmap!;
        }
        catch (Exception ex)
        {
            Avalonia.Logging.Logger.Sink?.Log(
                Avalonia.Logging.LogEventLevel.Error,
                "ApngInstance",
                this,
                $"处理APNG帧出错: {ex.Message}");

            // 如果处理出错，返回当前合成位图
            return _compositeBitmap!;
        }
    }

    /// <summary>
    /// 设置APNG实例的可见性状态，用于性能优化
    /// </summary>
    /// <param name="isVisible">是否可见</param>
    public void SetVisibility(bool isVisible)
    {
        _isVisible = isVisible;
    }


    private void ApplyDisposeOperation()
    {
        if (_compositeBitmap == null || _prevCompositeBitmap == null)
            return;

        // 应用前一帧的处置操作
        switch (_prevDisposeOp)
        {
            case DisposeOps.APNGDisposeOpNone:
                // 不需要做任何处理，保留前一帧的渲染结果
                break;

            case DisposeOps.APNGDisposeOpBackground:
                // 将前一帧区域清除为透明
                using (var context = _compositeBitmap.Lock())
                {
                    unsafe
                    {
                        var pixelData = (byte*)context.Address;
                        ClearRegion(pixelData, _prevFrameRect);
                    }
                }

                break;

            case DisposeOps.APNGDisposeOpPrevious:
                // 恢复到渲染前一帧之前的状态
                CopyBitmap(_prevCompositeBitmap, _compositeBitmap);
                break;
        }
    }

    private unsafe void ClearRegion(byte* pixelData, AvaRect region)
    {
        int startX = Math.Max(0, (int)region.X);
        int startY = Math.Max(0, (int)region.Y);
        int endX = Math.Min(ApngPixelSize.Width, (int)(region.X + region.Width));
        int endY = Math.Min(ApngPixelSize.Height, (int)(region.Y + region.Height));

        int stride = ApngPixelSize.Width * 4;

        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                int offset = y * stride + x * 4;
                pixelData[offset] = 0; // B
                pixelData[offset + 1] = 0; // G
                pixelData[offset + 2] = 0; // R
                pixelData[offset + 3] = 0; // A (透明)
            }
        }
    }

    private void ComposeFrame(WriteableBitmap frameBitmap, AvaRect frameRect, BlendOps blendOp)
    {
        if (_compositeBitmap == null)
            return;

        using (var frameContext = frameBitmap.Lock())
        using (var compositeContext = _compositeBitmap.Lock())
        {
            unsafe
            {
                var framePixels = (byte*)frameContext.Address;
                var compositePixels = (byte*)compositeContext.Address;

                int startX = Math.Max(0, (int)frameRect.X);
                int startY = Math.Max(0, (int)frameRect.Y);
                int endX = Math.Min(ApngPixelSize.Width, (int)(frameRect.X + frameRect.Width));
                int endY = Math.Min(ApngPixelSize.Height, (int)(frameRect.Y + frameRect.Height));

                int frameStride = frameBitmap.PixelSize.Width * 4;
                int compositeStride = ApngPixelSize.Width * 4;

                for (int y = startY; y < endY; y++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        int frameX = x - startX;
                        int frameY = y - startY;

                        // 确保不会越界
                        if (frameX < 0 || frameX >= frameBitmap.PixelSize.Width ||
                            frameY < 0 || frameY >= frameBitmap.PixelSize.Height)
                            continue;

                        int frameOffset = frameY * frameStride + frameX * 4;
                        int compositeOffset = y * compositeStride + x * 4;

                        byte frameB = framePixels[frameOffset];
                        byte frameG = framePixels[frameOffset + 1];
                        byte frameR = framePixels[frameOffset + 2];
                        byte frameA = framePixels[frameOffset + 3];

                        byte compositeB = compositePixels[compositeOffset];
                        byte compositeG = compositePixels[compositeOffset + 1];
                        byte compositeR = compositePixels[compositeOffset + 2];
                        byte compositeA = compositePixels[compositeOffset + 3];

                        // 执行混合操作，优化版本
                        if (blendOp == BlendOps.APNGBlendOpSource || frameA == 255)
                        {
                            // 直接覆盖，批量写入4个字节以提高性能
                            *(uint*)(compositePixels + compositeOffset) = *(uint*)(framePixels + frameOffset);
                        }
                        else if (frameA > 0)
                        {
                            // 如果背景完全透明，可以直接使用前景
                            if (compositeA == 0)
                            {
                                *(uint*)(compositePixels + compositeOffset) = *(uint*)(framePixels + frameOffset);
                            }
                            else
                            {
                                // 优化的混合像素算法，使用整数运算代替浮点运算
                                int alpha = frameA;
                                int inverseAlpha = 255 - alpha;
                                int outAlpha = alpha + (compositeA * inverseAlpha) / 255;

                                if (outAlpha > 0)
                                {
                                    // 使用整数算法，避免浮点运算
                                    int alphaRatio = (alpha * 255) / outAlpha;
                                    int inverseAlphaRatio = 255 - alphaRatio;

                                    compositePixels[compositeOffset] =
                                        (byte)((frameB * alphaRatio + compositeB * inverseAlphaRatio) / 255);
                                    compositePixels[compositeOffset + 1] =
                                        (byte)((frameG * alphaRatio + compositeG * inverseAlphaRatio) / 255);
                                    compositePixels[compositeOffset + 2] =
                                        (byte)((frameR * alphaRatio + compositeR * inverseAlphaRatio) / 255);
                                    compositePixels[compositeOffset + 3] = (byte)outAlpha;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void SwapCompositeBitmaps()
    {
        if (_compositeBitmap == null || _prevCompositeBitmap == null)
            return;

        // 如果下一帧需要恢复到渲染前状态，复制当前合成帧到备份
        if (_prevDisposeOp == DisposeOps.APNGDisposeOpPrevious)
        {
            CopyBitmap(_compositeBitmap, _prevCompositeBitmap);
        }
    }

    private void CopyBitmap(WriteableBitmap source, WriteableBitmap destination)
    {
        using (var sourceContext = source.Lock())
        using (var destContext = destination.Lock())
        {
            unsafe
            {
                var sourcePixels = (byte*)sourceContext.Address;
                var destPixels = (byte*)destContext.Address;

                int totalBytes = ApngPixelSize.Width * ApngPixelSize.Height * 4;
                for (int i = 0; i < totalBytes; i++)
                {
                    destPixels[i] = sourcePixels[i];
                }
            }
        }
    }

    static long GetFrameBytes(WriteableBitmap bitmap) =>
        (long)bitmap.PixelSize.Width * bitmap.PixelSize.Height * 4;

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (!ReferenceEquals(_targetBitmap, _compositeBitmap))
                    _targetBitmap?.Dispose();
                _compositeBitmap?.Dispose();
                _prevCompositeBitmap?.Dispose();
                foreach (var bitmap in _frameCache)
                    bitmap?.Dispose();
                _frameCache = [];
                _cachedFrameOrder.Clear();
                _decodedFrameCacheBytes = 0;
                _frames = [];
                _frameTimes = [];
                _targetBitmap = null;
                _compositeBitmap = null;
                _prevCompositeBitmap = null;
                CurrentCts.Cancel();
                CurrentCts.Dispose();
                _apng?.Dispose();
                Stream?.Dispose();
                Stream = null;
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
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
}