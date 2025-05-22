using FontFamily = global::Avalonia.Media.FontFamily;

namespace BD.Avalonia8.Fonts;

/// <summary>
/// MiSans 字体是由小米主导，联合蒙纳字库、汉仪字库共同打造的全球语言字体定制项目。这是一个庞大的字体家族，涵盖 20 多种书写系统，支持 600 多种语言，字符数量超过 10 万个。作为 Xiaomi HyperOS 系统默认字体，我们以简约/清晰，人文/易读，统一的视觉风格为基本原则出发，构建多语言信息体验一致性，旨在帮助为 Xiaomi HyperOS 提供互联的通用体验
/// </summary>
public static partial class MiSans
{
    /// <summary>
    /// 自定义字体的 Avalonia Res Uri
    /// </summary>
    public const string Name = "avares://BD.Avalonia8.Fonts.MiSans/MiSans-Regular.ttf#MiSans";

    static readonly Lazy<FontFamily> mInstance = new(() => new(Name), LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// 自定义字体的 <see cref="FontFamily"/>
    /// </summary>
    public static FontFamily Instance => mInstance.Value;
}
