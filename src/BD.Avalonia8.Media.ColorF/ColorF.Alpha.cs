using Microsoft.Maui.Graphics;

namespace BD.Avalonia8.Media;

partial struct ColorF // Alpha Change functions
{
    /// <summary>
    /// 返回 <see cref="ColorF"/>，将 alpha 值替换为提供的 <see langword="double"/> 值
    /// </summary>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public ColorF WithAlpha(double alpha)
    {
#if IOS || MACCATALYST || MACOS
        if (Math.Abs(alpha - Alpha) < GeometryUtil.Epsilon)
            return this;

        return new ColorF(Red, Green, Blue, alpha);
#else
        if ((alpha == 1 && Alpha == byte.MaxValue) || (Math.Abs(alpha - sRgbToScRgbD(Alpha)) < GeometryUtil.Epsilon))
        {
            return this;
        }
        return new ColorF(Red, Green, Blue, ScRgbTosRgb(alpha));
#endif
    }

    /// <summary>
    /// 返回 <see cref="ColorF"/>，将 alpha 值替换为提供的 <see langword="byte"/> 值
    /// </summary>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public ColorF WithAlpha(byte alpha)
    {
#if IOS || MACCATALYST || MACOS
        if ((alpha == byte.MaxValue && Alpha == 1.0) || Math.Abs(sRgbToScRgbD(alpha) - Alpha) < GeometryUtil.Epsilon)
        {
            return this;
        }
        return new ColorF(Red, Green, Blue, sRgbToScRgbD(alpha));
#else
        if (alpha == Alpha)
            return this;

        return new ColorF(Red, Green, Blue, alpha);
#endif
    }

    /// <summary>
    /// 通过将 alpha 值乘以提供的 <see langword="double"/> 值来返回 <see cref="ColorF"/>
    /// </summary>
    /// <param name="multiplyBy"></param>
    /// <returns></returns>
    public ColorF MultiplyAlpha(double multiplyBy)
    {
#if IOS || MACCATALYST || MACOS
        return new ColorF(Red, Green, Blue, Alpha.Value * multiplyBy);
#else
        return new ColorF(Red, Green, Blue,
            ScRgbTosRgb(sRgbToScRgbD(Alpha) * multiplyBy));
#endif
    }
}