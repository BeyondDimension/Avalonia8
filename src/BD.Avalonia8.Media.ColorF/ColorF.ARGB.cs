namespace BD.Avalonia8.Media;

partial struct ColorF // ARGB
{
    /// <inheritdoc cref="FromArgb(ReadOnlySpan{char})"/>
    public static ColorF FromArgb(string? colorAsHex)
    {
        if (colorAsHex != null)
        {
            return FromArgb(colorAsHex.AsSpan());
        }
        return default;
    }

    /// <summary>
    /// 来自基于 string 的十六进制值，格式为“#AARRGGBB”、“#RRGGBB”、“#ARGB”或“#RGB”，其中每个字母分别对应 alpha、红色、绿色和蓝色通道的十六进制数字，如果是空字符串或 <see langword="null"/> 则返回默认值（黑色）
    /// </summary>
    /// <param name="colorAsHex"></param>
    /// <returns></returns>
    public static ColorF FromArgb(ReadOnlySpan<char> colorAsHex)
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

            if (colorAsHex.Length == 6)
            {
                //#RRGGBB
                red = ParseInt(colorAsHex[..2]);
                green = ParseInt(colorAsHex.Slice(2, 2));
                blue = ParseInt(colorAsHex.Slice(4, 2));
            }
            else if (colorAsHex.Length == 3)
            {
                //#RGB
                Span<char> temp = stackalloc char[2];
                temp[0] = temp[1] = colorAsHex[0];
                red = ParseInt(temp);

                temp[0] = temp[1] = colorAsHex[1];
                green = ParseInt(temp);

                temp[0] = temp[1] = colorAsHex[2];
                blue = ParseInt(temp);
            }
            else if (colorAsHex.Length == 4)
            {
                //#ARGB
                Span<char> temp = stackalloc char[2];
                temp[0] = temp[1] = colorAsHex[0];
                alpha = ParseInt(temp);

                temp[0] = temp[1] = colorAsHex[1];
                red = ParseInt(temp);

                temp[0] = temp[1] = colorAsHex[2];
                green = ParseInt(temp);

                temp[0] = temp[1] = colorAsHex[3];
                blue = ParseInt(temp);
            }
            else if (colorAsHex.Length == 8)
            {
                //#AARRGGBB
                alpha = ParseInt(colorAsHex[..2]);
                red = ParseInt(colorAsHex.Slice(2, 2));
                green = ParseInt(colorAsHex.Slice(4, 2));
                blue = ParseInt(colorAsHex.Slice(6, 2));
            }
        }

        return FromRgba(red, green, blue, alpha);
    }
}