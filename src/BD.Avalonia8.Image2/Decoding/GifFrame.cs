namespace BD.Avalonia8.Image2.Decoding;

public record class GifFrame
{
    public bool HasTransparency;
    public bool IsInterlaced;
    public bool IsLocalColorTableUsed;
    public byte TransparentColorIndex;
    public int LzwMinCodeSize;
    public int LocalColorTableSize;
    public long LzwStreamPosition;
    public TimeSpan FrameDelay;
    public FrameDisposal FrameDisposalMethod;
    public bool ShouldBackup;
    public GifRect Dimensions;
    public GifColor[]? LocalColorTable;
}