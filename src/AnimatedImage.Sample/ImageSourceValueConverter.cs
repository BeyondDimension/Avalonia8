namespace AnimatedImage.Sample;

#pragma warning disable SA1600 // Elements should be documented

sealed class ImageSourceValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return null;
        try
        {
            var valueStr = value!.ToString();
            using var stream = AssetLoader.Open(new Uri(
                $"avares://AnimatedImage.Sample/Images/{valueStr}"));
            if (FileFormat.IsImage(stream, out var format))
            {
                return $"[{format}] {valueStr}";
            }
        }
        catch
        {
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
