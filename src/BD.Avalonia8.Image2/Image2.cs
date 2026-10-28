using Logger = Avalonia.Logging.Logger;
using Microsoft.IO;

namespace BD.Avalonia8.Image2;

/// <summary>
/// 动图控件，支持 Gif/Apng
/// </summary>
public sealed partial class Image2 : Control, IDisposable
{
    /// <summary>
    /// 内存流管理器，用于复用内存以减少GC压力
    /// </summary>
    internal static readonly RecyclableMemoryStreamManager MemoryStreamManager = new(
        new RecyclableMemoryStreamManager.Options
        {
            // 增加块大小以适应图像数据
            BlockSize = 131072, // 128KB
            LargeBufferMultiple = 1048576, // 1MB
            MaximumBufferSize = 134217728, // 128MB

            // 内存管理选项
            AggressiveBufferReturn = true,

            // 池大小限制
            MaximumSmallPoolFreeBytes = 8388608, // 8MB
            MaximumLargePoolFreeBytes = 134217728 // 128MB
        });

    public static readonly StyledProperty<object> SourceProperty =
        AvaloniaProperty.Register<Image2, object>(nameof(Source));

    public static readonly StyledProperty<object> FallbackSourceProperty =
        AvaloniaProperty.Register<Image2, object>(nameof(FallbackSource));

    //public static readonly StyledProperty<IterationCount> IterationCountProperty = AvaloniaProperty.Register<Image2, IterationCount>(nameof(IterationCount));

    public static readonly StyledProperty<bool> IsFailedProperty =
        AvaloniaProperty.Register<Image2, bool>(nameof(IsFailed), true);

    public static readonly StyledProperty<bool> AutoStartProperty =
        AvaloniaProperty.Register<Image2, bool>(nameof(AutoStart), true);

    public static readonly StyledProperty<int> DecodeWidthProperty =
        AvaloniaProperty.Register<Image2, int>(nameof(DecodeWidth));

    public static readonly StyledProperty<int> DecodeHeightProperty =
        AvaloniaProperty.Register<Image2, int>(nameof(DecodeHeight));

    public static readonly StyledProperty<bool> EnableCacheProperty =
        AvaloniaProperty.Register<Image2, bool>(nameof(EnableCache), true);

    public static readonly StyledProperty<bool> EnableCancelTokenProperty =
        AvaloniaProperty.Register<Image2, bool>(nameof(EnableCancelToken), true);

    public static readonly StyledProperty<bool> KeepSourceStreamOpenProperty =
        AvaloniaProperty.Register<Image2, bool>(nameof(KeepSourceStreamOpen));

    public static readonly StyledProperty<StretchDirection> StretchDirectionProperty =
        AvaloniaProperty.Register<Image2, StretchDirection>(nameof(StretchDirection), StretchDirection.Both);

    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<Image2, Stretch>(nameof(Stretch), Stretch.UniformToFill);

    IImageInstance? gifInstance;
    CompositionCustomVisual? _customVisual;
    AvaBitmap? backingRTB;
    CancellationTokenSource? _tokenSource;
    int _loadVersion;
    bool _isAttached;
    bool _animationInVisual;
    bool _ownsBitmap;
    bool disposedValue;

    [Content]
    public object Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public object FallbackSource
    {
        get => GetValue(FallbackSourceProperty);
        set => SetValue(FallbackSourceProperty, value);
    }

    //public IterationCount IterationCount
    //{
    //    get => GetValue(IterationCountProperty);
    //    set => SetValue(IterationCountProperty, value);
    //}

    public bool AutoStart
    {
        get => GetValue(AutoStartProperty);
        set => SetValue(AutoStartProperty, value);
    }

    public bool IsFailed
    {
        get => GetValue(IsFailedProperty);
        set => SetValue(IsFailedProperty, value);
    }

    public bool EnableCache
    {
        get => GetValue(EnableCacheProperty);
        set => SetValue(EnableCacheProperty, value);
    }

    public bool EnableCancelToken
    {
        get => GetValue(EnableCancelTokenProperty);
        set => SetValue(EnableCancelTokenProperty, value);
    }

    /// <summary>
    /// Borrow stream sources without closing them or changing their seek position.
    /// Otherwise ownership transfers to the control. Transferred streams retain one
    /// stopped decoded result while detached because the consumed stream cannot replay.
    /// </summary>
    public bool KeepSourceStreamOpen
    {
        get => GetValue(KeepSourceStreamOpenProperty);
        set => SetValue(KeepSourceStreamOpenProperty, value);
    }

    public int DecodeHeight
    {
        get => GetValue(DecodeHeightProperty);
        set => SetValue(DecodeHeightProperty, value);
    }

    public int DecodeWidth
    {
        get => GetValue(DecodeWidthProperty);
        set => SetValue(DecodeWidthProperty, value);
    }

    public StretchDirection StretchDirection
    {
        get => GetValue(StretchDirectionProperty);
        set => SetValue(StretchDirectionProperty, value);
    }

    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    //static void IterationCountChanged(AvaloniaPropertyChangedEventArgs e)
    //{
    //    if (e.Sender is not Image2)
    //        return;
    //}

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        switch (change.Property.Name)
        {
            case nameof(Source):
            case nameof(FallbackSource):
                ReloadSource();
                break;
            case nameof(IsVisible):
            case nameof(AutoStart):
                UpdateAnimationPlayback();
                break;
            case nameof(Stretch):
            case nameof(StretchDirection):
                InvalidateArrange();
                InvalidateMeasure();
                Update();
                break;
            case nameof(IterationCount):
                //IterationCountChanged(change);
                break;
            case nameof(Bounds):
                Update();
                break;
        }

        base.OnPropertyChanged(change);
    }

    /// <inheritdoc/>
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _isAttached = true;
        if (backingRTB == null && gifInstance == null && _tokenSource == null)
            ReloadSource();
        else
            AttachAnimation();
    }

    /// <inheritdoc/>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isAttached = false;
        _customVisual?.SendHandlerMessage(CustomVisualHandler.StopMessage);

        // A transferred stream cannot be replayed once consumed. Keep its one decoded
        // result (including an in-flight result), but never keep frames for URI sources.
        if (!IsTransferredStream(Source))
        {
            CancelLoad();
            StopAndDispose();
        }

        base.OnDetachedFromVisualTree(e);
    }

    bool IsTransferredStream(object? source) => !KeepSourceStreamOpen && source is Stream;

    void UpdateAnimationPlayback()
    {
        _customVisual?.SendHandlerMessage(_isAttached && IsEffectivelyVisible && AutoStart
            ? CustomVisualHandler.StartMessage
            : CustomVisualHandler.StopMessage);
    }

    void AttachAnimation()
    {
        if (!_isAttached || gifInstance == null)
            return;

        var compositor = ElementComposition.GetElementVisual(this)?.Compositor;
        if (compositor == null)
            return;

        if (_customVisual == null || _customVisual.Compositor != compositor)
        {
            _customVisual?.SendHandlerMessage(CustomVisualHandler.ReleaseMessage);
            _customVisual = compositor.CreateCustomVisual(new CustomVisualHandler());
            ElementComposition.SetElementChildVisual(this, _customVisual);
            _animationInVisual = false;
        }

        if (!_animationInVisual)
        {
            _customVisual.SendHandlerMessage(gifInstance);
            _animationInVisual = true;
        }
        UpdateAnimationPlayback();
        Update();
    }

    /// <inheritdoc/>
    public override void Render(DrawingContext context)
    {
        if (backingRTB is not { } bitmap) return;

        if (IsVisible && Bounds is { Width: > 0, Height: > 0 })
        {
            var viewPort = new AvaRect(Bounds.Size);
            var sourceSize = bitmap.Size;

            var scale = Stretch.CalculateScaling(Bounds.Size, sourceSize, StretchDirection);
            var scaledSize = sourceSize * scale;
            var destRect = viewPort
                .CenterRect(new AvaRect(scaledSize))
                .Intersect(viewPort);

            var sourceRect = new AvaRect(sourceSize)
                .CenterRect(new AvaRect(destRect.Size / scale));

            //var interpolationMode = RenderOptions.GetBitmapInterpolationMode(this);
            context.DrawImage(bitmap, sourceRect, destRect);
        }
    }

    /// <summary>
    /// Measures the control.
    /// </summary>
    /// <param name="availableSize">The available size.</param>
    /// <returns>The desired size of the control.</returns>
    protected override AvaSize MeasureOverride(AvaSize availableSize)
    {
        if (gifInstance != null)
        {
            var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
            return Stretch.CalculateSize(availableSize, gifInstance.GetSize(scaling),
                StretchDirection);
        }
        else if (backingRTB != null)
        {
            return Stretch.CalculateSize(availableSize, backingRTB.Size, StretchDirection);
        }

        return default;
    }

    /// <inheritdoc/>
    protected override AvaSize ArrangeOverride(AvaSize finalSize)
    {
        if (gifInstance != null)
        {
            var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
            var sourceSize = gifInstance.GetSize(scaling);
            return Stretch.CalculateSize(finalSize, sourceSize);
        }
        else if (backingRTB != null)
        {
            var sourceSize = backingRTB.Size;
            return Stretch.CalculateSize(finalSize, sourceSize);
        }

        return default;
    }

    void StopAndDispose()
    {
        if (_ownsBitmap)
            backingRTB?.Dispose();
        backingRTB = null;
        _ownsBitmap = false;
        _customVisual?.SendHandlerMessage(CustomVisualHandler.ReleaseMessage);
        if (gifInstance is { } animation)
        {
            lock (animation)
                animation.Dispose();
        }
        gifInstance = null;
        _animationInVisual = false;
        _customVisual = null;
        ElementComposition.SetElementChildVisual(this, null);
        RefreshImage();
    }

    void RefreshImage()
    {
        InvalidateArrange();
        InvalidateMeasure();
        InvalidateVisual();
        Update();
    }

    void CancelLoad()
    {
        ++_loadVersion;
        _tokenSource?.Cancel();
        _tokenSource = null; // The corresponding load disposes its source in finally.
    }

    async void ReloadSource()
    {
        CancelLoad();
        StopAndDispose();
        IsFailed = true;
        if (disposedValue || (!_isAttached && !IsTransferredStream(Source)))
            return;

        var source = Source;
        var fallback = FallbackSource;
        var version = _loadVersion;
        var keepOpen = KeepSourceStreamOpen;
        var cache = EnableCache;
        var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1d;
        var decodeWidth = DecodeWidth > 0 ? Math.Max(1, (int)Math.Ceiling(DecodeWidth * scaling)) : 0;
        var decodeHeight = DecodeHeight > 0 ? Math.Max(1, (int)Math.Ceiling(DecodeHeight * scaling)) : 0;
        using var cancellation = new CancellationTokenSource();
        _tokenSource = cancellation;
        var token = EnableCancelToken ? cancellation.Token : CancellationToken.None;
        var sourceSubmitted = false;
        try
        {
            if (source is AvaBitmap bitmap)
            {
                backingRTB = bitmap; // The caller owns bitmap sources.
                IsFailed = false;
                RefreshImage();
                return;
            }

            if (fallback != null)
            {
                var result = await Task.Run(() => LoadImageAsync(fallback, keepOpen, cache, decodeWidth, decodeHeight, token));
                if (!PublishImage(result, version, true))
                    return;
            }

            if (source != null)
            {
                sourceSubmitted = true;
                var result = await Task.Run(() => LoadImageAsync(source, keepOpen, cache, decodeWidth, decodeHeight, token));
                PublishImage(result, version, false);
            }
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            if (version == _loadVersion)
                Log.Error(nameof(Image2), ex, "SourceChanged fail");
        }
        finally
        {
            if (!sourceSubmitted && !keepOpen && source is Stream unconsumed)
                unconsumed.Dispose();
            if (ReferenceEquals(_tokenSource, cancellation))
                _tokenSource = null;
        }
    }

    bool PublishImage((AvaBitmap? Bitmap, IImageInstance? Animation) result, int version, bool fallback)
    {
        if (disposedValue || version != _loadVersion)
        {
            result.Bitmap?.Dispose();
            result.Animation?.Dispose();
            return false;
        }
        if (result.Bitmap == null && result.Animation == null)
            return true; // Keep an already displayed fallback on failure.

        StopAndDispose();
        backingRTB = result.Bitmap;
        _ownsBitmap = result.Bitmap != null;
        gifInstance = result.Animation;
        IsFailed = fallback;
        AttachAnimation();
        RefreshImage();
        return true;
    }

    static async Task<(AvaBitmap? Bitmap, IImageInstance? Animation)> LoadImageAsync(
        object source, bool keepOpen, bool cache, int decodeWidth, int decodeHeight, CancellationToken token)
    {
        Stream? stream = await ResolveObjectToStreamCore(source, keepOpen, cache, token).ConfigureAwait(false);
        IImageInstance? animation = null;
        AvaBitmap? bitmap = null;
        try
        {
            token.ThrowIfCancellationRequested();
            if (stream == null)
                return default;

            var format = FileFormat.IsImage(stream, out var imageFormat) ? imageFormat : default;
            stream.Position = 0;
            if (format == ImageFormat.GIF)
            {
                animation = new GifInstance(stream) { IterationCount = IterationCount.Infinite };
                stream = null; // Ownership transferred to the animation.
            }
            else if (format == ImageFormat.PNG)
            {
                var apng = new ApngInstance(stream, false, false);
                animation = apng;
                stream = null;
                if (apng.IsSimplePNG)
                {
                    bitmap = DecodeImage(apng.Stream!, decodeWidth, decodeHeight);
                    apng.Dispose();
                    animation = null;
                }
                else
                    apng.IterationCount = IterationCount.Infinite;
            }
            else
                bitmap = DecodeImage(stream, decodeWidth, decodeHeight);

            token.ThrowIfCancellationRequested();
            return (bitmap, animation);
        }
        catch
        {
            bitmap?.Dispose();
            animation?.Dispose();
            throw;
        }
        finally
        {
            stream?.Dispose();
        }
    }

    static AvaBitmap? DecodeImage(Stream stream, int decodeWidth, int decodeHeight)
    {
        try
        {
            stream.Position = 0;
            if (decodeWidth > 0)
                return AvaBitmap.DecodeToWidth(stream, decodeWidth);
            if (decodeHeight > 0)
                return AvaBitmap.DecodeToHeight(stream, decodeHeight);
            return new AvaBitmap(stream);
        }
        catch (Exception e)
        {
            Logger.Sink?.Log(LogEventLevel.Error, "AnimatedImage DecodeImage ", null, e.ToString());
            return null;
        }
    }

    void Update()
    {
        if (_customVisual is null || gifInstance is null)
            return;

        var dpi = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0d;
        var sourceSize = gifInstance.GetSize(dpi);
        var viewPort = new AvaRect(Bounds.Size);

        var scale = Stretch.CalculateScaling(Bounds.Size, sourceSize, StretchDirection);
        var scaledSize = sourceSize * scale;
        var destRect = viewPort
            .CenterRect(new AvaRect(scaledSize))
            .Intersect(viewPort);

        if (Stretch == Stretch.None)
        {
            _customVisual.Size = new Vector2((float)sourceSize.Width, (float)sourceSize.Height);
        }
        else
        {
            _customVisual.Size = new Vector2((float)destRect.Size.Width, (float)destRect.Size.Height);
        }

        _customVisual.Offset = new Vector3((float)destRect.Position.X, (float)destRect.Position.Y, 0f);
    }

    sealed class CustomVisualHandler : CompositionCustomVisualHandler
    {
        TimeSpan _animationElapsed;
        TimeSpan? _lastServerTime;
        IImageInstance? _currentInstance;
        bool _running;

        public static readonly object StopMessage = new();
        public static readonly object StartMessage = new();
        public static readonly object ReleaseMessage = new();

        public override void OnMessage(object message)
        {
            if (message == StartMessage)
            {
                if (_running)
                    return;
                _running = true;
                _lastServerTime = null;
                RegisterForNextAnimationFrameUpdate();
            }
            else if (message == StopMessage)
            {
                _running = false;
                _lastServerTime = null;
            }
            else if (message == ReleaseMessage)
            {
                _running = false;
                _currentInstance = null;
                Invalidate();
            }
            else if (message is IImageInstance instance)
            {
                _currentInstance = instance;
                _animationElapsed = TimeSpan.Zero;
                _lastServerTime = null;
                Invalidate();
            }
        }

        public override void OnAnimationFrameUpdate()
        {
            if (!_running)
                return;

            Invalidate();

            RegisterForNextAnimationFrameUpdate();
        }

        public override void OnRender(ImmediateDrawingContext drawingContext)
        {
            if (_running)
            {
                var now = CompositionNow;
                if (_lastServerTime.HasValue)
                {
                    var elapsed = now - _lastServerTime.Value;
                    _animationElapsed += elapsed;
                }

                _lastServerTime = now;
            }

            try
            {
                if (_currentInstance is not { } instance)
                    return;
                lock (instance)
                {
                    if (instance.IsDisposed)
                        return;
                    var bitmap = instance.ProcessFrameTime(_animationElapsed);
                    if (bitmap != null)
                        RenderBitmap(bitmap, drawingContext);
                }
            }
            catch (Exception e)
            {
                Logger.Sink?.Log(LogEventLevel.Error, "Image2 Renderer ", this, e.ToString());
            }
        }

        private void RenderBitmap(AvaBitmap bitmap, ImmediateDrawingContext drawingContext)
        {
            try
            {
                // 正常渲染APNG和GIF
                if (_currentInstance is ApngInstance)
                {
                    var ts = new AvaRect(_currentInstance.GetSize(1));
                    var rect = GetRenderBounds();
                    drawingContext.DrawBitmap(bitmap, ts, rect);
                }
                else
                {
                    drawingContext.DrawBitmap(bitmap, new AvaRect(_currentInstance.GetSize(1)),
                        GetRenderBounds());
                }
            }
            catch (Exception renderEx)
            {
                Logger.Sink?.Log(LogEventLevel.Warning, "Image2 Renderer DrawBitmap", this,
                    renderEx.ToString());
            }
        }
    }

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            disposedValue = true;
            if (disposing)
            {
                CancelLoad();
                StopAndDispose();
                ClearValue(SourceProperty);
                ClearValue(FallbackSourceProperty);
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null

        }
    }

    void IDisposable.Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}