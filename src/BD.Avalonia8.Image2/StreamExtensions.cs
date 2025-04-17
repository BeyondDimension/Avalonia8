using Microsoft.IO;
using System.Runtime.CompilerServices;

namespace BD.Avalonia8.Image2;

internal static partial class StreamExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort SpanToShort(Span<byte> b) => unchecked((ushort)(b[0] | b[1] << 8));

    /// <summary>
    /// 将流安全地复制到 RecyclableMemoryStream
    /// </summary>
    /// <param name="stream">源流</param>
    /// <param name="tag">RecyclableMemoryStream 的标签，用于调试</param>
    /// <param name="disposeOriginal">是否释放原始流</param>
    /// <returns>RecyclableMemoryStream 实例</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RecyclableMemoryStream SafeCopyToRecyclableMemoryStream(this Stream stream, string tag,
        bool disposeOriginal = false)
    {
        if (Image2.MemoryStreamManager == null)
            throw new InvalidOperationException("MemoryStreamManager 未初始化");

        var result = Image2.MemoryStreamManager.GetStream(tag);

        // 保存当前位置以便稍后恢复
        var originalPosition = stream.Position;
        stream.Position = 0;

        // 复制到新的 RecyclableMemoryStream
        stream.CopyTo(result);

        // 重置位置
        result.Position = 0;
        stream.Position = originalPosition;

        // 如果需要，释放原始流
        if (disposeOriginal)
            stream.Dispose();

        return result;
    }

    /// <summary>
    /// 将流转换为 RecyclableMemoryStream
    /// </summary>
    /// <param name="stream">源流</param>
    /// <param name="tag">RecyclableMemoryStream 的标签，用于调试</param>
    /// <returns>RecyclableMemoryStream 实例</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RecyclableMemoryStream ToRecyclableMemoryStream(this Stream stream, string tag)
    {
        return SafeCopyToRecyclableMemoryStream(stream, tag, false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Skip(this Stream stream, long count)
    {
        stream.Position += count;
    }

    /// <summary>
    /// Read a Gif block from stream while advancing the position.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadBlock(this Stream stream, byte[] tempBuf)
    {
        stream.Read(tempBuf, 0, 1);

        var blockLength = (int)tempBuf[0];

        if (blockLength > 0)
            stream.Read(tempBuf, 0, blockLength);

        // Guard against infinite loop.
        if (stream.Position >= stream.Length)
            throw new InvalidGifStreamException("Reach the end of the filestream without trailer block.");

        return blockLength;
    }

    /// <summary>
    /// Skips GIF blocks until it encounters an empty block.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SkipBlocks(this Stream stream, byte[] tempBuf)
    {
        int blockLength;
        do
        {
            stream.Read(tempBuf, 0, 1);

            blockLength = tempBuf[0];
            stream.Position += blockLength;

            // Guard against infinite loop.
            if (stream.Position >= stream.Length)
                throw new InvalidGifStreamException("Reach the end of the filestream without trailer block.");
        } while (blockLength > 0);
    }

    /// <summary>
    /// Read a <see cref="ushort"/> from stream by providing a temporary buffer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadUShortS(this Stream stream, byte[] tempBuf)
    {
        stream.Read(tempBuf, 0, 2);
        return SpanToShort(tempBuf);
    }

    /// <summary>
    /// Read a <see cref="ushort"/> from stream by providing a temporary buffer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ReadByteS(this Stream stream, byte[] tempBuf)
    {
        stream.Read(tempBuf, 0, 1);
        var finalVal = tempBuf[0];
        return finalVal;
    }
}