namespace LibAPNG;

using BinaryPrimitives = System.Buffers.Binary.BinaryPrimitives;

internal static class LibAPNGHelper
{
    /// <summary>
    ///     Convert big-endian to little-endian or reserve
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int ConvertEndian(int i)
    {
        //return BitConverter.ToInt32(ConvertEndian(BitConverter.GetBytes(i)), 0);
        return BinaryPrimitives.ReverseEndianness(i);
    }

    /// <summary>
    ///     Convert big-endian to little-endian or reserve
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint ConvertEndian(uint i)
    {
        //return BitConverter.ToUInt32(ConvertEndian(BitConverter.GetBytes(i)), 0);
        return BinaryPrimitives.ReverseEndianness(i);
    }

    /// <summary>
    ///     Convert big-endian to little-endian or reserve
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static short ConvertEndian(short i)
    {
        //return BitConverter.ToInt16(ConvertEndian(BitConverter.GetBytes(i)), 0);
        return BinaryPrimitives.ReverseEndianness(i);
    }

    /// <summary>
    ///     Convert big-endian to little-endian or reserve
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ushort ConvertEndian(ushort i)
    {
        //return BitConverter.ToUInt16(ConvertEndian(BitConverter.GetBytes(i)), 0);
        return BinaryPrimitives.ReverseEndianness(i);
    }

    /// <summary>
    ///     Compare two byte array
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBytesEqual(Stream l, byte[] r)
    {
        if (l.Length - l.Position < r.Length)
            return false;

        var pos = l.Position + r.Length;
        try
        {
            for (int i = 0; i < r.Length; i++)
            {
                if (l.ReadByte() != r[i])
                {
                    return false;
                }
            }
            return true;
        }
        finally
        {
            l.Position = pos;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteIDAT(Stream stream)
    {
        stream.WriteByte((byte)'I');
        stream.WriteByte((byte)'D');
        stream.WriteByte((byte)'A');
        stream.WriteByte((byte)'T');
    }
}