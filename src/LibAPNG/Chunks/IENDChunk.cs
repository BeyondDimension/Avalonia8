namespace LibAPNG.Chunks;

public class IENDChunk : Chunk
{
    public IENDChunk(byte[] bytes)
        : base(bytes)
    {
    }

    public IENDChunk(Stream ms)
        : base(ms)
    {
    }

    public IENDChunk(Chunk chunk)
        : base(chunk)
    {
    }
}