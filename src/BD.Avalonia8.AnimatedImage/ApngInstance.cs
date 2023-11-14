namespace BD.Avalonia8.AnimatedImage;

#pragma warning disable SA1600 // Elements should be documented

public sealed class ApngInstance : IImageInstance, IDisposable
{
    public Stream? Stream { get; private set; }

    public IterationCount IterationCount { get; set; }

    public bool AutoStart { get; private set; } = true;

    public PixelSize ApngPixelSize { get; private set; }

    public bool IsSimplePNG { get; private set; }

    public bool IsDisposed => disposedValue;

    public double Height => ApngPixelSize.Height;

    public double Width => ApngPixelSize.Width;

    public CancellationTokenSource CurrentCts { get; } = new();

    APNG _apng;
    WriteableBitmap? _targetBitmap;
    TimeSpan _totalTime;
    readonly List<TimeSpan> _frameTimes;
    public DisposeOps _disposeOps;
    public AvaPoint _targetOffset;
    int _currentFrameIndex;
    uint _iterationCount;
    public bool _hasNewFrame;
    bool disposedValue;

    public ApngInstance(Stream stream)
    {
        Stream = stream;

        if (!stream.CanSeek)
            throw new InvalidDataException("The provided stream is not seekable.");

        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read the stream provided.");

        stream.Seek(0, SeekOrigin.Begin);

        _apng = new APNG(Stream);
        IsSimplePNG = _apng.IsSimplePNG;

        if (IsSimplePNG)
        {
            _targetBitmap = WriteableBitmap.Decode(Stream);
            _frameTimes = [];
            return;
        }
        else if (_apng.Frames.Length != 0)
        {
            var firstFrame = _apng.Frames.First();
            var pixSize = new PixelSize(firstFrame.IHDRChunk.ThrowIsNull().Width, firstFrame.IHDRChunk.Height);

            _targetBitmap = WriteableBitmap.Decode(_apng.Frames[0].GetStream());
            ApngPixelSize = pixSize;
        }

        _totalTime = TimeSpan.Zero;

        _frameTimes = _apng.Frames.Select(frame =>
        {
            _totalTime = _totalTime.Add(frame.FrameDelay);
            return _totalTime;
        }).ToList();
    }

    public WriteableBitmap? GetBitmap() => _targetBitmap;

    public void AutoStartChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool newVal)
            AutoStart = newVal;
    }

    public AvaSize GetSize(double scaling) => ApngPixelSize.ToSize(scaling);

    public Bitmap? ProcessFrameTime(TimeSpan stopwatchElapsed)
    {
        if (!IterationCount.IsInfinite && _iterationCount > IterationCount.Value)
            return null;

        if (CurrentCts.IsCancellationRequested || _targetBitmap is null)
            return null;

        var elapsedTicks = stopwatchElapsed.Ticks;
        var timeModulus = TimeSpan.FromTicks(elapsedTicks % _totalTime.Ticks);
        var targetFrame = _frameTimes.FirstOrDefault(x => timeModulus < x);
        var currentFrame = _frameTimes.IndexOf(targetFrame);
        if (currentFrame == -1) currentFrame = 0;

        if (_currentFrameIndex == currentFrame)
            return _targetBitmap;

        _iterationCount = (uint)(elapsedTicks / _totalTime.Ticks);

        return ProcessFrameIndex(currentFrame);
    }

    internal Bitmap ProcessFrameIndex(int frameIndex)
    {
        var currentFrame = _apng.Frames[frameIndex];
        _hasNewFrame = frameIndex == 0 || currentFrame.fcTLChunk.ThrowIsNull().BlendOp == BlendOps.APNGBlendOpSource;
        _disposeOps = currentFrame.fcTLChunk.ThrowIsNull().DisposeOp;
        _targetOffset = new(currentFrame.fcTLChunk.XOffset, currentFrame.fcTLChunk.YOffset);

        _targetBitmap = WriteableBitmap.Decode(currentFrame.GetStream());

        _currentFrameIndex = frameIndex;

        return _targetBitmap;
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                _targetBitmap?.Dispose();
                CurrentCts.Cancel();
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
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
