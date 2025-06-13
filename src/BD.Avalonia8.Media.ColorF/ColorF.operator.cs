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

partial struct ColorF // operator ==(string)
{
    public static bool operator ==(ColorF left, string? right)
    {
        return left.Equals(right);
    }

    public static bool operator ==(string? left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF? left, string? right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return right == null;
        }
    }

    public static bool operator ==(string? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return left == null;
        }
    }
}

partial struct ColorF // operator !=(string)
{
    public static bool operator !=(ColorF left, string? right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(string? left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF? left, string? right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return right != null;
        }
    }

    public static bool operator !=(string? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return left != null;
        }
    }
}

partial struct ColorF // operator ==(int)
{
    public static bool operator ==(ColorF left, [NotNullWhen(true)] int? right)
    {
        return left.Equals(right);
    }

    public static bool operator ==([NotNullWhen(true)] int? left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF left, int right)
    {
        return left.Equals(right);
    }

    public static bool operator ==(int left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF? left, int? right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return !right.HasValue;
        }
    }

    public static bool operator ==(int? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return !left.HasValue;
        }
    }

    public static bool operator ==(ColorF? left, int right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return false;
        }
    }

    public static bool operator ==(int left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return false;
        }
    }
}

partial struct ColorF // operator !=(int)
{
    public static bool operator !=(ColorF left, int? right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(int? left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF left, int right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(int left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF? left, int? right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return right.HasValue;
        }
    }

    public static bool operator !=(int? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return left.HasValue;
        }
    }

    public static bool operator !=(ColorF? left, int right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return true;
        }
    }

    public static bool operator !=(int left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return true;
        }
    }
}

partial struct ColorF // operator ==(uint)
{
    public static bool operator ==(ColorF left, [NotNullWhen(true)] uint? right)
    {
        return left.Equals(right);
    }

    public static bool operator ==([NotNullWhen(true)] uint? left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF left, uint right)
    {
        return left.Equals(right);
    }

    public static bool operator ==(uint left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF? left, uint? right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return !right.HasValue;
        }
    }

    public static bool operator ==(uint? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return !left.HasValue;
        }
    }

    public static bool operator ==(ColorF? left, uint right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return false;
        }
    }

    public static bool operator ==(uint left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return false;
        }
    }
}

partial struct ColorF // operator !=(uint)
{
    public static bool operator !=(ColorF left, uint? right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(uint? left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF left, uint right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(uint left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF? left, uint? right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return right.HasValue;
        }
    }

    public static bool operator !=(uint? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return left.HasValue;
        }
    }

    public static bool operator !=(ColorF? left, uint right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return true;
        }
    }

    public static bool operator !=(uint left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return true;
        }
    }
}

partial struct ColorF // operator ==(long)
{
    public static bool operator ==(ColorF left, [NotNullWhen(true)] long? right)
    {
        return left.Equals(right);
    }

    public static bool operator ==([NotNullWhen(true)] long? left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF left, long right)
    {
        return left.Equals(right);
    }

    public static bool operator ==(long left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF? left, long? right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return !right.HasValue;
        }
    }

    public static bool operator ==(long? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return !left.HasValue;
        }
    }

    public static bool operator ==(ColorF? left, long right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return false;
        }
    }

    public static bool operator ==(long left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return false;
        }
    }
}

partial struct ColorF // operator !=(long)
{
    public static bool operator !=(ColorF left, long? right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(long? left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF left, long right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(long left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF? left, long? right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return right.HasValue;
        }
    }

    public static bool operator !=(long? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return left.HasValue;
        }
    }

    public static bool operator !=(ColorF? left, long right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return true;
        }
    }

    public static bool operator !=(long left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return true;
        }
    }
}

partial struct ColorF // operator ==(ulong)
{
    public static bool operator ==(ColorF left, [NotNullWhen(true)] ulong? right)
    {
        return left.Equals(right);
    }

    public static bool operator ==([NotNullWhen(true)] ulong? left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF left, ulong right)
    {
        return left.Equals(right);
    }

    public static bool operator ==(ulong left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF? left, ulong? right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return !right.HasValue;
        }
    }

    public static bool operator ==(ulong? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return !left.HasValue;
        }
    }

    public static bool operator ==(ColorF? left, ulong right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return false;
        }
    }

    public static bool operator ==(ulong left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return false;
        }
    }
}

partial struct ColorF // operator !=(ulong)
{
    public static bool operator !=(ColorF left, ulong? right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(ulong? left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF left, ulong right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(ulong left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF? left, ulong? right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return right.HasValue;
        }
    }

    public static bool operator !=(ulong? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return left.HasValue;
        }
    }

    public static bool operator !=(ColorF? left, ulong right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return true;
        }
    }

    public static bool operator !=(ulong left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return true;
        }
    }
}

partial struct ColorF // operator ==(Vector4)
{
    public static bool operator ==(ColorF left, [NotNullWhen(true)] Vector4? right)
    {
        return left.Equals(right);
    }

    public static bool operator ==([NotNullWhen(true)] Vector4? left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF left, Vector4 right)
    {
        return left.Equals(right);
    }

    public static bool operator ==(Vector4 left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF? left, Vector4? right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return !right.HasValue;
        }
    }

    public static bool operator ==(Vector4? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return !left.HasValue;
        }
    }

    public static bool operator ==(ColorF? left, Vector4 right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return false;
        }
    }

    public static bool operator ==(Vector4 left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return false;
        }
    }
}

partial struct ColorF // operator !=(Vector4)
{
    public static bool operator !=(ColorF left, Vector4? right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(Vector4? left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF left, Vector4 right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(Vector4 left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF? left, Vector4? right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return right.HasValue;
        }
    }

    public static bool operator !=(Vector4? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return left.HasValue;
        }
    }

    public static bool operator !=(ColorF? left, Vector4 right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return true;
        }
    }

    public static bool operator !=(Vector4 left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return true;
        }
    }
}

partial struct ColorF // operator ==(SDColor)
{
    public static bool operator ==(ColorF left, [NotNullWhen(true)] SDColor? right)
    {
        return left.Equals(right);
    }

    public static bool operator ==([NotNullWhen(true)] SDColor? left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF left, SDColor right)
    {
        return left.Equals(right);
    }

    public static bool operator ==(SDColor left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF? left, SDColor? right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return !right.HasValue;
        }
    }

    public static bool operator ==(SDColor? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return !left.HasValue;
        }
    }

    public static bool operator ==(ColorF? left, SDColor right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return false;
        }
    }

    public static bool operator ==(SDColor left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return false;
        }
    }
}

partial struct ColorF // operator !=(SDColor)
{
    public static bool operator !=(ColorF left, SDColor? right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(SDColor? left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF left, SDColor right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(SDColor left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF? left, SDColor? right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return right.HasValue;
        }
    }

    public static bool operator !=(SDColor? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return left.HasValue;
        }
    }

    public static bool operator !=(ColorF? left, SDColor right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return true;
        }
    }

    public static bool operator !=(SDColor left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return true;
        }
    }
}

partial struct ColorF // operator ==(AvaColor)
{
    public static bool operator ==(ColorF left, [NotNullWhen(true)] AvaColor? right)
    {
        return left.Equals(right);
    }

    public static bool operator ==([NotNullWhen(true)] AvaColor? left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF left, AvaColor right)
    {
        return left.Equals(right);
    }

    public static bool operator ==(AvaColor left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF? left, AvaColor? right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return !right.HasValue;
        }
    }

    public static bool operator ==(AvaColor? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return !left.HasValue;
        }
    }

    public static bool operator ==(ColorF? left, AvaColor right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return false;
        }
    }

    public static bool operator ==(AvaColor left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return false;
        }
    }
}

partial struct ColorF // operator !=(AvaColor)
{
    public static bool operator !=(ColorF left, AvaColor? right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(AvaColor? left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF left, AvaColor right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(AvaColor left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF? left, AvaColor? right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return right.HasValue;
        }
    }

    public static bool operator !=(AvaColor? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return left.HasValue;
        }
    }

    public static bool operator !=(ColorF? left, AvaColor right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return true;
        }
    }

    public static bool operator !=(AvaColor left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return true;
        }
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

    public static implicit operator ColorF?(string? color) => TryParse(color, out var r) ? r : default;

    public static implicit operator ColorF?(ReadOnlySpan<char> color) => TryParse(color, out var r) ? r : default;
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
        return new AvaColor(a, r, g, b);
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

    public static implicit operator string(ColorF color) => color.ToArgbHex();

    public static implicit operator string?(ColorF? color) => color.HasValue ? color.Value.ToArgbHex() : null;
}

#if IOS || MACCATALYST
partial struct ColorF // operator ==(UIColor)
{
    public static bool operator ==(ColorF left, [NotNullWhen(true)] UIColor? right)
    {
        return left.Equals(right);
    }

    public static bool operator ==([NotNullWhen(true)] UIColor? left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF? left, UIColor? right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return right == null;
        }
    }

    public static bool operator ==(UIColor? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return left == null;
        }
    }
}

partial struct ColorF // operator !=(UIColor)
{
    public static bool operator !=(ColorF left, UIColor? right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(UIColor? left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF? left, UIColor? right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return right != null;
        }
    }

    public static bool operator !=(UIColor? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return left != null;
        }
    }
}

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
partial struct ColorF // operator ==(CGColor)
{
    public static bool operator ==(ColorF left, [NotNullWhen(true)] CGColor? right)
    {
        return left.Equals(right);
    }

    public static bool operator ==([NotNullWhen(true)] CGColor? left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF? left, CGColor? right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return right == (object?)null;
        }
    }

    public static bool operator ==(CGColor? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return left == (object?)null;
        }
    }
}

partial struct ColorF // operator !=(CGColor)
{
    public static bool operator !=(ColorF left, CGColor? right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(CGColor? left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF? left, CGColor? right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return right != (object?)null;
        }
    }

    public static bool operator !=(CGColor? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return left != (object?)null;
        }
    }
}

partial struct ColorF // implicit operator From(CGColor)
{
    public static implicit operator CGColor(ColorF color) => color.ToCGColor();

    [return: NotNullIfNotNull(nameof(color))]
    public static implicit operator CGColor?(ColorF? color) => color.HasValue ? color.Value.ToCGColor() : null;
}
#endif

#if ANDROID
partial struct ColorF // operator ==(AColor)
{
    public static bool operator ==(ColorF left, [NotNullWhen(true)] AColor? right)
    {
        return left.Equals(right);
    }

    public static bool operator ==([NotNullWhen(true)] AColor? left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF left, AColor right)
    {
        return left.Equals(right);
    }

    public static bool operator ==(AColor left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF? left, AColor? right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return !right.HasValue;
        }
    }

    public static bool operator ==(AColor? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return !left.HasValue;
        }
    }

    public static bool operator ==(ColorF? left, AColor right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return false;
        }
    }

    public static bool operator ==(AColor left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return false;
        }
    }
}

partial struct ColorF // operator !=(AColor)
{
    public static bool operator !=(ColorF left, AColor? right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(AColor? left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF left, AColor right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(AColor left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF? left, AColor? right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return right.HasValue;
        }
    }

    public static bool operator !=(AColor? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return left.HasValue;
        }
    }

    public static bool operator !=(ColorF? left, AColor right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return true;
        }
    }

    public static bool operator !=(AColor left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return true;
        }
    }
}

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
partial struct ColorF // operator ==(NSColor)
{
    public static bool operator ==(ColorF left, [NotNullWhen(true)] NSColor? right)
    {
        return left.Equals(right);
    }

    public static bool operator ==([NotNullWhen(true)] NSColor? left, ColorF right)
    {
        return right.Equals(left);
    }

    public static bool operator ==(ColorF? left, NSColor? right)
    {
        if (left.HasValue)
        {
            return left.Value.Equals(right);
        }
        else
        {
            return right == null;
        }
    }

    public static bool operator ==(NSColor? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return right.Value.Equals(left);
        }
        else
        {
            return left == null;
        }
    }
}

partial struct ColorF // operator !=(NSColor)
{
    public static bool operator !=(ColorF left, NSColor? right)
    {
        return !left.Equals(right);
    }

    public static bool operator !=(NSColor? left, ColorF right)
    {
        return !right.Equals(left);
    }

    public static bool operator !=(ColorF? left, NSColor? right)
    {
        if (left.HasValue)
        {
            return !left.Value.Equals(right);
        }
        else
        {
            return right != null;
        }
    }

    public static bool operator !=(NSColor? left, ColorF? right)
    {
        if (right.HasValue)
        {
            return !right.Value.Equals(left);
        }
        else
        {
            return left != null;
        }
    }
}

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