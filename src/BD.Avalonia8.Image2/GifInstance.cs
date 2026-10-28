namespace BD.Avalonia8.Image2;

public sealed class GifInstance : IImageInstance, IDisposable
{
    public IterationCount IterationCount { get; set; }

    public bool AutoStart { get; private set; } = true;

    readonly GifDecoder _gifDecoder;
    readonly Stream _sourceStream;
    readonly WriteableBitmap? _targetBitmap;
    TimeSpan _totalTime;
    readonly List<TimeSpan> _frameTimes;
    uint _iterationCount;
    int _currentFrameIndex;
    //readonly List<ulong> _colorTableIdList;

    public CancellationTokenSource CurrentCts { get; }

    internal GifInstance(object newValue) : this(newValue switch
    {
        Stream s => s,
        Uri u => GetStreamFromUri(u),
        string str => GetStreamFromString(str),
        _ => throw new InvalidDataException("Unsupported source object"),
    })
    {
    }

    public GifInstance(string uri) : this(GetStreamFromString(uri))
    {
    }

    public GifInstance(Uri uri) : this(GetStreamFromUri(uri))
    {
    }

    public GifInstance(Stream currentStream, bool leaveOpen = false)
    {
        if (!currentStream.CanSeek)
            throw new InvalidDataException("The provided stream is not seekable.");

        if (!currentStream.CanRead)
            throw new InvalidOperationException("Can't read the stream provided.");

        _sourceStream = leaveOpen
            ? currentStream.SafeCopyToRecyclableMemoryStream("GifInstance.BorrowedSource")
            : currentStream;
        CurrentCts = new CancellationTokenSource();
        try
        {
            _sourceStream.Position = 0;
            _gifDecoder = new GifDecoder(_sourceStream, CurrentCts.Token);
            var pixSize = _gifDecoder.Size;
            _targetBitmap = new WriteableBitmap(pixSize, new AvaVector(96, 96),
                AvaPixelFormat.Bgra8888, AlphaFormat.Opaque);
            GifPixelSize = pixSize;
            _totalTime = TimeSpan.Zero;
            _frameTimes = _gifDecoder.Frames.Select(frame =>
            {
                _totalTime = _totalTime.Add(frame.FrameDelay);
                return _totalTime;
            }).ToList();
            _gifDecoder.RenderFrame(0, _targetBitmap);
        }
        catch
        {
            _targetBitmap?.Dispose();
            _gifDecoder?.Dispose();
            _sourceStream.Dispose();
            CurrentCts.Dispose();
            throw;
        }
    }

    static Stream GetStreamFromString(string str)
    {
        if (!Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out var res))
            throw new InvalidCastException("The string provided can't be converted to URI.");

        return GetStreamFromUri(res);
    }

    static Stream GetStreamFromUri(Uri uri)
    {
        var uriString = uri.OriginalString.Trim();

        if (!uriString.StartsWith("resm") && !uriString.StartsWith("avares"))
            throw new InvalidDataException(
                "The URI provided is not currently supported.");

        // 从资源加载后转换为 RecyclableMemoryStream
        using var originalStream = AssetLoader.Open(uri);
        return originalStream.SafeCopyToRecyclableMemoryStream($"GifInstance.AssetLoader.{Guid.NewGuid()}", true);
    }

    public int GifFrameCount => _frameTimes.Count;

    public PixelSize GifPixelSize { get; }

    /// <inheritdoc/>
    public double Height => GifPixelSize.Height;

    /// <inheritdoc/>
    public double Width => GifPixelSize.Width;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        CurrentCts.Cancel();
        _targetBitmap?.Dispose();
        _gifDecoder.Dispose();
        _sourceStream.Dispose();
        CurrentCts.Dispose();
    }

    /// <inheritdoc/>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc/>
    public AvaBitmap? ProcessFrameTime(TimeSpan stopwatchElapsed)
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

        _iterationCount = unchecked((uint)(elapsedTicks / _totalTime.Ticks));

        return ProcessFrameIndex(currentFrame);
    }

    public WriteableBitmap ProcessFrameIndex(int frameIndex)
    {
        _gifDecoder.RenderFrame(frameIndex, _targetBitmap.ThrowIsNull());
        _currentFrameIndex = frameIndex;

        return _targetBitmap;
    }

    /// <inheritdoc/>
    public AvaSize GetSize(double scaling) => GifPixelSize.ToSize(scaling);
}