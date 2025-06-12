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

    protected virtual string? GetString(ColorF value)
    {
        string result;
        if (IsAlphaMaxValue(value))
        {
            result = $"rgb({value.Red}, {value.Green}, {value.Blue})";
        }
        else
        {
            result = $"rgba({value.Red}, {value.Green}, {value.Blue}, {value.Alpha})";
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
