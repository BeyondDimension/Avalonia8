using BD.Avalonia8.Fonts;

namespace Image2.Sample;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Ioc.ConfigureServices(static s =>
        {
            s.AddLogging();
            s.AddSingleton<IHttpPlatformHelperService, HttpPlatformHelperServiceImpl>();
            s.AddFusilladeHttpClientFactory();
            s.AddSingleton<IImageHttpClientService, ImageHttpClientServiceImpl>();
        });
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static AppBuilder BuildAvaloniaApp()
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
        var builder = AppBuilder.Configure<App>()
                    .With(options)
                    .UsePlatformDetect()
                    .LogToTrace();
        return builder;
    }

    sealed class HttpPlatformHelperServiceImpl : HttpPlatformHelperService
    {
        public override string UserAgent => DefaultUserAgent;
    }
}
