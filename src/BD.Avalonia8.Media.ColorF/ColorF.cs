#if IOS || MACCATALYST || MACOS
using RGBAType = System.Runtime.InteropServices.NFloat;
#else
using RGBAType = System.Byte;
#endif
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace BD.Avalonia8.Media;

/// <summary>
/// 颜色，用于将颜色指定为红色-绿色-蓝色 (RGB) 值、色调-饱和度-发光度 (HSL) 值、色调-饱和度-值 (HSV) 值或使用颜色名称。 还可以使用 alpha 通道来指明透明度。
/// https://learn.microsoft.com/zh-cn/dotnet/maui/user-interface/graphics/colors
/// <para>RGBA 通常定义为 byte，取值范围 0~255，例如</para>
/// <list type="bullet">
/// <item>(GDIPlus)System.Drawing.Color</item>
/// <item>(WPF)System.Windows.Media.Color</item>
/// <item>(UWP)Windows.UI.Color</item>
/// <item>(WinUI3)Microsoft.UI.Color</item>
/// </list>
/// 在 Apple 平台上，RGBA 通常定义为 float 或 NFloat，取值范围 0.0f~1.0f，例如
/// <list type="bullet">
/// <item>System.Windows.Media.Color 中的 ScARGB(WPF)</item>
/// <item>(iOS)UIKit.UIColor</item>
/// <item></item>
/// </list>
/// 以及游戏引擎中的 Color，例如 Unity、
/// <list type="bullet">
/// <item>(Unity) https://docs.unity.cn/cn/2019.4/ScriptReference/Color.html</item>
/// <item>(Godot) https://docs.godotengine.org/zh-cn/4.x/classes/class_color.html</item>
/// <item></item>
/// </list>
/// </summary>
[DebuggerDisplay("Red={Red}, Green={Green}, Blue={Blue}, Alpha={Alpha}")]
[StructLayout(LayoutKind.Sequential)]
[TypeConverter(typeof(ColorTypeConverter))]
[JsonConverter(typeof(JsonColorConverter))]
public readonly partial struct ColorF
{
    /// <summary>
    /// 表示颜色的红色通道
    /// </summary>
    public readonly RGBAType Red;

    /// <summary>
    /// 表示颜色的绿色通道
    /// </summary>
    public readonly RGBAType Green;

    /// <summary>
    /// 表示颜色的蓝色通道
    /// </summary>
    public readonly RGBAType Blue;

    /// <summary>
    /// 表示颜色的 alpha 通道
    /// </summary>
    public readonly RGBAType Alpha =
#if IOS || MACCATALYST || MACOS
        1;
#else
        byte.MaxValue;
#endif

    /// <summary>
    /// 初始化一个新的 <see cref="ColorF"/> 实例，使用默认的黑色
    /// </summary>
    public ColorF()
    {
        // Default Black
        Red = Green = Blue = 0;
    }
}