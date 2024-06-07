namespace Image2.Sample;

public sealed class App : AvaApplication
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
#pragma warning disable CS8603 // 可能返回 null 引用。
            return Assembly.GetExecutingAssembly().GetName().Name.ThrowIsNull();
#pragma warning restore CS8603 // 可能返回 null 引用。
        }
        catch
        {
        }
        return Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    public static string AssemblyName => mAssemblyName.Value;
}