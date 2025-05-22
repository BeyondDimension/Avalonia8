using Avalonia;
using Avalonia.Media.Imaging;

namespace BD.Avalonia8.Image2;

public interface IImageInstance : IDisposable
{
    bool IsDisposed { get; }

    double Height { get; }

    double Width { get; }

    Bitmap? ProcessFrameTime(TimeSpan stopwatchElapsed);

    Size GetSize(double scaling);
}
