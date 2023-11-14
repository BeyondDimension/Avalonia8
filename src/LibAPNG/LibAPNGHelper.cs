namespace LibAPNG;

#pragma warning disable SA1600 // Elements should be documented

internal static class LibAPNGHelper
{
    /// <summary>
    ///     Convert big-endian to little-endian or reserve
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static byte[] ConvertEndian(byte[] i)
    {
        if (i.Length % 2 != 0)
            throw new LibAPNGException("byte array length must multiply of 2");

        Array.Reverse(i);

        return i;
    }

    /// <summary>
    ///     Convert big-endian to little-endian or reserve
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int ConvertEndian(int i)
    {
        return BitConverter.ToInt32(ConvertEndian(BitConverter.GetBytes(i)), 0);
    }

    /// <summary>
    ///     Convert big-endian to little-endian or reserve
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint ConvertEndian(uint i)
    {
        return BitConverter.ToUInt32(ConvertEndian(BitConverter.GetBytes(i)), 0);
    }

    /// <summary>
    ///     Convert big-endian to little-endian or reserve
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static short ConvertEndian(short i)
    {
        return BitConverter.ToInt16(ConvertEndian(BitConverter.GetBytes(i)), 0);
    }

    /// <summary>
    ///     Convert big-endian to little-endian or reserve
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ushort ConvertEndian(ushort i)
    {
        return BitConverter.ToUInt16(ConvertEndian(BitConverter.GetBytes(i)), 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static byte[] StreamToBytes(Stream stream)
    {
        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);
        // 设置当前流的位置为流的开始
        stream.Seek(0, SeekOrigin.Begin);
        return bytes;
    }

    /// <summary>
    ///     Compare two byte array
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBytesEqual(byte[] byte1, byte[] byte2)
    {
        if (byte1.Length != byte2.Length)
            return false;

        for (int i = 0; i < byte1.Length; i++)
        {
            if (byte1[i] != byte2[i])
                return false;
        }
        return true;
    }
}