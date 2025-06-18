#if ANDROID
using AColor = Android.Graphics.Color;
#endif
using System.Numerics;
using SkiaSharp;
using SDColor = System.Drawing.Color;
using AvaColor = Avalonia.Media.Color;

namespace BD.Avalonia8.Media;

partial struct ColorF
{
    public static class Converter
    {
        public static string[] GetKnownColors() => KnownColors.values;

        public static int GetNamedColorKeyMaxLength() => KnownColors.maxLength.Value;

        public static bool CanConvert(Type t) =>
            t == typeof(ColorF) ||
            t == typeof(ColorF?) ||
#if ANDROID
            t == typeof(AColor) ||
            t == typeof(AColor?) ||
#endif
#if MACOS
            t == typeof(NSColor) ||
#endif
#if IOS || MACCATALYST
            t == typeof(UIColor) ||
#endif
            t == typeof(string) ||
            t == typeof(Vector4) ||
            t == typeof(Vector4?) ||
            t == typeof(AvaColor) ||
            t == typeof(AvaColor?) ||
            t == typeof(SDColor) ||
            t == typeof(SDColor?) ||
            t == typeof(SKColor) ||
            t == typeof(SKColor?) ||
            t == typeof(int) ||
            t == typeof(int?) ||
            t == typeof(uint) ||
            t == typeof(uint?);

        public static ColorF? ConvertFrom(object? value)
        {
            if (value is null)
            {
                return null;
            }
            else if (value is ColorF f)
            {
                return f;
            }
            else if (value is string str)
            {
                return ColorF.Parse(str);
            }
#if ANDROID
            else if (value is AColor aColor)
            {
                ColorF r = aColor;
                return r;
            }
#endif
#if MACOS
            else if (value is NSColor nsColor)
            {
                ColorF r = nsColor;
                return r;
            }
#endif
#if IOS || MACCATALYST
            else if (value is UIColor uiColor)
            {
                ColorF r = uiColor;
                return r;
            }
#endif
            else if (value is Vector4 vec)
            {
                return new ColorF(vec);
            }
            else if (value is AvaColor avaColor)
            {
                ColorF r = avaColor;
                return r;
            }
            else if (value is SDColor sDColor)
            {
                ColorF r = sDColor;
                return r;
            }
            else if (value is SKColor sKColor)
            {
                ColorF r = sKColor;
                return r;
            }
            else if (value is int int32)
            {
                return ColorF.FromInt(int32);
            }
            else if (value is uint uInt32)
            {
                return ColorF.FromUint(uInt32);
            }
            return null;
        }

        public static object? ConvertTo(ColorF value, Type t)
        {
            if (t == typeof(string))
            {
                return value.ToRgbaHex();
            }
            else if (t == typeof(ColorF))
            {
                return value;
            }
            else if (t == typeof(AvaColor))
            {
                AvaColor r = value;
                return r;
            }
            else if (t == typeof(SDColor))
            {
                SDColor r = value;
                return r;
            }
            else if (t == typeof(SKColor))
            {
                SKColor r = value;
                return r;
            }
            else if (t == typeof(Vector4))
            {
                Vector4 r = value;
                return r;
            }
            else if (t == typeof(int))
            {
                return value.ToInt();
            }
            else if (t == typeof(uint))
            {
                return value.ToUint();
            }
#if ANDROID
            else if (t == typeof(AColor))
            {
                AColor r = value;
                return r;
            }
#endif
#if MACOS
            else if (t == typeof(NSColor))
            {
                NSColor r = value;
                return r;
            }
#endif
#if IOS || MACCATALYST
            else if (t == typeof(UIColor))
            {
                UIColor r = value;
                return r;
            }
#endif
            return null;
        }
    }
}

file static class KnownColors
{
    internal static readonly string[] values =
[
    "AliceBlue",
    "AntiqueWhite",
    "Aqua",
    "Aquamarine",
    "Azure",
    "Beige",
    "Bisque",
    "Black",
    "BlanchedAlmond",
    "Blue",
    "BlueViolet",
    "Brown",
    "BurlyWood",
    "CadetBlue",
    "Chartreuse",
    "Chocolate",
    "Coral",
    "CornflowerBlue",
    "Cornsilk",
    "Crimson",
    "Cyan",
    "DarkBlue",
    "DarkCyan",
    "DarkGoldenrod",
    "DarkGray",
    "DarkGreen",
    "DarkGrey",
    "DarkKhaki",
    "DarkMagenta",
    "DarkOliveGreen",
    "DarkOrange",
    "DarkOrchid",
    "DarkRed",
    "DarkSalmon",
    "DarkSeaGreen",
    "DarkSlateBlue",
    "DarkSlateGray",
    "DarkSlateGrey",
    "DarkTurquoise",
    "DarkViolet",
    "DeepPink",
    "DeepSkyBlue",
    "DimGray",
    "DimGrey",
    "DodgerBlue",
    "Firebrick",
    "FloralWhite",
    "ForestGreen",
    "Fuchsia",
    "Gainsboro",
    "GhostWhite",
    "Gold",
    "Goldenrod",
    "Gray",
    "Green",
    "GreenYellow",
    "Grey",
    "Honeydew",
    "HotPink",
    "IndianRed",
    "Indigo",
    "Ivory",
    "Khaki",
    "Lavender",
    "LavenderBlush",
    "LawnGreen",
    "LemonChiffon",
    "LightBlue",
    "LightCoral",
    "LightCyan",
    "LightGoldenrodYellow",
    "LightGray",
    "LightGreen",
    "LightGrey",
    "LightPink",
    "LightSalmon",
    "LightSeaGreen",
    "LightSkyBlue",
    "LightSlateGray",
    "LightSlateGrey",
    "LightSteelBlue",
    "LightYellow",
    "Lime",
    "LimeGreen",
    "Linen",
    "Magenta",
    "Maroon",
    "MediumAquamarine",
    "MediumBlue",
    "MediumOrchid",
    "MediumPurple",
    "MediumSeaGreen",
    "MediumSlateBlue",
    "MediumSpringGreen",
    "MediumTurquoise",
    "MediumVioletRed",
    "MidnightBlue",
    "MintCream",
    "MistyRose",
    "Moccasin",
    "NavajoWhite",
    "Navy",
    "OldLace",
    "Olive",
    "OliveDrab",
    "Orange",
    "OrangeRed",
    "Orchid",
    "PaleGoldenrod",
    "PaleGreen",
    "PaleTurquoise",
    "PaleVioletRed",
    "PapayaWhip",
    "PeachPuff",
    "Peru",
    "Pink",
    "Plum",
    "PowderBlue",
    "Purple",
    "Red",
    "RosyBrown",
    "RoyalBlue",
    "SaddleBrown",
    "Salmon",
    "SandyBrown",
    "SeaGreen",
    "SeaShell",
    "Sienna",
    "Silver",
    "SkyBlue",
    "SlateBlue",
    "SlateGray",
    "SlateGrey",
    "Snow",
    "SpringGreen",
    "SteelBlue",
    "Tan",
    "Teal",
    "Thistle",
    "Tomato",
    "Transparent",
    "Turquoise",
    "Violet",
    "Wheat",
    "White",
    "WhiteSmoke",
    "Yellow",
    "YellowGreen"
];

    internal static Lazy<int> maxLength = new(() =>
    {
        var max = values.Max(static x => x.Length);
        return max;
    });
}