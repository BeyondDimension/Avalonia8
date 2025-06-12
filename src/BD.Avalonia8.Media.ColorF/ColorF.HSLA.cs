using Microsoft.Maui.Graphics;

namespace BD.Avalonia8.Media;

partial struct ColorF // 色调-饱和度-发光度 (HSL)
{
    /// <summary>
    /// https://github.com/dotnet/maui/blob/9.0.71/src/Graphics/src/Graphics/Color.cs#L481
    /// </summary>
    /// <param name="hue"></param>
    /// <param name="saturation"></param>
    /// <param name="luminosity"></param>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    public static void ConvertToRgb(float hue, float saturation, float luminosity, out float r, out float g, out float b)
    {
        if (luminosity == 0)
        {
            r = g = b = 0;
            return;
        }

        if (saturation == 0)
        {
            r = g = b = luminosity;
            return;
        }
        float temp2 = luminosity <= 0.5f ? luminosity * (1.0f + saturation) : luminosity + saturation - luminosity * saturation;
        float temp1 = 2.0f * luminosity - temp2;

        var t3 = new[] { hue + 1.0f / 3.0f, hue, hue - 1.0f / 3.0f };
        var clr = new float[] { 0, 0, 0 };
        for (var i = 0; i < 3; i++)
        {
            if (t3[i] < 0)
                t3[i] += 1.0f;
            if (t3[i] > 1)
                t3[i] -= 1.0f;
            if (6.0 * t3[i] < 1.0)
                clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0f;
            else if (2.0 * t3[i] < 1.0)
                clr[i] = temp2;
            else if (3.0 * t3[i] < 2.0)
                clr[i] = temp1 + (temp2 - temp1) * (2.0f / 3.0f - t3[i]) * 6.0f;
            else
                clr[i] = temp1;
        }

        r = clr[0];
        g = clr[1];
        b = clr[2];
    }

    public static void ConvertToRgb(double hue, double saturation, double luminosity, out double r, out double g, out double b)
    {
        if (luminosity == 0)
        {
            r = g = b = 0;
            return;
        }

        if (saturation == 0)
        {
            r = g = b = luminosity;
            return;
        }
        double temp2 = luminosity <= 0.5 ? luminosity * (1.0 + saturation) : luminosity + saturation - luminosity * saturation;
        double temp1 = 2.0 * luminosity - temp2;

        var t3 = new[] { hue + 1.0 / 3.0, hue, hue - 1.0 / 3.0 };
        var clr = new double[] { 0, 0, 0 };
        for (var i = 0; i < 3; i++)
        {
            if (t3[i] < 0)
                t3[i] += 1.0;
            if (t3[i] > 1)
                t3[i] -= 1.0;
            if (6.0 * t3[i] < 1.0)
                clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0;
            else if (2.0 * t3[i] < 1.0)
                clr[i] = temp2;
            else if (3.0 * t3[i] < 2.0)
                clr[i] = temp1 + (temp2 - temp1) * (2.0 / 3.0 - t3[i]) * 6.0;
            else
                clr[i] = temp1;
        }

        r = clr[0];
        g = clr[1];
        b = clr[2];
    }

    /// <summary>
    /// 来自 <see langword="double"/> HSLA 值
    /// </summary>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="l"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static ColorF FromHsla(double h, double s, double l, double a = 1)
    {
        ConvertToRgb(h, s, l, out var red, out var green, out var blue);
        return new ColorF(red, green, blue, a);
    }

    /// <summary>
    /// 来自 <see langword="float"/> HSLA 值
    /// </summary>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="l"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static ColorF FromHsla(float h, float s, float l, float a = 1)
    {
        ConvertToRgb(h, s, l, out var red, out var green, out var blue);
        return new ColorF(red, green, blue, a);
    }

    /// <summary>
    /// 将 <see cref="ColorF"/> 转换为作为 out 参数传递的 HSL <see langword="float"/> 值
    /// </summary>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="l"></param>
    public void ToHsl(out float h, out float s, out float l)
    {
#if IOS || MACCATALYST || MACOS
        float r = (float)Red;
        float g = (float)Green;
        float b = (float)Blue;
#else
        float r = sRgbToScRgb(Red);
        float g = sRgbToScRgb(Green);
        float b = sRgbToScRgb(Blue);
#endif

        float v = MathF.Max(r, g);
        v = MathF.Max(v, b);

        float m = MathF.Min(r, g);
        m = MathF.Min(m, b);

        l = (m + v) / 2.0f;
        if (l <= 0.0)
        {
            h = s = l = 0;
            return;
        }
        float vm = v - m;
        s = vm;

        if (s > 0.0)
        {
            s /= l <= 0.5f ? v + m : 2.0f - v - m;
        }
        else
        {
            h = 0;
            s = 0;
            return;
        }

        float r2 = (v - r) / vm;
        float g2 = (v - g) / vm;
        float b2 = (v - b) / vm;

        if (r == v)
        {
            h = g == m ? 5.0f + b2 : 1.0f - g2;
        }
        else if (g == v)
        {
            h = b == m ? 1.0f + r2 : 3.0f - b2;
        }
        else
        {
            h = r == m ? 3.0f + g2 : 5.0f - r2;
        }
        h /= 6.0f;
    }

    /// <summary>
    /// 将 <see cref="ColorF"/> 转换为作为 out 参数传递的 HSL <see langword="double"/> 值
    /// </summary>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="l"></param>
    public void ToHsl(out double h, out double s, out double l)
    {
#if IOS || MACCATALYST || MACOS
        double r = Red.Value;
        double g = Green.Value;
        double b = Blue.Value;
#else
        double r = sRgbToScRgbD(Red);
        double g = sRgbToScRgbD(Green);
        double b = sRgbToScRgbD(Blue);
#endif

        double v = Math.Max(r, g);
        v = Math.Max(v, b);

        double m = Math.Min(r, g);
        m = Math.Min(m, b);

        l = (m + v) / 2.0;
        if (l <= 0.0)
        {
            h = s = l = 0;
            return;
        }
        double vm = v - m;
        s = vm;

        if (s > 0.0)
        {
            s /= l <= 0.5 ? v + m : 2.0 - v - m;
        }
        else
        {
            h = 0;
            s = 0;
            return;
        }

        double r2 = (v - r) / vm;
        double g2 = (v - g) / vm;
        double b2 = (v - b) / vm;

        if (r == v)
        {
            h = g == m ? 5.0 + b2 : 1.0 - g2;
        }
        else if (g == v)
        {
            h = b == m ? 1.0 + r2 : 3.0 - b2;
        }
        else
        {
            h = r == m ? 3.0 + g2 : 5.0 - r2;
        }
        h /= 6.0;
    }

    /// <summary>
    /// 返回表示颜色亮度通道的 <see langword="double"/>
    /// </summary>
    /// <returns></returns>
    public double GetLuminosity()
    {
#if IOS || MACCATALYST || MACOS
        double r = Red.Value;
        double g = Green.Value;
        double b = Blue.Value;
#else
        double r = sRgbToScRgbD(Red);
        double g = sRgbToScRgbD(Green);
        double b = sRgbToScRgbD(Blue);
#endif
        double v = Math.Max(r, g);
        v = Math.Max(v, b);
        double m = Math.Min(r, g);
        m = Math.Min(m, b);
        var l = (m + v) / 2.0;
        if (l <= 0.0)
            return 0;
        return l;
    }

    /// <summary>
    /// 通过将亮度值添加到提供的 delta 值来返回新的 <see cref="ColorF"/>
    /// </summary>
    /// <param name="delta"></param>
    /// <returns></returns>
    public ColorF AddLuminosity(float delta)
    {
        ToHsl(out float h, out float s, out float l);
        l += delta;
        l = l.Clamp(0, 1);
        return FromHsla(h, s, l, Alpha);
    }

    /// <summary>
    /// 通过将亮度值添加到提供的 delta 值来返回新的 <see cref="ColorF"/>
    /// </summary>
    /// <param name="delta"></param>
    /// <returns></returns>
    public ColorF AddLuminosity(double delta)
    {
        ToHsl(out double h, out double s, out double l);
        l += delta;
        l = l.Clamp(0, 1);
        return FromHsla(h, s, l, Alpha);
    }

    /// <summary>
    /// 返回 <see cref="ColorF"/>，将亮度值替换为提供的 <see langword="float"/> 值
    /// </summary>
    /// <param name="luminosity"></param>
    /// <returns></returns>
    public ColorF WithLuminosity(float luminosity)
    {
        ToHsl(out float h, out float s, out _);
        return FromHsla(h, s, luminosity, Alpha);
    }

    /// <summary>
    /// 返回 <see cref="ColorF"/>，将亮度值替换为提供的 <see langword="double"/> 值
    /// </summary>
    /// <param name="luminosity"></param>
    /// <returns></returns>
    public ColorF WithLuminosity(double luminosity)
    {
        ToHsl(out double h, out double s, out _);
        return FromHsla(h, s, luminosity, Alpha);
    }

    /// <summary>
    /// 返回表示颜色亮度通道的 <see langword="double"/>
    /// </summary>
    /// <returns></returns>
    public double GetSaturation()
    {
        ToHsl(out _, out double s, out _);
        return s;
    }

    /// <summary>
    /// 返回 <see cref="ColorF"/>，将饱和度值替换为提供的 <see langword="float"/> 值
    /// </summary>
    /// <param name="saturation"></param>
    /// <returns></returns>
    public ColorF WithSaturation(float saturation)
    {
        ToHsl(out float h, out _, out float l);
        return FromHsla(h, saturation, l, Alpha);
    }

    /// <summary>
    /// 返回 <see cref="ColorF"/>，将饱和度值替换为提供的 <see langword="double"/> 值
    /// </summary>
    /// <param name="saturation"></param>
    /// <returns></returns>
    public ColorF WithSaturation(double saturation)
    {
        ToHsl(out double h, out _, out double l);
        return FromHsla(h, saturation, l, Alpha);
    }

    /// <summary>
    /// 返回表示颜色的色调通道的 <see langword="double"/>
    /// </summary>
    /// <returns></returns>
    public double GetHue()
    {
        ToHsl(out double h, out _, out _);
        return h;
    }

    /// <summary>
    /// 返回 <see cref="ColorF"/>，将色调值替换为提供的 <see langword="float"/> 值
    /// </summary>
    /// <param name="hue"></param>
    /// <returns></returns>
    public ColorF WithHue(float hue)
    {
        ToHsl(out _, out float s, out float l);
        return FromHsla(hue, s, l, Alpha);
    }

    /// <summary>
    /// 返回 <see cref="ColorF"/>，将色调值替换为提供的 <see langword="double"/> 值
    /// </summary>
    /// <param name="hue"></param>
    /// <returns></returns>
    public ColorF WithHue(double hue)
    {
        ToHsl(out _, out double s, out double l);
        return FromHsla(hue, s, l, Alpha);
    }

    /// <summary>
    /// 返回互补性 <see cref="ColorF"/>
    /// </summary>
    /// <returns></returns>
    public ColorF GetComplementary()
    {
        ToHsl(out double h, out double s, out double l);

        // Add 180 (degrees) to get to the other side of the circle.
        h += 0.5;

        // Ensure still within the bounds of a circle.
        h %= 1.0;

        return ColorF.FromHsla(h, s, l);
    }
}