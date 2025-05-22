using Avalonia;
using Avalonia.Markup.Xaml;
using System.Extensions;
using System.Reflection;

namespace Image2.Sample;

public sealed class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var window = new MainWindow();

        window.Show();
        base.OnFrameworkInitializationCompleted();
    }

    static readonly Lazy<string> mAssemblyName = new(static () =>
    {
        try
        {
            return Assembly.GetExecutingAssembly().GetName().Name.ThrowIsNull();
        }
        catch
        {
        }
        return Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    public static string AssemblyName => mAssemblyName.Value;
}