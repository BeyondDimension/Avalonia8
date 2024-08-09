namespace LibAPNG.Chunks;

public class OtherChunk : Chunk
{
    public OtherChunk(byte[] bytes)
        : base(bytes)
    {
    }

    public OtherChunk(Stream ms)
        : base(ms)
    {
    }

    public OtherChunk(Chunk chunk)
        : base(chunk)
    {
    }
}