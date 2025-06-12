#if ANDROID
using AColor = Android.Graphics.Color;
#endif
using SkiaSharp;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using SDColor = System.Drawing.Color;
using AvaColor = Avalonia.Media.Color;

namespace BD.Avalonia8.Media;

partial struct ColorF // operator ==
{
    public static bool operator ==(ColorF left, ColorF right)
    {
        return left.Equals(right);
    }

    public static bool operator ==(ColorF? left, ColorF? right)
    {
        if (left.HasValue)
        {
            if (right.HasValue)
            {
                return left.Value.Equals(right.Value);
            }
        }
        else if (!right.HasValue)
        {
            return true;
        }
        return false;
    }

    public static bool operator ==(ColorF? left, ColorF right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        return false;
    }

    public static bool operator ==(ColorF left, ColorF? right)
    {
        if (right.HasValue)
        {
            return left.Equals(right.Value);
        }
        return false;
    }
}

partial struct ColorF // operator !=
{
    public static bool operator !=(ColorF left, ColorF right)
    {
        return !(left == right);
    }

    public static bool operator !=(ColorF? left, ColorF right)
    {
        if (left.HasValue)
        {
            return !(left.Value == right);
        }
        return true;
    }

    public static bool operator !=(ColorF left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !(left == right.Value);
        }
        return true;
    }

    public static bool operator !=(ColorF? left, ColorF? right)
    {
        if (left.HasValue)
        {
            if (right.HasValue)
            {
                return !(left == right);
            }
        }
        else if (!right.HasValue)
        {
            return false;
        }
        return true;
    }
}

partial struct ColorF // implicit operator To
{
    public static implicit operator ColorF(Vector4 color) => new(color);

    [return: NotNullIfNotNull(nameof(color))]
    public static implicit operator ColorF?(Vector4? color) => color.HasValue ? new(color.Value) : null;

    public static implicit operator ColorF(SDColor color) => new(color.R, color.G, color.B, color.A);

    public static implicit operator ColorF(AvaColor color) => new(color.R, color.G, color.B, color.A);

    [return: NotNullIfNotNull(nameof(color))]
    public static implicit operator ColorF?(SDColor? color) => color.HasValue ? new(color.Value.R, color.Value.G, color.Value.B, color.Value.A) : null;

    [return: NotNullIfNotNull(nameof(color))]
    public static implicit operator ColorF?(AvaColor? color) => color.HasValue ? new(color.Value.R, color.Value.G, color.Value.B, color.Value.A) : null;

    public static implicit operator ColorF(SKColor color) => new(color.Red, color.Green, color.Blue, color.Alpha);

    [return: NotNullIfNotNull(nameof(color))]
    public static implicit operator ColorF?(SKColor? color) => color.HasValue ? new(color.Value.Red, color.Value.Green, color.Value.Blue, color.Value.Alpha) : null;
}

partial struct ColorF // implicit operator From
{
    public static implicit operator Vector4(ColorF color)
    {
        float a, r, g, b;
#if IOS || MACCATALYST || MACOS
        a = (float)color.Alpha.Value;
        r = (float)color.Red.Value;
        g = (float)color.Green.Value;
        b = (float)color.Blue.Value;
#else
        a = sRgbToScRgb(color.Alpha);
        r = sRgbToScRgb(color.Red);
        g = sRgbToScRgb(color.Green);
        b = sRgbToScRgb(color.Blue);
#endif
        return new Vector4(r, g, b, a);
    }

    [return: NotNullIfNotNull(nameof(color))]
    public static implicit operator Vector4?(ColorF? color)
    {
        if (color.HasValue)
        {
            Vector4 r = color.Value;
            return r;
        }
        return null;
    }


    public static implicit operator SDColor(ColorF color)
    {
        color.ToRgba(out var r, out var g, out var b, out var a);
        return SDColor.FromArgb(a, r, g, b);
    }

    [return: NotNullIfNotNull(nameof(color))]

    public static implicit operator SDColor?(ColorF? color)
    {
        if (color.HasValue)
        {
            SDColor r = color.Value;
            return r;
        }
        return null;
    }


    public static implicit operator SKColor(ColorF color)
    {
        color.ToRgba(out var r, out var g, out var b, out var a);
        return new SKColor(r, g, b, a);
    }

    [return: NotNullIfNotNull(nameof(color))]
    public static implicit operator SKColor?(ColorF? color)
    {
        if (color.HasValue)
        {
            SKColor r = color.Value;
            return r;
        }
        return null;
    }


    public static implicit operator AvaColor(ColorF color)
    {
        color.ToRgba(out var r, out var g, out var b, out var a);
        return new AvaColor(r, g, b, a);
    }

    [return: NotNullIfNotNull(nameof(color))]
    public static implicit operator AvaColor?(ColorF? color)
    {
        if (color.HasValue)
        {
            AvaColor r = color.Value;
            return r;
        }
        return null;
    }
}

#if IOS || MACCATALYST
partial struct ColorF // implicit operator To(iOS)
{
    public static implicit operator ColorF(UIColor color)
    {
        color.GetRGBA(out nfloat red, out nfloat green, out nfloat blue, out nfloat alpha);
        return new ColorF(red, green, blue, alpha);
    }

    [return: NotNullIfNotNull(nameof(color))]
    public static implicit operator ColorF?(UIColor? color)
    {
        if (color == null)
        {
            return null;
        }
        else
        {
            color.GetRGBA(out nfloat red, out nfloat green, out nfloat blue, out nfloat alpha);
            return new ColorF(red, green, blue, alpha);
        }
    }
}

partial struct ColorF // implicit operator From(iOS)
{
    public static implicit operator UIColor(ColorF color) => color.ToPlatform();

    [return: NotNullIfNotNull(nameof(color))]
    public static implicit operator UIColor?(ColorF? color) => color.HasValue ? color.Value.ToPlatform() : null;
}
#endif

#if IOS || MACCATALYST || MACOS
partial struct ColorF // implicit operator From(CGColor)
{
    public static implicit operator CGColor(ColorF color) => color.ToCGColor();

    [return: NotNullIfNotNull(nameof(color))]
    public static implicit operator CGColor?(ColorF? color) => color.HasValue ? color.Value.ToCGColor() : null;
}
#endif

#if ANDROID
partial struct ColorF // implicit operator To(Android)
{
    public static implicit operator ColorF(AColor color)
    {
        return new ColorF(color.R, color.G, color.B, color.A);
    }

    [return: NotNullIfNotNull(nameof(color))]
    public static implicit operator ColorF?(AColor? color)
    {
        if (color.HasValue)
        {
            return new ColorF(color.Value.R, color.Value.G, color.Value.B, color.Value.A);
        }
        return null;
    }
}

partial struct ColorF // implicit operator From(Android)
{
    public static implicit operator AColor(ColorF color)
    {
        return new AColor(color.Red, color.Green, color.Blue, color.Alpha);
    }

    [return: NotNullIfNotNull(nameof(color))]
    public static implicit operator AColor?(ColorF? color)
    {
        if (color.HasValue)
        {
            AColor r = color.Value;
            return r;
        }
        return null;
    }
}
#endif

#if MACOS
partial struct ColorF // implicit operator To(macOS)
{
    public static implicit operator ColorF(NSColor color) => color.AsColor();

    [return: NotNullIfNotNull(nameof(color))]
    public static implicit operator ColorF?(NSColor? color) => color?.AsColor();
}

partial struct ColorF // implicit operator From(macOS)
{
    public static implicit operator NSColor(ColorF color)
    {
        return NSColor.FromRgba(color.Red, color.Green, color.Blue, color.Alpha);
    }

    [return: NotNullIfNotNull(nameof(color))]
    public static implicit operator NSColor?(ColorF? color)
    {
        if (color.HasValue)
        {
            NSColor r = color.Value;
            return r;
        }
        return null;
    }
}
#endif