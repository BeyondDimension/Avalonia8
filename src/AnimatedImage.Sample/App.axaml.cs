namespace AnimatedImage.Sample;

#pragma warning disable SA1600 // Elements should be documented

public sealed class App : AvaApplication
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var window = new MainWindow
        {
            DataContext = new MainWindowViewModel(),
        };

        window.Show();
        base.OnFrameworkInitializationCompleted();
    }
}