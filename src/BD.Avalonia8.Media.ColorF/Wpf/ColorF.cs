namespace BD.Avalonia8.Media;

partial struct ColorF // Microsoft.DotNet.Wpf
{
    // https://github.com/dotnet/wpf/blob/v9.0.6/src/Microsoft.DotNet.Wpf/src/PresentationCore/System/Windows/Media/Color.cs#L1040

    ///<summary>
    /// private helper function to set context values from a color value with a set context and ScRgb values
    ///</summary>
#pragma warning disable IDE1006 // 命名样式
    public static float sRgbToScRgb(byte bval)
#pragma warning restore IDE1006 // 命名样式
    {
        float val = bval / 255.0f;

        if (!(val > 0.0))       // Handles NaN case too. (Though, NaN isn't actually
                                // possible in this case.)
        {
            return 0.0f;
        }
        else if (val <= 0.04045)
        {
            return val / 12.92f;
        }
        else if (val < 1.0f)
        {
#if NETCOREAPP2_0_OR_GREATER || NET462_OR_GREATER || NETSTANDARD2_0_OR_GREATER
            return MathF.Pow(((float)val + 0.055f) / 1.055f, 2.4f);
#else
            return (float)Math.Pow(((double)val + 0.055) / 1.055, 2.4);
#endif
        }
        else
        {
            return 1.0f;
        }
    }

    ///<summary>
    /// private helper function to set context values from a color value with a set context and ScRgb values
    ///</summary>
#pragma warning disable IDE1006 // 命名样式
    public static double sRgbToScRgbD(byte bval)
#pragma warning restore IDE1006 // 命名样式
    {
        double val = bval / 255.0;

        if (!(val > 0.0))       // Handles NaN case too. (Though, NaN isn't actually
                                // possible in this case.)
        {
            return 0.0f;
        }
        else if (val <= 0.04045)
        {
            return val / 12.92;
        }
        else if (val < 1.0)
        {
            return Math.Pow(((double)val + 0.055) / 1.055, 2.4);
        }
        else
        {
            return 1.0;
        }
    }

    ///<summary>
    /// private helper function to set context values from a color value with a set context and ScRgb values
    ///</summary>
    ///
    public static byte ScRgbTosRgb(float val)
    {
        if (!(val > 0.0))       // Handles NaN case too
        {
            return 0;
        }
        else if (val <= 0.0031308)
        {
            return (byte)((255.0f * val * 12.92f) + 0.5f);
        }
        else if (val < 1.0)
        {
#if NETCOREAPP2_0_OR_GREATER || NET462_OR_GREATER || NETSTANDARD2_0_OR_GREATER
            var f = MathF.Pow((float)val, 1.0f / 2.4f);
#else
            var f = (float)Math.Pow((double)val, 1.0 / 2.4);
#endif
            return (byte)((255.0f * ((1.055f * f) - 0.055f)) + 0.5f);
        }
        else
        {
            return 255;
        }
    }

    ///<summary>
    /// private helper function to set context values from a color value with a set context and ScRgb values
    ///</summary>
    ///
    public static byte ScRgbTosRgb(double val)
    {
        if (!(val > 0.0))       // Handles NaN case too
        {
            return 0;
        }
        else if (val <= 0.0031308)
        {
            return (byte)((255.0f * val * 12.92f) + 0.5f);
        }
        else if (val < 1.0)
        {
            return (byte)((255.0f * ((1.055f * Math.Pow(val, 1.0 / 2.4)) - 0.055f)) + 0.5f);
        }
        else
        {
            return 255;
        }
    }
}