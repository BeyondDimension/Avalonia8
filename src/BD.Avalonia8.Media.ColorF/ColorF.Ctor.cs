// https://github.com/dotnet/maui/blob/9.0.71/src/Graphics/src/Graphics/Color.cs

using Microsoft.Maui.Graphics;
using System.Numerics;

namespace BD.Avalonia8.Media;

#if IOS || MACCATALYST || MACOS
partial struct ColorF // 构造函数（Apple）
{
    public ColorF(nfloat gray)
    {
        Red = Green = Blue = nfloat.Clamp(gray, 0, 1);
    }

    public ColorF(float red, float green, float blue)
    {
        Red = nfloat.Clamp(red, 0, 1);
        Green = nfloat.Clamp(green, 0, 1);
        Blue = nfloat.Clamp(blue, 0, 1);
        Alpha = 1.0f;
    }

    public ColorF(float red, float green, float blue, float alpha)
    {
        Red = nfloat.Clamp(red, 0, 1);
        Green = nfloat.Clamp(green, 0, 1);
        Blue = nfloat.Clamp(blue, 0, 1);
        Alpha = nfloat.Clamp(alpha, 0, 1);
    }

    public ColorF(double red, double green, double blue)
    {
        Red = nfloat.Clamp((nfloat)red, 0, 1);
        Green = nfloat.Clamp((nfloat)green, 0, 1);
        Blue = nfloat.Clamp((nfloat)blue, 0, 1);
        Alpha = 1.0f;
    }

    public ColorF(double red, double green, double blue, double alpha)
    {
        Red = nfloat.Clamp((nfloat)red, 0, 1);
        Green = nfloat.Clamp((nfloat)green, 0, 1);
        Blue = nfloat.Clamp((nfloat)blue, 0, 1);
        Alpha = nfloat.Clamp((nfloat)alpha, 0, 1);
    }

    public ColorF(byte red, byte green, byte blue)
    {
        Red = (red / 255f).Clamp(0, 1);
        Green = (green / 255f).Clamp(0, 1);
        Blue = (blue / 255f).Clamp(0, 1);
        Alpha = 1.0f;
    }

    public ColorF(byte red, byte green, byte blue, byte alpha)
    {
        Red = (red / 255f).Clamp(0, 1);
        Green = (green / 255f).Clamp(0, 1);
        Blue = (blue / 255f).Clamp(0, 1);
        Alpha = (alpha / 255f).Clamp(0, 1);
    }

    public ColorF(int red, int green, int blue)
    {
        Red = (red / 255f).Clamp(0, 1);
        Green = (green / 255f).Clamp(0, 1);
        Blue = (blue / 255f).Clamp(0, 1);
        Alpha = 1.0f;
    }

    public ColorF(int red, int green, int blue, int alpha)
    {
        Red = (red / 255f).Clamp(0, 1);
        Green = (green / 255f).Clamp(0, 1);
        Blue = (blue / 255f).Clamp(0, 1);
        Alpha = (alpha / 255f).Clamp(0, 1);
    }

    public ColorF(Vector4 color)
    {
        Red = color.X.Clamp(0, 1);
        Green = color.Y.Clamp(0, 1);
        Blue = color.Z.Clamp(0, 1);
        Alpha = color.W.Clamp(0, 1);
    }

    public ColorF(nfloat red, nfloat green, nfloat blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = 1.0f;
    }

    public ColorF(nfloat red, nfloat green, nfloat blue, nfloat alpha)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }
}
#else
partial struct ColorF // 构造函数
{
    public ColorF(float gray)
    {
        Red = Green = Blue = ScRgbTosRgb(gray.Clamp(0, 1));
    }

    public ColorF(double gray)
    {
        Red = Green = Blue = ScRgbTosRgb(gray.Clamp(0, 1));
    }

    public ColorF(float red, float green, float blue)
    {
        Red = ScRgbTosRgb(red.Clamp(0, 1));
        Green = ScRgbTosRgb(green.Clamp(0, 1));
        Blue = ScRgbTosRgb(blue.Clamp(0, 1));
        Alpha = byte.MaxValue;
    }

    public ColorF(float red, float green, float blue, float alpha)
    {
        Red = ScRgbTosRgb(red.Clamp(0, 1));
        Green = ScRgbTosRgb(green.Clamp(0, 1));
        Blue = ScRgbTosRgb(blue.Clamp(0, 1));
        Alpha = ScRgbTosRgb(alpha.Clamp(0, 1));
    }

    public ColorF(double red, double green, double blue)
    {
        Red = ScRgbTosRgb(red.Clamp(0, 1));
        Green = ScRgbTosRgb(green.Clamp(0, 1));
        Blue = ScRgbTosRgb(blue.Clamp(0, 1));
        Alpha = byte.MaxValue;
    }

    public ColorF(double red, double green, double blue, double alpha)
    {
        Red = ScRgbTosRgb(red.Clamp(0, 1));
        Green = ScRgbTosRgb(green.Clamp(0, 1));
        Blue = ScRgbTosRgb(blue.Clamp(0, 1));
        Alpha = ScRgbTosRgb(alpha.Clamp(0, 1));
    }

    public ColorF(byte red, byte green, byte blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = byte.MaxValue;
    }

    public ColorF(byte red, byte green, byte blue, byte alpha)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }

    public ColorF(int red, int green, int blue)
    {
        Red = unchecked((byte)red);
        Green = unchecked((byte)green);
        Blue = unchecked((byte)blue);
        Alpha = byte.MaxValue;
    }

    public ColorF(int red, int green, int blue, int alpha)
    {
        Red = unchecked((byte)red);
        Green = unchecked((byte)green);
        Blue = unchecked((byte)blue);
        Alpha = unchecked((byte)alpha);
    }

    public ColorF(Vector4 color)
    {
        Red = ScRgbTosRgb(color.X.Clamp(0, 1));
        Green = ScRgbTosRgb(color.Y.Clamp(0, 1));
        Blue = ScRgbTosRgb(color.Z.Clamp(0, 1));
        Alpha = ScRgbTosRgb(color.W.Clamp(0, 1));
    }
}
#endif