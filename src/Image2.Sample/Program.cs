using Avalonia;
using Avalonia.Media;
using BD.Avalonia8.Fonts;
using BD.Common8.Http.ClientFactory.Extensions;
using BD.Common8.Http.ClientFactory.Services;
using BD.Common8.Http.ClientFactory.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

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
            DefaultFamilyName = HarmonyOS_Sans_SC.Name,
            FontFallbacks =
            [
                new FontFallback { FontFamily = HarmonyOS_Sans_SC.Instance },
                new FontFallback { FontFamily = FontFamily.Default },
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
