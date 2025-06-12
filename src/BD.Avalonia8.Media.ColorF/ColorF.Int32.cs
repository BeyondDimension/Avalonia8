namespace BD.Avalonia8.Media;

partial struct ColorF // 使用单个 Int32/UInt32 表示 RGBA 颜色值
{
    /// <summary>
    /// 返回 <see cref="ColorF"/> 的 ARGB <see langword="int"/> 表示形式
    /// </summary>
    /// <returns></returns>
    public int ToInt()
    {
        ToRgba(out var r, out var g, out var b, out var a);
        int argb = a << 24 | r << 16 | g << 8 | b;
        return argb;
    }

    /// <summary>
    /// 返回 <see cref="ColorF"/> 的 ARGB <see langword="uint"/> 表示形式
    /// </summary>
    /// <returns></returns>
    public uint ToUint() => unchecked((uint)ToInt());

    /// <summary>
    /// 来自 <see langword="uint"/> 值，计算公式为 (B + 256 * (G + 256 * (R + 256 * A)))
    /// </summary>
    /// <param name="argb"></param>
    /// <returns></returns>
    public static ColorF FromUint(uint argb)
    {
        return FromRgba(
            (byte)((argb & 0x00ff0000) >> 0x10),
            (byte)((argb & 0x0000ff00) >> 0x8),
            (byte)(argb & 0x000000ff),
            (byte)((argb & 0xff000000) >> 0x18));
    }

    /// <summary>
    /// 来自 <see langword="int"/> 值，计算公式为 (B + 256 * (G + 256 * (R + 256 * A)))
    /// </summary>
    /// <param name="argb"></param>
    /// <returns></returns>
    public static ColorF FromInt(int argb)
    {
        return FromRgba(
            (byte)((argb & 0x00ff0000) >> 0x10),
            (byte)((argb & 0x0000ff00) >> 0x8),
            (byte)(argb & 0x000000ff),
            (byte)((argb & 0xff000000) >> 0x18));
    }
}