#if ANDROID
using AColor = Android.Graphics.Color;
#endif
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BD.Avalonia8.Media;

public class JsonColorConverter : JsonConverter<ColorF>
{
    /// <summary>
    /// 获取从 Json 反序列化时解析失败时的默认值
    /// </summary>
    /// <returns></returns>
    protected virtual ColorF GetDefaultValue() => default;

    /// <summary>
    /// 是否写入 Json 时必定包含 alpha 通道值，默认为 <see langword="false"/> 当 alpha 通道值为最大值时（例如 255 或 1.0f）则不包含 alpha 通道值
    /// </summary>
    /// <returns></returns>
    protected virtual bool GetIncludeAlpha() => false;

    bool IsAlphaMaxValue(ColorF value)
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

    string? GetString(ColorF value)
    {
        string result;
        if (!GetIncludeAlpha() && IsAlphaMaxValue(value))
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
