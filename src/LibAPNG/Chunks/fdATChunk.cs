namespace LibAPNG.Chunks;

#pragma warning disable IDE1006 // 命名样式
internal class fdATChunk : Chunk
#pragma warning restore IDE1006 // 命名样式
{
    public fdATChunk(byte[] bytes)
        : base(bytes)
    {
    }

    public fdATChunk(MemoryStream ms)
        : base(ms)
    {
    }

    public fdATChunk(Chunk chunk)
        : base(chunk)
    {
    }

    public uint SequenceNumber { get; private set; }

    public byte[]? FrameData { get; private set; }

    protected override void ParseData(MemoryStream ms)
    {
        SequenceNumber = LibAPNGHelper.ConvertEndian(ms.ReadUInt32());
        FrameData = ms.ReadBytes((int)Length - 4);
    }

    public IDATChunk ToIDATChunk()
    {
        uint newCrc;
        using (var msCrc = new MemoryStream())
        {
            msCrc.WriteBytes([(byte)'I', (byte)'D', (byte)'A', (byte)'T']);
            msCrc.WriteBytes(FrameData.ThrowIsNull());

            newCrc = CrcHelper.Calculate(msCrc.ToArray());
        }

        using var ms = new MemoryStream();
        ms.WriteUInt32(LibAPNGHelper.ConvertEndian(Length - 4));
        ms.WriteBytes([(byte)'I', (byte)'D', (byte)'A', (byte)'T']);
        ms.WriteBytes(FrameData);
        ms.WriteUInt32(LibAPNGHelper.ConvertEndian(newCrc));
        ms.Position = 0;

        return new IDATChunk(ms);
    }
}