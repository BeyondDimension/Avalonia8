namespace BD.Avalonia8.Media;

partial struct ColorF // RGBA
{
    /// <summary>
    /// 将 <see cref="ColorF"/> 转换为作为 <see langword="out"/> 参数返回的 RGBA <see langword="byte"/> 值
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    public void ToRgba(out byte r, out byte g, out byte b, out byte a)
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

    /// <summary>
    /// 来自范围从 0 到 255 的 <see langword="byte"/> RGBA 值
    /// </summary>
    /// <param name="red"></param>
    /// <param name="green"></param>
    /// <param name="blue"></param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public static ColorF FromRgba(byte red, byte green, byte blue, byte alpha)
    {
#if IOS || MACCATALYST || MACOS
        return new ColorF(red / 255f, green / 255f, blue / 255f, alpha / 255f);
#else
        return new ColorF(red, green, blue, alpha);
#endif
    }

    /// <summary>
    /// 来自范围从 0 到 255 的 <see langword="int"/> RGBA 值
    /// </summary>
    /// <param name="red"></param>
    /// <param name="green"></param>
    /// <param name="blue"></param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public static ColorF FromRgba(int red, int green, int blue, int alpha)
    {
#if IOS || MACCATALYST || MACOS
        return new ColorF(red / 255f, green / 255f, blue / 255f, alpha / 255f);
#else
        return new ColorF(unchecked((byte)red), unchecked((byte)green), unchecked((byte)blue), unchecked((byte)alpha));
#endif
    }

    /// <summary>
    /// 来自范围从 0 到 1 的 <see langword="float"/> RGB 值
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static ColorF FromRgba(float r, float g, float b, float a)
    {
        return new ColorF(r, g, b, a);
    }

    /// <summary>
    /// 来自范围从 0 到 1 的 <see langword="double"/> RGB 值
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static ColorF FromRgba(double r, double g, double b, double a)
    {
#if IOS || MACCATALYST || MACOS
        return new ColorF((nfloat)r, (nfloat)g, (nfloat)b, (nfloat)a);
#else
        return new ColorF(r, g, b, a);
#endif
    }

    public static ColorF FromRgba(string? colorAsHex)
    {
        if (colorAsHex != null)
        {
            return FromRgba(colorAsHex.AsSpan());
        }
        return default;
    }

    public static ColorF FromRgba(ReadOnlySpan<char> colorAsHex)
    {
        int red = 0;
        int green = 0;
        int blue = 0;
        int alpha = 255;

        if (!colorAsHex.IsEmpty)
        {
            //Skip # if present
            if (colorAsHex[0] == '#')
                colorAsHex = colorAsHex[1..];

            if (colorAsHex.Length == 6 || colorAsHex.Length == 3)
            {
                //#RRGGBB or #RGB - since there is no A, use FromArgb

                return FromArgb(colorAsHex);
            }
            else if (colorAsHex.Length == 4)
            {
                //#RGBA
                Span<char> temp = stackalloc char[2];
                temp[0] = temp[1] = colorAsHex[0];
                red = ParseInt(temp);

                temp[0] = temp[1] = colorAsHex[1];
                green = ParseInt(temp);

                temp[0] = temp[1] = colorAsHex[2];
                blue = ParseInt(temp);

                temp[0] = temp[1] = colorAsHex[3];
                alpha = ParseInt(temp);
            }
            else if (colorAsHex.Length == 8)
            {
                //#RRGGBBAA
                red = ParseInt(colorAsHex[..2]);
                green = ParseInt(colorAsHex.Slice(2, 2));
                blue = ParseInt(colorAsHex.Slice(4, 2));
                alpha = ParseInt(colorAsHex.Slice(6, 2));
            }
        }

        return FromRgba(red, green, blue, alpha);
    }
}