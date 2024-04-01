namespace BD.Avalonia8.Image2.Decoding;

/// <summary>
/// A struct that represents a ARGB color and is aligned as
/// a BGRA bytefield in memory.
/// </summary>
/// <param name="r">Red</param>
/// <param name="g">Green</param>
/// <param name="b">Blue</param>
/// <param name="a">Alpha</param>
[StructLayout(LayoutKind.Explicit)]
public readonly struct GifColor(byte r, byte g, byte b, byte a = byte.MaxValue)
{
    [FieldOffset(3)]
    public readonly byte A = a;

    [FieldOffset(2)]
    public readonly byte R = r;

    [FieldOffset(1)]
    public readonly byte G = g;

    [FieldOffset(0)]
    public readonly byte B = b;
}