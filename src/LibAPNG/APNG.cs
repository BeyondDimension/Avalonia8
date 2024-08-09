namespace LibAPNG;

public class APNG : IDisposable
{
    Frame defaultImage = new();
    bool disposedValue;
    readonly List<Frame> frames = [];
    readonly Stream ms;

    //public APNG(string fileName)
    //    : this(File.ReadAllBytes(fileName))
    //{
    //}

    //public APNG(Stream stream)
    //    : this(LibAPNGHelper.StreamToBytes(stream))
    //{
    //}

    //public APNG(byte[] fileBytes)

    /// <summary>
    /// Initializes a new instance of the <see cref="APNG"/> class.
    /// </summary>
    /// <param name="stream"></param>
    public APNG(Stream stream)
    {
        ms = stream;

        // check file signature.
        if (!LibAPNGHelper.IsBytesEqual(ms, Frame.Signature))
            throw new LibAPNGException("File signature incorrect.");

        // Read IHDR chunk.
        IHDRChunk = new IHDRChunk(ms);
        if (IHDRChunk.ChunkType != "IHDR")
            throw new LibAPNGException("IHDR chunk must located before any other chunks.");

        // Now let's loop in chunks
        Chunk chunk;
        Frame? frame = null;
        var otherChunks = new List<OtherChunk>();
        bool isIDATAlreadyParsed = false;
        do
        {
            if (ms.Position == ms.Length)
                throw new LibAPNGException("IEND chunk expected.");

            chunk = new Chunk(ms);

            switch (chunk.ChunkType)
            {
                case "IHDR":
                    throw new LibAPNGException("Only single IHDR is allowed.");

                case "acTL":
                    if (IsSimplePNG)
                        throw new LibAPNGException("acTL chunk must located before any IDAT and fdAT");

                    acTLChunk = new acTLChunk(chunk);
                    break;

                case "IDAT":
                    // To be an APNG, acTL must located before any IDAT and fdAT.
                    if (acTLChunk == null)
                        IsSimplePNG = true;

                    // Only default image has IDAT.
                    defaultImage.IHDRChunk = IHDRChunk;
                    defaultImage.AddIDATChunk(new IDATChunk(chunk));
                    isIDATAlreadyParsed = true;
                    break;

                case "fcTL":
                    // Simple PNG should ignore this.
                    if (IsSimplePNG)
                        continue;

                    if (frame != null && frame.IDATChunks.Count == 0)
                        throw new LibAPNGException("One frame must have only one fcTL chunk.");

                    // IDAT already parsed means this fcTL is used by FRAME IMAGE.
                    if (isIDATAlreadyParsed)
                    {
                        // register current frame object and build a new frame object
                        // for next use
                        if (frame != null)
                            frames.Add(frame);
                        frame = new Frame
                        {
                            IHDRChunk = IHDRChunk,
                            fcTLChunk = new fcTLChunk(chunk),
                        };
                    }
                    // Otherwise this fcTL is used by the DEFAULT IMAGE.
                    else
                    {
                        defaultImage.fcTLChunk = new fcTLChunk(chunk);
                    }
                    break;
                case "fdAT":
                    // Simple PNG should ignore this.
                    if (IsSimplePNG)
                        continue;

                    // fdAT is only used by frame image.
                    if (frame == null || frame.fcTLChunk == null)
                        throw new LibAPNGException("fcTL chunk expected.");

                    frame.AddIDATChunk(new fdATChunk(chunk).ToIDATChunk());
                    break;

                case "IEND":
                    // register last frame object
                    if (frame != null)
                        frames.Add(frame);

                    if (DefaultImage.IDATChunks.Count != 0)
                        DefaultImage.IENDChunk = new IENDChunk(chunk);
                    foreach (Frame f in frames)
                    {
                        f.IENDChunk = new IENDChunk(chunk);
                    }
                    break;

                default:
                    otherChunks.Add(new OtherChunk(chunk));
                    break;
            }
        } while (chunk.ChunkType != "IEND");

        // We have one more thing to do:
        // If the default image if part of the animation,
        // we should insert it into frames list.
        if (defaultImage.fcTLChunk != null)
        {
            frames.Insert(0, defaultImage);
            DefaultImageIsAnimated = true;
        }

        // Now we should apply every chunk in otherChunks to every frame.
        frames.ForEach(f => otherChunks.ForEach(f.AddOtherChunk));
    }

    /// <summary>
    ///     Indicate whether the file is a simple PNG.
    /// </summary>
    public bool IsSimplePNG { get; private set; }

    /// <summary>
    ///     Indicate whether the default image is part of the animation
    /// </summary>
    public bool DefaultImageIsAnimated { get; private set; }

    /// <summary>
    ///     Gets the base image.
    ///     If IsSimplePNG = True, returns the only image;
    ///     if False, returns the default image
    /// </summary>
    public Frame DefaultImage
    {
        get { return defaultImage; }
    }

    /// <summary>
    ///     Gets the frame array.
    ///     If IsSimplePNG = True, returns empty
    /// </summary>
    public Frame[] Frames
    {
        get { return frames.ToArray(); }
    }

    /// <summary>
    ///     Gets the IHDR Chunk
    /// </summary>
    public IHDRChunk? IHDRChunk { get; private set; }

    /// <summary>
    ///     Gets the acTL Chunk
    /// </summary>
#pragma warning disable IDE1006 // 命名样式
    public acTLChunk? acTLChunk { get; private set; }
#pragma warning restore IDE1006 // 命名样式

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                defaultImage = null!;
                IHDRChunk = null;
                acTLChunk = null;

                frames.Clear();
                ms.Dispose();
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

[Serializable]
public sealed class LibAPNGException(string? message) : ApplicationException(message)
{
}