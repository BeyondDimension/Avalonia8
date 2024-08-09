namespace LibAPNG.Chunks;

public class IDATChunk : Chunk
{
    internal IDATChunk(string chunkType, byte[] chunkData, uint length, uint crc)
        : base(chunkType, chunkData, length, crc)
    {
    }

    public IDATChunk(byte[] bytes)
        : base(bytes)
    {
    }

    public IDATChunk(Stream ms)
        : base(ms)
    {
    }

    public IDATChunk(Chunk chunk)
        : base(chunk)
    {
    }
}