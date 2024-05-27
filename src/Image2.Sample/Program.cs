using Image2.Sample;
using BD.Avalonia8.Fonts;

Ioc.ConfigureServices(static s =>
{
    s.AddLogging();
    s.TryAddHttpPlatformHelper();
    s.AddFusilladeHttpClientFactory();
    s.AddSingleton<IImageHttpClientService, ImageHttpClientServiceImpl>();
});

static AppBuilder BuildAvaloniaApp()
{
    FontManagerOptions options = new()
    {
        DefaultFamilyName = CustomFont.Name,
        FontFallbacks =
        [
            new FontFallback { FontFamily = CustomFont.Instance },
            new FontFallback { FontFamily = AvaFontFamily.Default },
        ],
    };
    return AppBuilder.Configure<App>()
                .With(options)
                .UsePlatformDetect()
                .LogToTrace();
}

BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);