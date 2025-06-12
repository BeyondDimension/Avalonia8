#if ANDROID
using AColor = Android.Graphics.Color;
#endif
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using SkiaSharp;
using SDColor = System.Drawing.Color;
using AvaColor = Avalonia.Media.Color;
using System.Text;
using System.Runtime.CompilerServices;

namespace BD.Avalonia8.Media;

public class JsonColorConverter : JsonConverter<ColorF>
{
    protected virtual ColorF GetDefaultValue() => default;

    protected virtual bool IsAlphaMaxValue(ColorF value)
    {
#if IOS || MACCATALYST || MACOS
        if (value.Alpha == 1)
#else
        if (value.Alpha == byte.MaxValue)
#endif
        {
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if IOS || MACCATALYST || MACOS
    static FormattableString _(nfloat t)
    {
        var s = t.Value.ToString();
        return $"{s}{(s.Contains('.') ? null : ".0")}";
    }
#else
    static string _(byte t)
    {
        var s = t.ToString();
        return s;
    }
#endif

    protected virtual string? GetString(ColorF value)
    {
        string result;
        if (IsAlphaMaxValue(value))
        {
            result =
                $"rgb({_(value.Red)}, {_(value.Green)}, {_(value.Blue)})";
        }
        else
        {
            result =
                $"rgba({_(value.Red)}, {_(value.Green)}, {_(value.Blue)}, {_(value.Alpha)})";
        }
        return result;
    }

    /// <inheritdoc/>
    public override ColorF Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        if (ColorF.TryParse(str, out var colorF))
        {
            return colorF;
        }
        return GetDefaultValue();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, ColorF value, JsonSerializerOptions options)
    {
        var result = GetString(value);
        writer.WriteStringValue(result);
    }
}
