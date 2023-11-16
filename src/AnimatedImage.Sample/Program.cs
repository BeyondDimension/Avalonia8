using AnimatedImage.Sample;

Ioc.ConfigureServices(static s =>
{
    s.AddLogging();
    s.TryAddHttpPlatformHelper();
    s.AddFusilladeHttpClientFactory();
    s.AddSingleton<IImageHttpClientService, ImageHttpClientServiceImpl>();
});

static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();

BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);