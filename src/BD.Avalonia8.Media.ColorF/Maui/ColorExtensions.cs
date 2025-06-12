#if ANDROID
using AColor = Android.Graphics.Color;
#endif

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace BD.Avalonia8.Media;

public static partial class ColorExtensions
{
}

#if IOS || MACCATALYST
static partial class ColorExtensions // iOS
{
    // https://github.com/dotnet/maui/blob/9.0.71/src/Core/src/Platform/iOS/ColorExtensions.cs#L10

    public static CGColor ToCGColor(this ColorF color)
    {
        return color.ToPlatform().CGColor;
    }

    public static ColorF? ToColor(this UIColor color)
    {
        if (color == null)
            return null;

        color.GetRGBA(out nfloat red, out nfloat green, out nfloat blue, out nfloat alpha);

        return new ColorF(red, green, blue, alpha);
    }

    public static UIColor ToPlatform(this ColorF color)
    {
        return new UIColor(color.Red, color.Green, color.Blue, color.Alpha);
    }

    public static UIColor? ToPlatform(this ColorF? color, ColorF? defaultColor)
            => color?.ToPlatform() ?? defaultColor?.ToPlatform();

    public static UIColor ToPlatform(this ColorF? color, UIColor defaultColor)
        => color?.ToPlatform() ?? defaultColor;

    //internal static bool AreEqual(UIColor a, UIColor b)
    //{
    //    a.GetRGBA(out nfloat aRed, out nfloat aGreen, out nfloat aBlue, out nfloat aAlpha);
    //    b.GetRGBA(out nfloat bRed, out nfloat bGreen, out nfloat bBlue, out nfloat bAlpha);

    //    var redMatches = aRed == bRed;
    //    var greenMatches = aGreen == bGreen;
    //    var blueMatches = aBlue == bBlue;
    //    var alphaMatches = aAlpha == bAlpha;

    //    return redMatches && greenMatches && blueMatches && alphaMatches;
    //}
}
#endif

#if ANDROID
static partial class ColorExtensions // Android
{
    // https://github.com/dotnet/maui/blob/9.0.71/src/Core/src/Platform/Android/ColorExtensions.cs

    public static AColor ToPlatform(this ColorF self)
    {
        return new AColor(self.Red, self.Green, self.Blue, self.Alpha);
    }

    //public static AColor ToPlatform(this ColorF self, int defaultColorResourceId, Context context)
    //    => self?.ToPlatform() ?? new AColor(ContextCompat.GetColor(context, defaultColorResourceId));

    public static AColor ToPlatform(this ColorF? self, ColorF defaultColor)
        => self?.ToPlatform() ?? defaultColor.ToPlatform();

    public static ColorF ToColor(this uint color)
    {
        return ColorF.FromUint(color);
    }

    public static ColorF ToColor(this AColor color)
    {
        return ColorF.FromInt(color.ToArgb());
    }
}
#endif

#if MACOS
static partial class ColorExtensions // macOS
{
    // https://github.com/dotnet/maui/blob/9.0.71/src/Graphics/src/Graphics/Platforms/Mac/NSColorExtensions.cs#L5

    public static ColorF AsColor(this NSColor color)
    {
        var convertedColorspace = color.UsingColorSpace(NSColorSpace.GenericRGBColorSpace);
        convertedColorspace.GetRgba(out var red, out var green, out var blue, out var alpha);
        return new ColorF(red, green, blue, alpha);
    }

    public static CGColor ToCGColor(this ColorF color)
    {
        return new CGColor(color.Red, color.Green, color.Blue, color.Alpha);
    }
}
#endif