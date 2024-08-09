namespace LibAPNG;

/// <summary>
///     Describe a single frame.
/// </summary>
public class Frame
{
    public static readonly byte[] Signature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    List<IDATChunk> idatChunks = [];
    List<OtherChunk> otherChunks = [];

    public TimeSpan FrameDelay
    {
        get
        {
            var delayDen = (double)(fcTLChunk.ThrowIsNull().DelayDen == 0 ? 100D : fcTLChunk.DelayDen);
            var keyTime = fcTLChunk.DelayNum == 0
                ? TimeSpan.FromMilliseconds(1)
                : TimeSpan.FromSeconds(fcTLChunk.DelayNum / delayDen);
            return keyTime;
        }
    }

    /// <summary>
    ///     Gets or Sets the acTL chunk
    /// </summary>
    public IHDRChunk? IHDRChunk { get; set; }

    /// <summary>
    ///     Gets or Sets the fcTL chunk
    /// </summary>
#pragma warning disable IDE1006 // 命名样式
    public fcTLChunk? fcTLChunk { get; set; }
#pragma warning restore IDE1006 // 命名样式

    /// <summary>
    ///     Gets or Sets the IEND chunk
    /// </summary>
    public IENDChunk? IENDChunk { get; set; }

    /// <summary>
    ///     Gets or Sets the other chunks
    /// </summary>
    public List<OtherChunk> OtherChunks
    {
        get { return otherChunks; }
        set { otherChunks = value; }
    }

    /// <summary>
    ///     Gets or Sets the IDAT chunks
    /// </summary>
    public List<IDATChunk> IDATChunks
    {
        get { return idatChunks; }
        set { idatChunks = value; }
    }

    /// <summary>
    ///     Add an Chunk to end end of existing list.
    /// </summary>
    public void AddOtherChunk(OtherChunk chunk)
    {
        otherChunks.Add(chunk);
    }

    /// <summary>
    ///     Add an IDAT Chunk to end end of existing list.
    /// </summary>
    public void AddIDATChunk(IDATChunk chunk)
    {
        idatChunks.Add(chunk);
    }

    /// <summary>
    ///     Gets the frame as PNG FileStream.
    /// </summary>
    public MemoryStream GetStream()
    {
        var ihdrChunk = new IHDRChunk(IHDRChunk.ThrowIsNull());
        if (fcTLChunk != null)
        {
            // Fix frame size with fcTL data.
            ihdrChunk.ModifyChunkData(0, LibAPNGHelper.ConvertEndian(fcTLChunk.Width));
            ihdrChunk.ModifyChunkData(4, LibAPNGHelper.ConvertEndian(fcTLChunk.Height));
        }

        // Write image data
        var ms = new MemoryStream();

        ms.Write(Signature);
        ihdrChunk.WriteRawData(ms);
        otherChunks.ForEach(o => o.WriteRawData(ms));
        idatChunks.ForEach(i => i.WriteRawData(ms));
        IENDChunk.ThrowIsNull().WriteRawData(ms);

        ms.Flush();
        ms.Position = 0;

#if DEBUG

        //var s = new MemoryStream();
        //ihdrChunk.WriteRawData(s);
        //var ihdrChunk_raw = Hashs.String.MD5(s.ToArray());
        //s.Dispose();

        //s = new();
        //otherChunks.ForEach(o => o.WriteRawData(s));
        //var otherChunks_raw = Hashs.String.MD5(s.ToArray());
        //s.Dispose();

        //s = new();
        //idatChunks.ForEach(o => o.WriteRawData(s));
        //var idatChunks_raw = Hashs.String.MD5(s.ToArray());
        //s.Dispose();

        //s = new();
        //IENDChunk.ThrowIsNull().WriteRawData(s);
        //var iENDChunk_raw = Hashs.String.MD5(s.ToArray());
        //s.Dispose();

#endif

        return ms;
    }
}