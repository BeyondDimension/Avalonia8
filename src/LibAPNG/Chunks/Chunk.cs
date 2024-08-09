namespace LibAPNG.Chunks;

public class Chunk
{
    internal Chunk(string chunkType, byte[] chunkData, uint length, uint crc)
    {
        ChunkType = chunkType;
        ChunkData = chunkData;
        Length = length;
        Crc = crc;
    }

    internal unsafe Chunk(byte[] bytes)
    {
        fixed (byte* pointer = bytes)
        {
            using UnmanagedMemoryStream ms = new(pointer, bytes.LongLength);

            Length = LibAPNGHelper.ConvertEndian(ms.ReadUInt32());

            Span<byte> chunkType = stackalloc byte[4];
            ms.Read(chunkType);
            ChunkType = Encoding.ASCII.GetString(chunkType);

            ChunkData = GC.AllocateUninitializedArray<byte>(unchecked((int)Length));
            ms.Read(ChunkData);

            Crc = LibAPNGHelper.ConvertEndian(ms.ReadUInt32());

            if (ms.Position != ms.Length)
                throw new LibAPNGException("Chunk length not correct.");
            if (Length != ChunkData.Length)
                throw new LibAPNGException("Chunk data length not correct.");

            ParseData(ChunkData);
        }
    }

    internal Chunk(Stream ms)
    {
        Length = LibAPNGHelper.ConvertEndian(ms.ReadUInt32());

        Span<byte> chunkType = stackalloc byte[4];
        ms.Read(chunkType);
        ChunkType = Encoding.ASCII.GetString(chunkType);

        ChunkData = GC.AllocateUninitializedArray<byte>(unchecked((int)Length));
        ms.Read(ChunkData);

        Crc = LibAPNGHelper.ConvertEndian(ms.ReadUInt32());

        ParseData(ChunkData);
    }

    internal Chunk(Chunk chunk)
    {
        Length = chunk.Length;
        ChunkType = chunk.ChunkType;
        ChunkData = chunk.ChunkData.ThrowIsNull();
        Crc = chunk.Crc;

        ParseData(ChunkData);
    }

    public uint Length { get; protected set; }

    public string ChunkType { get; protected set; } = string.Empty;

    public byte[] ChunkData { get; protected set; }

    public uint Crc { get; protected set; }

    public void WriteRawData(Stream stream)
    {
        stream.WriteUInt32(LibAPNGHelper.ConvertEndian(Length));
        stream.Write(Encoding.ASCII.GetBytes(ChunkType));
        stream.Write(ChunkData.ThrowIsNull());
        stream.WriteUInt32(LibAPNGHelper.ConvertEndian(Crc));
    }

    /// <summary>
    ///     Get raw data of the chunk
    /// </summary>
    [Obsolete("use WriteRawData")]
    public byte[] RawData
    {
        get
        {
            var ms = new MemoryStream();
            WriteRawData(ms);
            return ms.ToArray();
        }
    }

    /// <summary>
    ///     Modify the ChunkData part.
    /// </summary>
    public void ModifyChunkData(int position, byte[] newData)
    {
        Array.Copy(newData, 0, ChunkData.ThrowIsNull(), position, newData.Length);

        #region Calculate by MemoryStream

        //using var msCrc = new MemoryStream();
        //msCrc.WriteUtf16StrToUtf8OrCustom(ChunkType, Encoding.ASCII);
        //msCrc.Write(ChunkData);

        //Crc = CrcHelper.Calculate(msCrc.ToEnumerable());

        #endregion

        #region Calculate by Concat

        var enumerable = Encoding.ASCII.GetBytes(ChunkType).Concat(ChunkData);
        Crc = CrcHelper.Calculate(enumerable);

        #endregion
    }

    /// <summary>
    ///     Modify the ChunkData part.
    /// </summary>
    public void ModifyChunkData(int position, uint newData)
    {
        ModifyChunkData(position, BitConverter.GetBytes(newData));
    }

    protected virtual void ParseData(ReadOnlySpan<byte> bytes)
    {
    }
}