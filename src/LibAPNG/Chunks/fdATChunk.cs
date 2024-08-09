namespace LibAPNG.Chunks;

#pragma warning disable IDE1006 // 命名样式
internal class fdATChunk : Chunk
#pragma warning restore IDE1006 // 命名样式
{
    public fdATChunk(byte[] bytes)
        : base(bytes)
    {
    }

    public fdATChunk(Stream ms)
        : base(ms)
    {
    }

    public fdATChunk(Chunk chunk)
        : base(chunk)
    {
    }

    public uint SequenceNumber { get; private set; }

    //public byte[]? FrameData { get; private set; }

    int startFrameData;

    public ReadOnlySpan<byte> GetFrameData()
    {
        var chunkData = ChunkData;
        return chunkData.AsSpan().Slice(startFrameData, unchecked((int)Length) - 4);
    }

    //protected override void ParseData(MemoryStream ms)
    //{
    //    SequenceNumber = LibAPNGHelper.ConvertEndian(ms.ReadUInt32());
    //    FrameData = ms.ReadBytes((int)Length - 4);
    //}

    protected override unsafe void ParseData(ReadOnlySpan<byte> bytes)
    {
        fixed (byte* pointer = bytes)
        {
            using UnmanagedMemoryStream ms = new(pointer, bytes.Length);
            SequenceNumber = LibAPNGHelper.ConvertEndian(ms.ReadUInt32());
            startFrameData = unchecked((int)ms.Position);
            //FrameData = GC.AllocateUninitializedArray<byte>(unchecked((int)Length) - 4);
            //ms.Read(FrameData);
        }
    }

    public IDATChunk ToIDATChunk()
    {
        var frameData = GetFrameData();
        var frameDataArray = frameData.ToArray();

        #region Calculate by MemoryStream

        //uint newCrc;
        //using (var msCrc = new MemoryStream())
        //{
        //    LibAPNGHelper.WriteIDAT(msCrc);
        //    msCrc.Write(frameData);

        //    msCrc.Position = 0;

        //    newCrc = CrcHelper.Calculate(msCrc.ToEnumerable());
        //}

        #endregion

        #region Calculate by Concat

        var enumerable = new byte[] { (byte)'I', (byte)'D', (byte)'A', (byte)'T' }.Concat(frameDataArray);
        var newCrc = CrcHelper.Calculate(enumerable);

        #endregion

        return new IDATChunk("IDAT", frameDataArray, Length - 4, newCrc);
    }
}