namespace BD.Avalonia8.Media;

partial struct ColorF // RGB
{
    /// <summary>
    /// 将 <see cref="ColorF"/> 转换为作为 <see langword="out"/> 参数返回的 RGB <see langword="byte"/> 值
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    public void ToRgb(out byte r, out byte g, out byte b)
    {
#if IOS || MACCATALYST || MACOS
        r = ScRgbTosRgb(Red.Value);
        g = ScRgbTosRgb(Green.Value);
        b = ScRgbTosRgb(Blue.Value);
#else
        r = Red;
        g = Green;
        b = Blue;
#endif
    }

    /// <summary>
    /// 来自范围从 0 到 255 的 <see langword="byte"/> RGB 值
    /// </summary>
    /// <param name="red"></param>
    /// <param name="green"></param>
    /// <param name="blue"></param>
    /// <returns></returns>
    public static ColorF FromRgb(byte red, byte green, byte blue)
    {
#if IOS || MACCATALYST || MACOS
        return new ColorF(red / 255f, green / 255f, blue / 255f, 1f);
#else
        return new ColorF(red, green, blue, byte.MaxValue);
#endif
    }

    /// <summary>
    /// 来自范围从 0 到 255 的 <see langword="int"/> RGB 值
    /// </summary>
    /// <param name="red"></param>
    /// <param name="green"></param>
    /// <param name="blue"></param>
    /// <returns></returns>
    public static ColorF FromRgb(int red, int green, int blue)
    {
#if IOS || MACCATALYST || MACOS
        return new ColorF(red / 255f, green / 255f, blue / 255f, 1f);
#else
        return new ColorF(unchecked((byte)red), unchecked((byte)green), unchecked((byte)blue), byte.MaxValue);
#endif
    }

    /// <summary>
    /// 来自范围从 0 到 1 的 <see langword="float"/> RGB 值
    /// </summary>
    /// <param name="red"></param>
    /// <param name="green"></param>
    /// <param name="blue"></param>
    /// <returns></returns>
    public static ColorF FromRgb(float red, float green, float blue)
    {
        return new ColorF(red, green, blue);
    }

    /// <summary>
    /// 来自范围从 0 到 1 的 <see langword="double"/> RGB 值
    /// </summary>
    /// <param name="red"></param>
    /// <param name="green"></param>
    /// <param name="blue"></param>
    /// <returns></returns>
    public static ColorF FromRgb(double red, double green, double blue)
    {
#if IOS || MACCATALYST || MACOS
        return new ColorF((nfloat)red, (nfloat)green, (nfloat)blue);
#else
        return new ColorF(red, green, blue);
#endif
    }
}