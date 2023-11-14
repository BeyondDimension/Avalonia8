namespace BD.Avalonia8.AnimatedImage.Decoding;

#pragma warning disable SA1600 // Elements should be documented

public record struct GifFrame
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