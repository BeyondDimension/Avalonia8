namespace LibAPNG.Chunks;

#pragma warning disable SA1600 // Elements should be documented

public class IDATChunk : Chunk
{
    public IDATChunk(byte[] bytes)
        : base(bytes)
    {
    }

    public IDATChunk(MemoryStream ms)
        : base(ms)
    {
    }

    public IDATChunk(Chunk chunk)
        : base(chunk)
    {
    }
}