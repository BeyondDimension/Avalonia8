namespace BD.Avalonia8.AnimatedImage;

#pragma warning disable SA1600 // Elements should be documented

public interface IImageInstance : IDisposable
{
    bool IsDisposed { get; }

    double Height { get; }

    double Width { get; }

    Bitmap? ProcessFrameTime(TimeSpan stopwatchElapsed);

    AvaSize GetSize(double scaling);
}
