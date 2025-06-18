using System.Diagnostics;
using AvaColor = Avalonia.Media.Color;
using Colors = Avalonia.Media.Colors;

namespace BD.Avalonia8.Media;

partial struct ColorF // Parse
{
    public static ColorF Parse(string? value)
    {
        if (TryParse(value, out var c))
            return c;

        throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(ColorF)}");
    }

    public static bool TryParse(string? value, out ColorF color)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            color = default;
            return false;
        }
        return TryParse(value.AsSpan(), out color);
    }

    public static bool TryParse(ReadOnlySpan<char> value, out ColorF color)
    {
        value = value.Trim();
        if (!value.IsEmpty)
        {
            if (value[0] == '#')
            {
                try
                {
                    color = ColorF.FromArgb(value);
                    return true;
                }
                catch
                {
                    goto ReturnFalse;
                }
            }

            if (value.StartsWith("rgba".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                if (!TryParseFourColorRanges(value,
                    out ReadOnlySpan<char> quad0,
                    out ReadOnlySpan<char> quad1,
                    out ReadOnlySpan<char> quad2,
                    out ReadOnlySpan<char> quad3))
                {
                    goto ReturnFalse;
                }

                bool valid = TryParseColorValue(quad0, 255, acceptPercent: true, out double r);
                valid &= TryParseColorValue(quad1, 255, acceptPercent: true, out double g);
                valid &= TryParseColorValue(quad2, 255, acceptPercent: true, out double b);
                valid &= TryParseOpacity(quad3, out double a);

                if (!valid)
                    goto ReturnFalse;

                color = new ColorF(r, g, b, a);
                return true;
            }

            if (value.StartsWith("rgb".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                if (!TryParseThreeColorRanges(value,
                    out ReadOnlySpan<char> triplet0,
                    out ReadOnlySpan<char> triplet1,
                    out ReadOnlySpan<char> triplet2))
                {
                    goto ReturnFalse;
                }

                bool valid = TryParseColorValue(triplet0, 255, acceptPercent: true, out double r);
                valid &= TryParseColorValue(triplet1, 255, acceptPercent: true, out double g);
                valid &= TryParseColorValue(triplet2, 255, acceptPercent: true, out double b);

                if (!valid)
                    goto ReturnFalse;

                color = new ColorF(r, g, b);
                return true;
            }

            if (value.StartsWith("hsla".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                if (!TryParseFourColorRanges(value,
                    out ReadOnlySpan<char> quad0,
                    out ReadOnlySpan<char> quad1,
                    out ReadOnlySpan<char> quad2,
                    out ReadOnlySpan<char> quad3))
                {
                    goto ReturnFalse;
                }

                bool valid = TryParseColorValue(quad0, 360, acceptPercent: false, out double h);
                valid &= TryParseColorValue(quad1, 100, acceptPercent: true, out double s);
                valid &= TryParseColorValue(quad2, 100, acceptPercent: true, out double l);
                valid &= TryParseOpacity(quad3, out double a);

                if (!valid)
                    goto ReturnFalse;

                color = ColorF.FromHsla(h, s, l, a);
                return true;
            }

            if (value.StartsWith("hsl".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                if (!TryParseThreeColorRanges(value,
                    out ReadOnlySpan<char> triplet0,
                    out ReadOnlySpan<char> triplet1,
                    out ReadOnlySpan<char> triplet2))
                {
                    goto ReturnFalse;
                }

                bool valid = TryParseColorValue(triplet0, 360, acceptPercent: false, out double h);
                valid &= TryParseColorValue(triplet1, 100, acceptPercent: true, out double s);
                valid &= TryParseColorValue(triplet2, 100, acceptPercent: true, out double l);

                if (!valid)
                    goto ReturnFalse;

                color = ColorF.FromHsla(h, s, l);
                return true;
            }

            if (value.StartsWith("hsva".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                if (!TryParseFourColorRanges(value,
                    out ReadOnlySpan<char> quad0,
                    out ReadOnlySpan<char> quad1,
                    out ReadOnlySpan<char> quad2,
                    out ReadOnlySpan<char> quad3))
                {
                    goto ReturnFalse;
                }

                bool valid = TryParseColorValue(quad0, 360, acceptPercent: false, out double h);
                valid &= TryParseColorValue(quad1, 100, acceptPercent: true, out double s);
                valid &= TryParseColorValue(quad2, 100, acceptPercent: true, out double v);
                valid &= TryParseOpacity(quad3, out double a);

                if (!valid)
                    goto ReturnFalse;

                color = ColorF.FromHsva(h, s, v, a);
                return true;
            }

            if (value.StartsWith("hsv".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                if (!TryParseThreeColorRanges(value,
                    out ReadOnlySpan<char> triplet0,
                    out ReadOnlySpan<char> triplet1,
                    out ReadOnlySpan<char> triplet2))
                {
                    goto ReturnFalse;
                }

                bool valid = TryParseColorValue(triplet0, 360, acceptPercent: false, out double h);
                valid &= TryParseColorValue(triplet1, 100, acceptPercent: true, out double s);
                valid &= TryParseColorValue(triplet2, 100, acceptPercent: true, out double v);

                if (!valid)
                    goto ReturnFalse;

                color = ColorF.FromHsv(h, s, v);
                return true;
            }

            var namedColor = GetNamedColor(value);
            if (namedColor.HasValue)
            {
                color = new(namedColor.Value.R, namedColor.Value.G, namedColor.Value.B, namedColor.Value.A);
                return true;
            }
        }

    ReturnFalse:
        color = default;
        return false;
    }

    public static AvaColor? GetNamedColor(ReadOnlySpan<char> value)
    {
        if (value.Length > 128 || value.Length > Converter.GetNamedColorKeyMaxLength())
        {
            return null;
        }

        // the longest built-in Color's name is much lower than this check, so we should not allocate here in a typical usage
        //Span<char> loweredValue = value.Length <= 128 ? stackalloc char[value.Length] : new char[value.Length];
        Span<char> loweredValue = stackalloc char[value.Length];
        for (int i = 0; i < value.Length; i++)
        {
            loweredValue[i] = char.ToLowerInvariant(value[i]);
        }

        AvaColor? avaColor = loweredValue switch
        {
            // https://github.com/dotnet/maui/blob/9.0.71/src/Graphics/src/Graphics/Colors.cs
            "default" => new AvaColor(1, 0, 0, 0), // #ff000000
            "aliceblue" => Colors.AliceBlue,
            "antiquewhite" => Colors.AntiqueWhite,
            "aqua" => Colors.Aqua,
            "aquamarine" => Colors.Aquamarine,
            "azure" => Colors.Azure,
            "beige" => Colors.Beige,
            "bisque" => Colors.Bisque,
            "black" => Colors.Black,
            "blanchedalmond" => Colors.BlanchedAlmond,
            "blue" => Colors.Blue,
            "blueviolet" => Colors.BlueViolet,
            "brown" => Colors.Brown,
            "burlywood" => Colors.BurlyWood,
            "cadetblue" => Colors.CadetBlue,
            "chartreuse" => Colors.Chartreuse,
            "chocolate" => Colors.Chocolate,
            "coral" => Colors.Coral,
            "cornflowerblue" => Colors.CornflowerBlue,
            "cornsilk" => Colors.Cornsilk,
            "crimson" => Colors.Crimson,
            "cyan" => Colors.Cyan,
            "darkblue" => Colors.DarkBlue,
            "darkcyan" => Colors.DarkCyan,
            "darkgoldenrod" => Colors.DarkGoldenrod,
            "darkgray" => Colors.DarkGray,
            "darkgreen" => Colors.DarkGreen,
            "darkgrey" => Colors.DarkGray,
            "darkkhaki" => Colors.DarkKhaki,
            "darkmagenta" => Colors.DarkMagenta,
            "darkolivegreen" => Colors.DarkOliveGreen,
            "darkorange" => Colors.DarkOrange,
            "darkorchid" => Colors.DarkOrchid,
            "darkred" => Colors.DarkRed,
            "darksalmon" => Colors.DarkSalmon,
            "darkseagreen" => Colors.DarkSeaGreen,
            "darkslateblue" => Colors.DarkSlateBlue,
            "darkslategray" => Colors.DarkSlateGray,
            "darkslategrey" => Colors.DarkSlateGray,
            "darkturquoise" => Colors.DarkTurquoise,
            "darkviolet" => Colors.DarkViolet,
            "deeppink" => Colors.DeepPink,
            "deepskyblue" => Colors.DeepSkyBlue,
            "dimgray" => Colors.DimGray,
            "dimgrey" => Colors.DimGray,
            "dodgerblue" => Colors.DodgerBlue,
            "firebrick" => Colors.Firebrick,
            "floralwhite" => Colors.FloralWhite,
            "forestgreen" => Colors.ForestGreen,
            "fuchsia" => Colors.Fuchsia,
            "gainsboro" => Colors.Gainsboro,
            "ghostwhite" => Colors.GhostWhite,
            "gold" => Colors.Gold,
            "goldenrod" => Colors.Goldenrod,
            "gray" => Colors.Gray,
            "green" => Colors.Green,
            "grey" => Colors.Gray,
            "greenyellow" => Colors.GreenYellow,
            "honeydew" => Colors.Honeydew,
            "hotpink" => Colors.HotPink,
            "indianred" => Colors.IndianRed,
            "indigo" => Colors.Indigo,
            "ivory" => Colors.Ivory,
            "khaki" => Colors.Khaki,
            "lavender" => Colors.Lavender,
            "lavenderblush" => Colors.LavenderBlush,
            "lawngreen" => Colors.LawnGreen,
            "lemonchiffon" => Colors.LemonChiffon,
            "lightblue" => Colors.LightBlue,
            "lightcoral" => Colors.LightCoral,
            "lightcyan" => Colors.LightCyan,
            "lightgoldenrodyellow" => Colors.LightGoldenrodYellow,
            "lightgrey" => Colors.LightGray,
            "lightgray" => Colors.LightGray,
            "lightgreen" => Colors.LightGreen,
            "lightpink" => Colors.LightPink,
            "lightsalmon" => Colors.LightSalmon,
            "lightseagreen" => Colors.LightSeaGreen,
            "lightskyblue" => Colors.LightSkyBlue,
            "lightslategray" => Colors.LightSlateGray,
            "lightslategrey" => Colors.LightSlateGray,
            "lightsteelblue" => Colors.LightSteelBlue,
            "lightyellow" => Colors.LightYellow,
            "lime" => Colors.Lime,
            "limegreen" => Colors.LimeGreen,
            "linen" => Colors.Linen,
            "magenta" => Colors.Magenta,
            "maroon" => Colors.Maroon,
            "mediumaquamarine" => Colors.MediumAquamarine,
            "mediumblue" => Colors.MediumBlue,
            "mediumorchid" => Colors.MediumOrchid,
            "mediumpurple" => Colors.MediumPurple,
            "mediumseagreen" => Colors.MediumSeaGreen,
            "mediumslateblue" => Colors.MediumSlateBlue,
            "mediumspringgreen" => Colors.MediumSpringGreen,
            "mediumturquoise" => Colors.MediumTurquoise,
            "mediumvioletred" => Colors.MediumVioletRed,
            "midnightblue" => Colors.MidnightBlue,
            "mintcream" => Colors.MintCream,
            "mistyrose" => Colors.MistyRose,
            "moccasin" => Colors.Moccasin,
            "navajowhite" => Colors.NavajoWhite,
            "navy" => Colors.Navy,
            "oldlace" => Colors.OldLace,
            "olive" => Colors.Olive,
            "olivedrab" => Colors.OliveDrab,
            "orange" => Colors.Orange,
            "orangered" => Colors.OrangeRed,
            "orchid" => Colors.Orchid,
            "palegoldenrod" => Colors.PaleGoldenrod,
            "palegreen" => Colors.PaleGreen,
            "paleturquoise" => Colors.PaleTurquoise,
            "palevioletred" => Colors.PaleVioletRed,
            "papayawhip" => Colors.PapayaWhip,
            "peachpuff" => Colors.PeachPuff,
            "peru" => Colors.Peru,
            "pink" => Colors.Pink,
            "plum" => Colors.Plum,
            "powderblue" => Colors.PowderBlue,
            "purple" => Colors.Purple,
            "red" => Colors.Red,
            "rosybrown" => Colors.RosyBrown,
            "royalblue" => Colors.RoyalBlue,
            "saddlebrown" => Colors.SaddleBrown,
            "salmon" => Colors.Salmon,
            "sandybrown" => Colors.SandyBrown,
            "seagreen" => Colors.SeaGreen,
            "seashell" => Colors.SeaShell,
            "sienna" => Colors.Sienna,
            "silver" => Colors.Silver,
            "skyblue" => Colors.SkyBlue,
            "slateblue" => Colors.SlateBlue,
            "slategray" => Colors.SlateGray,
            "slategrey" => Colors.SlateGray,
            "snow" => Colors.Snow,
            "springgreen" => Colors.SpringGreen,
            "steelblue" => Colors.SteelBlue,
            "tan" => Colors.Tan,
            "teal" => Colors.Teal,
            "thistle" => Colors.Thistle,
            "tomato" => Colors.Tomato,
            "transparent" => Colors.Transparent,
            "turquoise" => Colors.Turquoise,
            "violet" => Colors.Violet,
            "wheat" => Colors.Wheat,
            "white" => Colors.White,
            "whitesmoke" => Colors.WhiteSmoke,
            "yellow" => Colors.Yellow,
            "yellowgreen" => Colors.YellowGreen,
            _ => null
        };

        return avaColor;
    }
}
