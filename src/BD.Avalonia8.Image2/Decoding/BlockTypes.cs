namespace BD.Avalonia8.Image2.Decoding;

internal enum BlockTypes : byte
{
    Empty = 0,
    Extension = 0x21,
    ImageDescriptor = 0x2C,
    Trailer = 0x3B,
}