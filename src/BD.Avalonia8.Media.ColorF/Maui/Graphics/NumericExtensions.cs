using System.ComponentModel;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Maui.Graphics;

/// <summary>
/// https://github.com/dotnet/maui/blob/9.0.71/src/Graphics/src/Graphics/NumericExtensions.cs
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
static class NumericExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(this float self, float min, float max)
    {
        if (max < min)
        {
            return max;
        }
        else if (self < min)
        {
            return min;
        }
        else if (self > max)
        {
            return max;
        }

        return self;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Clamp(this double self, double min, double max)
    {
        if (max < min)
        {
            return max;
        }
        else if (self < min)
        {
            return min;
        }
        else if (self > max)
        {
            return max;
        }

        return self;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(this int self, int min, int max)
    {
        if (max < min)
        {
            return max;
        }
        else if (self < min)
        {
            return min;
        }
        else if (self > max)
        {
            return max;
        }

        return self;
    }
}
