using Microsoft.Maui.Graphics;
using System.Globalization;

namespace BD.Avalonia8.Media;

partial struct ColorF // PrivateMethods(Maui)
{
    // https://github.com/dotnet/maui/blob/9.0.71/src/Graphics/src/Graphics/Color.cs#L918-L1040

    static bool TryParseFourColorRanges(
        ReadOnlySpan<char> value,
        out ReadOnlySpan<char> quad0,
        out ReadOnlySpan<char> quad1,
        out ReadOnlySpan<char> quad2,
        out ReadOnlySpan<char> quad3)
    {
        var op = value.IndexOf('(');
        var cp = value.LastIndexOf(')');
        if (op < 0 || cp < 0 || cp < op)
            goto ReturnFalse;

        value = value.Slice(op + 1, cp - op - 1);

        int index = value.IndexOf(',');
        if (index == -1)
            goto ReturnFalse;
        quad0 = value[..index];
        value = value[(index + 1)..];

        index = value.IndexOf(',');
        if (index == -1)
            goto ReturnFalse;
        quad1 = value[..index];
        value = value[(index + 1)..];

        index = value.IndexOf(',');
        if (index == -1)
            goto ReturnFalse;
        quad2 = value[..index];
        quad3 = value[(index + 1)..];

        // if there are more commas, fail
        if (quad3.IndexOf(',') != -1)
            goto ReturnFalse;

        return true;

    ReturnFalse:
        quad0 = quad1 = quad2 = quad3 = default;
        return false;
    }

    static bool TryParseThreeColorRanges(
        ReadOnlySpan<char> value,
        out ReadOnlySpan<char> triplet0,
        out ReadOnlySpan<char> triplet1,
        out ReadOnlySpan<char> triplet2)
    {
        var op = value.IndexOf('(');
        var cp = value.LastIndexOf(')');
        if (op < 0 || cp < 0 || cp < op)
            goto ReturnFalse;

        value = value.Slice(op + 1, cp - op - 1);

        int index = value.IndexOf(',');
        if (index == -1)
            goto ReturnFalse;
        triplet0 = value[..index];
        value = value[(index + 1)..];

        index = value.IndexOf(',');
        if (index == -1)
            goto ReturnFalse;
        triplet1 = value[..index];
        triplet2 = value[(index + 1)..];

        // if there are more commas, fail
        if (triplet2.IndexOf(',') != -1)
            goto ReturnFalse;

        return true;

    ReturnFalse:
        triplet0 = triplet1 = triplet2 = default;
        return false;
    }

    static bool TryParseColorValue(ReadOnlySpan<char> elem, int maxValue, bool acceptPercent, out double value)
    {
        elem = elem.Trim();
        if (!elem.IsEmpty && elem[^1] == '%' && acceptPercent)
        {
            maxValue = 100;
            elem = elem[..^1];
        }

        if (TryParseDouble(elem, out value))
        {
            value = value.Clamp(0, maxValue) / maxValue;
            return true;
        }
        return false;
    }

    static bool TryParseOpacity(ReadOnlySpan<char> elem, out double value)
    {
        if (TryParseDouble(elem, out value))
        {
            value = value.Clamp(0, 1);
            return true;
        }
        return false;
    }

    static bool TryParseDouble(ReadOnlySpan<char> s, out double value) =>
        double.TryParse(
#if !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
            s.ToString(),
#else
            s,
#endif
            NumberStyles.Number, CultureInfo.InvariantCulture, out value);

    static int ParseInt(ReadOnlySpan<char> s) =>
        int.Parse(
#if !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
            s.ToString(),
#else
            s,
#endif
             NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
}
