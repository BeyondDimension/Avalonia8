using SDColor = System.Drawing.Color;
using AvaColor = Avalonia.Media.Color;
using System.Numerics;
using SkiaSharp;

namespace BD.Avalonia8.Media;

partial struct ColorF : IEquatable<ColorF>, IEquatable<Vector4>, IEquatable<SDColor>, IEquatable<AvaColor>, IEquatable<ColorF?>, IEquatable<Vector4?>, IEquatable<SDColor?>, IEquatable<AvaColor?>
{
    readonly bool Equals(float a, float r, float g, float b)
    {
#if IOS || MACCATALYST || MACOS
        if (a == (float)Alpha.Value)
        {
            if (r == (float)Red.Value)
            {
                if (g == (float)Green.Value)
                {
                    if (b == (float)Blue.Value)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
#else
        if (a == sRgbToScRgb(Alpha))
        {
            if (r == sRgbToScRgb(Red))
            {
                if (g == sRgbToScRgb(Green))
                {
                    if (b == sRgbToScRgb(Blue))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
#endif
    }

    /// <inheritdoc/>
    public bool Equals(ColorF other)
    {
        return Red.Equals(other.Red) &&
               Green.Equals(other.Green) &&
               Blue.Equals(other.Blue) &&
               Alpha.Equals(other.Alpha);
    }

    /// <inheritdoc/>
    public bool Equals(ColorF? other)
    {
        if (other.HasValue)
        {
            return Equals(other.Value);
        }
        return false;
    }

    /// <inheritdoc/>
    public bool Equals(Vector4 other)
    {
        return Equals(other.X, other.Y, other.Z, other.W);
    }

    /// <inheritdoc/>
    public bool Equals(Vector4? other)
    {
        if (other.HasValue)
        {
            return Equals(other.Value);
        }
        return false;
    }

    /// <inheritdoc/>
    public bool Equals(SDColor other)
    {
#if IOS || MACCATALYST || MACOS
        return ScRgbTosRgb(Red.Value).Equals(other.R) &&
               ScRgbTosRgb(Green.Value).Equals(other.G) &&
               ScRgbTosRgb(Blue.Value).Equals(other.B) &&
               ScRgbTosRgb(Alpha.Value).Equals(other.A);
#else
        return Red.Equals(other.R) &&
               Green.Equals(other.G) &&
               Blue.Equals(other.B) &&
               Alpha.Equals(other.A);
#endif
    }

    /// <inheritdoc/>
    public bool Equals(SDColor? other)
    {
        if (other.HasValue)
        {
            return Equals(other.Value);
        }
        return false;
    }

    /// <inheritdoc/>
    public bool Equals(AvaColor other)
    {
#if IOS || MACCATALYST || MACOS
        return ScRgbTosRgb(Red.Value).Equals(other.R) &&
               ScRgbTosRgb(Green.Value).Equals(other.G) &&
               ScRgbTosRgb(Blue.Value).Equals(other.B) &&
               ScRgbTosRgb(Alpha.Value).Equals(other.A);
#else
        return Red.Equals(other.R) &&
               Green.Equals(other.G) &&
               Blue.Equals(other.B) &&
               Alpha.Equals(other.A);
#endif
    }

    /// <inheritdoc/>
    public bool Equals(AvaColor? other)
    {
        if (other.HasValue)
        {
            return Equals(other.Value);
        }
        return false;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }
        else if (obj is ColorF other)
        {
            return Red.Equals(other.Red) &&
                   Green.Equals(other.Green) &&
                   Blue.Equals(other.Blue) &&
                   Alpha.Equals(other.Alpha);
        }
        else if (obj is SDColor sDColor)
        {
            return Equals(sDColor);
        }
        else if (obj is AvaColor avaColor)
        {
            return Equals(avaColor);
        }
#if IOS || MACCATALYST
        else if (obj is global::UIKit.UIColor uiColor)
        {
            return Equals(uiColor);
        }
        else if (obj is global::CoreImage.CIColor ciColor)
        {
            return Equals(ciColor);
        }
#endif
#if MACOS
        else if (obj is global::AppKit.NSColor nsColor)
        {
            return Equals(nsColor);
        }
#endif
#if ANDROID
        else if (obj is global::Android.Graphics.Color aColor)
        {
            return Equals(aColor);
        }
#endif
        else if (obj is SKColor sKColor)
        {
            return Equals(sKColor);
        }
        else if (obj is Vector4 v4)
        {
            return Equals(v4.X, v4.Y, v4.Z, v4.W);
        }
        else if (obj is int int32)
        {
            return Equals(int32);
        }
        else if (obj is uint uint32)
        {
            return Equals(uint32);
        }
        else if (obj is long int64)
        {
            return Equals(int64);
        }
        else if (obj is ulong uint64)
        {
            return Equals(uint64);
        }
        else if (obj is string str)
        {
            return Equals(str);
        }
        return base.Equals(obj);
    }
}

partial struct ColorF : IEquatable<int>, IEquatable<uint>, IEquatable<long>, IEquatable<ulong>, IEquatable<int?>, IEquatable<uint?>, IEquatable<long?>, IEquatable<ulong?>
{
    /// <inheritdoc/>
    public bool Equals(int other)
    {
        return ToInt() == other;
    }

    /// <inheritdoc/>
    public bool Equals(int? other)
    {
        if (other.HasValue)
        {
            return Equals(other.Value);
        }
        return false;
    }

    /// <inheritdoc/>
    public bool Equals(uint other)
    {
        return ToUint() == other;
    }

    /// <inheritdoc/>
    public bool Equals(uint? other)
    {
        if (other.HasValue)
        {
            return Equals(other.Value);
        }
        return false;
    }

    /// <inheritdoc/>
    public bool Equals(long other)
    {
        return ToInt() == other;
    }

    /// <inheritdoc/>
    public bool Equals(long? other)
    {
        if (other.HasValue)
        {
            return Equals(other.Value);
        }
        return false;
    }

    /// <inheritdoc/>
    public bool Equals(ulong other)
    {
        return ToUint() == other;
    }

    /// <inheritdoc/>
    public bool Equals(ulong? other)
    {
        if (other.HasValue)
        {
            return Equals(other.Value);
        }
        return false;
    }
}

partial struct ColorF : IEquatable<string?>
{
    /// <inheritdoc/>
    public bool Equals(string? other)
    {
        if (TryParse(other, out var c))
        {
            return Equals(c);
        }
        return false;
    }
}

#if IOS || MACCATALYST
partial struct ColorF : IEquatable<global::UIKit.UIColor>, IEquatable<global::CoreImage.CIColor>
{
    /// <inheritdoc/>
    public bool Equals(global::UIKit.UIColor? other)
    {
        if (other == null)
        {
            return false;
        }
        else
        {
            other.GetRGBA(out var r, out var g, out var b, out var a);
            return Red == r &&
                   Green == g &&
                   Blue == b &&
                   Alpha == a;
        }
    }


    /// <inheritdoc/>
    public bool Equals(global::CoreImage.CIColor? other)
    {
        if (other == null)
        {
            return false;
        }
        else
        {
            return Red == other.Red &&
                   Green == other.Green &&
                   Blue == other.Blue &&
                   Alpha == other.Alpha;
        }
    }
}
#endif

#if MACOS
partial struct ColorF : IEquatable<global::AppKit.NSColor>
{
    /// <inheritdoc/>
    public bool Equals(global::AppKit.NSColor? other)
    {
        if (other == null)
        {
            return false;
        }
        else
        {
            var convertedColorspace = other.UsingColorSpace(NSColorSpace.GenericRGBColorSpace);
            convertedColorspace.GetRgba(out var red, out var green, out var blue, out var alpha);
            return Red == red &&
                   Green == green &&
                   Blue == blue &&
                   Alpha == alpha;
        }
    }
}
#endif

#if ANDROID
partial struct ColorF : IEquatable<global::Android.Graphics.Color>, IEquatable<global::Android.Graphics.Color?>
{
    /// <inheritdoc/>
    public bool Equals(global::Android.Graphics.Color other)
    {
        return Red == other.R &&
               Green == other.G &&
               Blue == other.B &&
               Alpha == other.A;
    }

    /// <inheritdoc/>
    public bool Equals(global::Android.Graphics.Color? other)
    {
        if (other.HasValue)
        {
            return Equals(other.Value);
        }
        return false;
    }
}
#endif

partial struct ColorF : IEquatable<SKColor>, IEquatable<SKColor?>
{
    /// <inheritdoc/>
    public bool Equals(SKColor other)
    {
        return Red == other.Red &&
               Green == other.Green &&
               Blue == other.Blue &&
               Alpha == other.Alpha;
    }

    /// <inheritdoc/>
    public bool Equals(SKColor? other)
    {
        if (other.HasValue)
        {
            return Equals(other.Value);
        }
        return false;
    }
}