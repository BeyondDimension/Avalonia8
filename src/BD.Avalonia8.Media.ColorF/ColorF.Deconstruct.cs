namespace BD.Avalonia8.Media;

partial struct ColorF // 解构
{
    public readonly void Deconstruct(out double a, out double r, out double g, out double b)
    {
#if IOS || MACCATALYST || MACOS
        a = Alpha.Value;
        r = Red.Value;
        g = Green.Value;
        b = Blue.Value;
#else
        a = sRgbToScRgbD(Alpha);
        r = sRgbToScRgbD(Red);
        g = sRgbToScRgbD(Green);
        b = sRgbToScRgbD(Blue);
#endif
    }

    public readonly void Deconstruct(out byte a, out byte r, out byte g, out byte b)
    {
#if IOS || MACCATALYST || MACOS
        a = ScRgbTosRgb(Alpha.Value);
        r = ScRgbTosRgb(Red.Value);
        g = ScRgbTosRgb(Green.Value);
        b = ScRgbTosRgb(Blue.Value);
#else
        a = Alpha;
        r = Red;
        g = Green;
        b = Blue;
#endif
    }
}