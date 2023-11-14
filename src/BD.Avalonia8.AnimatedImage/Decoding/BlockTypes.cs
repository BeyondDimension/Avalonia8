namespace BD.Avalonia8.AnimatedImage.Decoding;

#pragma warning disable SA1600 // Elements should be documented

internal enum BlockTypes : byte
{
    Empty = 0,
    Extension = 0x21,
    ImageDescriptor = 0x2C,
    Trailer = 0x3B,
}