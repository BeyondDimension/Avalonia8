using Avalonia.Data.Converters;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using SkiaSharp;
using SDColor = System.Drawing.Color;
using AvaColor = Avalonia.Media.Color;

namespace BD.Avalonia8.Media;

public sealed partial class ColorTypeConverter : TypeConverter
{
    // https://github.com/dotnet/maui/blob/9.0.71/src/Graphics/src/Graphics/Converters/ColorTypeConverter.cs

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object fromValue)
    {
        var r = ColorF.Converter.ConvertFrom(fromValue);
        if (!r.HasValue && fromValue != null)
        {
            return ColorF.Parse(fromValue.ToString());
        }
        else if (r.HasValue)
        {
            return r;
        }
        return default(ColorF);
    }

    static object? GetDefaultValue(Type t)
    {
        if (t.IsValueType)
        {
#pragma warning disable IL2067 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.
            try
            {
                return Activator.CreateInstance(t);
            }
            catch
            {
                try
                {
                    return Activator.CreateInstance(t, true);
                }
                catch
                {
                }
            }
#pragma warning restore IL2067 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.
        }
        return null;
    }

    /// <inheritdoc/>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value == null)
        {
            return GetDefaultValue(destinationType);
        }

        if (value.GetType() == destinationType)
        {
            return value;
        }

        var value2 = ColorF.Converter.ConvertFrom(value);
        if (!value2.HasValue)
        {
            return GetDefaultValue(destinationType);
        }
        var r = ColorF.Converter.ConvertTo(value2.Value, destinationType);
        return r;
    }

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
        => new(ColorF.Converter.GetKnownColors());

    public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context)
        => false;

    public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
        => true;

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        var r = ColorF.Converter.CanConvert(sourceType);
        return r;
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        if (destinationType == null)
        {
            return false;
        }
        var r = ColorF.Converter.CanConvert(destinationType);
        return r;
    }
}