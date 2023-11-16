namespace BD.Avalonia8.UnitTest.Apps;

#pragma warning disable SA1600 // Elements should be documented

public sealed class App : AvaApplication
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new Window();
        base.OnFrameworkInitializationCompleted();
    }

    public static IDisposable Start()
    {
        var starter = new AppStarter();

        var th = new Thread(starter.Start);
        th.Start();

        starter.GetStartTask().GetAwaiter().GetResult();

        return starter;
    }
}

sealed class AppStarter : IDisposable
{
    ClassicDesktopStyleApplicationLifetime? _lifetime;
    bool disposedValue;
    readonly TaskCompletionSource tcs = new();

    public Task GetStartTask() => tcs.Task;

    public void Start()
    {
        var builder = AppBuilder.Configure<App>();
        builder.UsePlatformDetect();

        _lifetime = new ClassicDesktopStyleApplicationLifetime()
        {
            Args = [],
            ShutdownMode = ShutdownMode.OnMainWindowClose,
        };
        builder.SetupWithLifetime(_lifetime);

        tcs.SetResult();

        while (true)
            Dispatcher.UIThread.RunJobs();
    }

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                try
                {
                    _lifetime?.Shutdown();
                }
                finally
                {
                    _lifetime?.Dispose();
                }
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            _lifetime = null;
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
