using System.Buffers.Binary;

namespace BD.Avalonia8.UnitTest;

public sealed class BinaryTest
{
    [OneTimeSetUp]
    public void Setup()
    {
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
    }

    [Test]
    public void ConvertEndian()
    {
        int int32 = Random.Shared.Next();
        uint uint32 = unchecked((uint)Random.Shared.NextInt64(0, uint.MaxValue + 1L));
        short int16 = unchecked((short)Random.Shared.Next(short.MinValue, short.MaxValue + 1));
        ushort uint16 = unchecked((ushort)Random.Shared.Next(0, short.MaxValue + 1));

        Assert.That(H.ConvertEndian(int32) == BinaryPrimitives.ReverseEndianness(int32));
        Assert.That(H.ConvertEndian(uint32) == BinaryPrimitives.ReverseEndianness(uint32));
        Assert.That(H.ConvertEndian(int16) == BinaryPrimitives.ReverseEndianness(int16));
        Assert.That(H.ConvertEndian(uint16) == BinaryPrimitives.ReverseEndianness(uint16));
    }

    [Test]
    public void IsBytesEqual()
    {
        const string testString =
"""
namespace LibAPNG;

using CommunityToolkit.HighPerformance;
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
}
""";

        var str = string.Join(null, testString.Split(Environment.NewLine).Select(static x => x.Trim()));
        var u8 = Encoding.UTF8.GetBytes(str);

        using var s1 = new MemoryStream(u8);
        using var s2 = new MemoryStream(u8);

        var r = u8.Take(Random.Shared.Next(1, u8.Length)).ToArray();

        var r1 = H.IsBytesEqual(s1, r);
        Assert.That(r1);

        Span<byte> b2 = stackalloc byte[r.Length];
        s2.Read(b2);
        var r2 = H.IsBytesEqual(b2.ToArray(), r);
        Assert.That(r2);

        Assert.That(s1.Position == s2.Position);
    }

    static class H
    {
        /// <summary>
        ///     Convert big-endian to little-endian or reserve
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte[] ConvertEndian(byte[] i)
        {
            //if (i.Length % 2 != 0)
            //    throw new LibAPNGException("byte array length must multiply of 2");

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
    }
}
