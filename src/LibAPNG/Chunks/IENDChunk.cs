namespace LibAPNG.Chunks;

#pragma warning disable SA1600 // Elements should be documented

public class IENDChunk : Chunk
{
    public IENDChunk(byte[] bytes)
        : base(bytes)
    {
    }

    public IENDChunk(MemoryStream ms)
        : base(ms)
    {
    }

    public IENDChunk(Chunk chunk)
        : base(chunk)
    {
    }
}