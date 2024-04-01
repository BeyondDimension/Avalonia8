namespace BD.Avalonia8.Image2;

public interface IImageInstance : IDisposable
{
    bool IsDisposed { get; }

    double Height { get; }

    double Width { get; }

    AvaBitmap? ProcessFrameTime(TimeSpan stopwatchElapsed);

    AvaSize GetSize(double scaling);
}
