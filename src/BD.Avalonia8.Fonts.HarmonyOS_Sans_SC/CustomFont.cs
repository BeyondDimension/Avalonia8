namespace BD.Avalonia8.Fonts;

/// <summary>
/// 自定义字体
/// </summary>
public static class CustomFont
{
    /// <summary>
    /// 自定义字体的 Avalonia Res Uri
    /// </summary>
    public const string Name = "avares://BD.Avalonia8.Fonts.HarmonyOS_Sans_SC/HarmonyOS_Sans_SC_Regular.ttf#HarmonyOS Sans SC";

    static readonly Lazy<AvaFontFamily> mInstance = new(() => new(Name), LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// 自定义字体的 <see cref="AvaFontFamily"/>
    /// </summary>
    public static AvaFontFamily Instance => mInstance.Value;
}
