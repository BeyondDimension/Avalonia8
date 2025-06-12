using Microsoft.Maui.Graphics;

namespace BD.Avalonia8.Media;

partial struct ColorF // 色调-饱和度-值 (HSV)
{
    /// <summary>
    /// 来自范围从 0 到 1 的 <see langword="float"/> HSVA 值
    /// </summary>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static ColorF FromHsva(float h, float s, float v, float a)
    {
        h = h.Clamp(0, 1);
        s = s.Clamp(0, 1);
        v = v.Clamp(0, 1);
        var range = (int)(MathF.Floor(h * 6)) % 6;
        var f = h * 6 - MathF.Floor(h * 6);
        var p = v * (1 - s);
        var q = v * (1 - f * s);
        var t = v * (1 - (1 - f) * s);

        return range switch
        {
            0 => FromRgba(v, t, p, a),
            1 => FromRgba(q, v, p, a),
            2 => FromRgba(p, v, t, a),
            3 => FromRgba(p, q, v, a),
            4 => FromRgba(t, p, v, a),
            _ => FromRgba(v, p, q, a),
        };
    }

    /// <summary>
    /// 来自范围从 0 到 1 的 <see langword="double"/> HSVA 值
    /// </summary>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static ColorF FromHsva(double h, double s, double v, double a)
    {
        h = h.Clamp(0, 1);
        s = s.Clamp(0, 1);
        v = v.Clamp(0, 1);
        var range = (int)(Math.Floor(h * 6)) % 6;
        var f = h * 6 - Math.Floor(h * 6);
        var p = v * (1 - s);
        var q = v * (1 - f * s);
        var t = v * (1 - (1 - f) * s);

        return range switch
        {
            0 => FromRgba(v, t, p, a),
            1 => FromRgba(q, v, p, a),
            2 => FromRgba(p, v, t, a),
            3 => FromRgba(p, q, v, a),
            4 => FromRgba(t, p, v, a),
            _ => FromRgba(v, p, q, a),
        };
    }

    /// <summary>
    /// 来自范围从 0 到 1 的 <see langword="float"/> HSV 值
    /// </summary>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static ColorF FromHsv(float h, float s, float v)
    {
        return FromHsva(h, s, v, 1f);
    }

    /// <summary>
    /// 来自范围从 0 到 1 的 <see langword="double"/> HSV 值
    /// </summary>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static ColorF FromHsv(double h, double s, double v)
    {
        return FromHsva(h, s, v, 1d);
    }

    /// <summary>
    /// 来自范围从 0 到 255 的 <see langword="int"/> HSVA 值
    /// </summary>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static ColorF FromHsva(int h, int s, int v, int a)
    {
        return FromHsva(h / 360, s / 100, v / 100, a / 100);
    }

    /// <summary>
    /// 来自范围从 0 到 255 的 <see langword="int"/> HSV 值
    /// </summary>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static ColorF FromHsv(int h, int s, int v)
    {
        return FromHsva(h / 360, s / 100, v / 100, 1);
    }
}