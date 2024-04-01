namespace LibAPNG.Chunks;

#pragma warning disable IDE1006 // 命名样式
public class acTLChunk : Chunk
#pragma warning restore IDE1006 // 命名样式
{
    public acTLChunk(byte[] bytes)
        : base(bytes)
    {
    }

    public acTLChunk(MemoryStream ms)
        : base(ms)
    {
    }

    public acTLChunk(Chunk chunk)
        : base(chunk)
    {
    }

    public uint NumFrames { get; private set; }

    public uint NumPlays { get; private set; }

    protected override void ParseData(MemoryStream ms)
    {
        NumFrames = LibAPNGHelper.ConvertEndian(ms.ReadUInt32());
        NumPlays = LibAPNGHelper.ConvertEndian(ms.ReadUInt32());
    }
}