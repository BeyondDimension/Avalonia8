namespace LibAPNG.Chunks;

#pragma warning disable SA1600 // Elements should be documented

public class Chunk
{
    internal Chunk() { }

    internal Chunk(byte[] bytes)
    {
        var ms = new MemoryStream(bytes);
        Length = LibAPNGHelper.ConvertEndian(ms.ReadUInt32());
        ChunkType = Encoding.ASCII.GetString(ms.ReadBytes(4));
        ChunkData = ms.ReadBytes((int)Length);
        Crc = LibAPNGHelper.ConvertEndian(ms.ReadUInt32());

        if (ms.Position != ms.Length)
            throw new LibAPNGException("Chunk length not correct.");
        if (Length != ChunkData.Length)
            throw new LibAPNGException("Chunk data length not correct.");

        ParseData(new MemoryStream(ChunkData));
    }

    internal Chunk(MemoryStream ms)
    {
        Length = LibAPNGHelper.ConvertEndian(ms.ReadUInt32());
        ChunkType = Encoding.ASCII.GetString(ms.ReadBytes(4));
        ChunkData = ms.ReadBytes((int)Length);
        Crc = LibAPNGHelper.ConvertEndian(ms.ReadUInt32());

        ParseData(new MemoryStream(ChunkData));
    }

    internal Chunk(Chunk chunk)
    {
        Length = chunk.Length;
        ChunkType = chunk.ChunkType;
        ChunkData = chunk.ChunkData.ThrowIsNull();
        Crc = chunk.Crc;

        ParseData(new MemoryStream(ChunkData));
    }

    public uint Length { get; set; }

    public string ChunkType { get; set; } = string.Empty;

    public byte[]? ChunkData { get; set; }

    public uint Crc { get; set; }

    /// <summary>
    ///     Get raw data of the chunk
    /// </summary>
    public byte[] RawData
    {
        get
        {
            var ms = new MemoryStream();
            ms.WriteUInt32(LibAPNGHelper.ConvertEndian(Length));
            ms.WriteBytes(Encoding.ASCII.GetBytes(ChunkType));
            ms.WriteBytes(ChunkData.ThrowIsNull());
            ms.WriteUInt32(LibAPNGHelper.ConvertEndian(Crc));

            return ms.ToArray();
        }
    }

    /// <summary>
    ///     Modify the ChunkData part.
    /// </summary>
    public void ModifyChunkData(int position, byte[] newData)
    {
        Array.Copy(newData, 0, ChunkData.ThrowIsNull(), position, newData.Length);

        using var msCrc = new MemoryStream();
        msCrc.WriteBytes(Encoding.ASCII.GetBytes(ChunkType));
        msCrc.WriteBytes(ChunkData);

        Crc = CrcHelper.Calculate(msCrc.ToArray());
    }

    /// <summary>
    ///     Modify the ChunkData part.
    /// </summary>
    public void ModifyChunkData(int position, uint newData)
    {
        ModifyChunkData(position, BitConverter.GetBytes(newData));
    }

    protected virtual void ParseData(MemoryStream ms)
    {
    }
}