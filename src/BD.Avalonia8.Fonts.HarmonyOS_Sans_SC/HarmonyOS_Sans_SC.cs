using FontFamily = global::Avalonia.Media.FontFamily;

namespace BD.Avalonia8.Fonts;

/// <summary>
/// HarmonyOS Sans SC 字体是 HarmonyOS 的官方字体，支持多种语言和格式，适用于 HarmonyOS 应用的设计和开发。在华为开发者联盟设计资源库，您可以免费下载鸿蒙 Sans 字体包，以及其他 HarmonyOS 的图标、色彩、音效等资源。
/// </summary>
public static partial class HarmonyOS_Sans_SC
{
    /// <summary>
    /// 自定义字体的 Avalonia Res Uri
    /// </summary>
    public const string Name = "avares://BD.Avalonia8.Fonts.HarmonyOS_Sans_SC/HarmonyOS_Sans_SC_Regular.ttf#HarmonyOS Sans SC";

    static readonly Lazy<FontFamily> mInstance = new(() => new(Name), LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// 自定义字体的 <see cref="FontFamily"/>
    /// </summary>
    public static FontFamily Instance => mInstance.Value;
}
