using Logger = Avalonia.Logging.Logger;

namespace BD.Avalonia8.Image2;

/// <summary>
/// 动图控件，支持 Gif/Apng
/// </summary>
public sealed partial class Image2 : Control, IDisposable
{
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

    public static readonly StyledProperty<StretchDirection> StretchDirectionProperty =
        AvaloniaProperty.Register<Image2, StretchDirection>(nameof(StretchDirection), StretchDirection.Both);

    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<Image2, Stretch>(nameof(Stretch), Stretch.UniformToFill);

    IImageInstance? gifInstance;
    CompositionCustomVisual? _customVisual;
    AvaBitmap? backingRTB;
    ImageFormat imageFormat;
    bool isSimplePNG;
    CancellationTokenSource _tokenSource = new();
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
                SourceChanged(change);
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
    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (FallbackSource != null && backingRTB == null)
        {
            if (EnableCancelToken)
            {
                _tokenSource.Cancel();
                _tokenSource = new CancellationTokenSource();
            }

            var value = await ResolveObjectToStream(FallbackSource, this, _tokenSource.Token);
            if (value != null)
            {
                backingRTB = DecodeImage(value);
                value.Dispose();
            }
        }

        if (imageFormat == ImageFormat.GIF || !isSimplePNG)
        {
            var compositor = ElementComposition.GetElementVisual(this)?.Compositor;
            if (compositor == null || _customVisual?.Compositor == compositor)
                return;
            _customVisual = compositor.CreateCustomVisual(new CustomVisualHandler());
            ElementComposition.SetElementChildVisual(this, _customVisual);
            _customVisual.SendHandlerMessage(CustomVisualHandler.StartMessage);

            if (gifInstance is not null)
            {
                _customVisual?.SendHandlerMessage(gifInstance);
            }

            Update();
        }

        base.OnAttachedToVisualTree(e);
    }

    /// <inheritdoc/>
    public override void Render(DrawingContext context)
    {
        if (backingRTB is not AvaBitmap bitmap) return;

        if (bitmap is not null && IsVisible && Bounds.Width > 0 && Bounds.Height > 0)
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
            var scaling = this.GetVisualRoot()?.RenderScaling ?? 1.0;
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
            var scaling = this.GetVisualRoot()?.RenderScaling ?? 1.0;
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
        backingRTB?.Dispose();
        gifInstance?.Dispose();
        _customVisual = null;
    }

    void SetNullValue()
    {
        InvalidateArrange();
        InvalidateMeasure();
        Update();
    }

    async void SourceChanged(AvaloniaPropertyChangedEventArgs e)
    {
        try
        {
            await SourceChangedAsync(e);
        }
        catch (Exception ex)
        {
            Log.Error(nameof(Image2), ex, "SourceChanged fail");
            SetNullValue();
        }
    }

    async Task SourceChangedAsync(AvaloniaPropertyChangedEventArgs e)
    {
        if (EnableCancelToken)
        {
            _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
        }

        gifInstance?.Dispose();
        gifInstance = null;
        backingRTB?.Dispose();
        backingRTB = null;
        IsFailed = true;

        if (e.NewValue == null && FallbackSource == null)
        {
            return;
        }

        Stream? value;
        if (e.NewValue is AvaBitmap bitmap)
        {
            IsFailed = false;
            backingRTB = bitmap;
            return;
        }
        else
        {
            if (FallbackSource != null)
            {
                value = await ResolveObjectToStream(FallbackSource, this, _tokenSource.Token);
                if (value != null)
                {
                    IsFailed = true;
                    backingRTB = DecodeImage(value);
                    value.Dispose();
                }
            }

            value = await ResolveObjectToStream(e.NewValue, this, _tokenSource.Token);
        }

        if (value == null)
        {
            SetNullValue();
            return;
        }

        IsFailed = false;
        imageFormat = FileFormat.IsImage(value, out var imageFormat_) ? imageFormat_ : default;

        if (imageFormat == ImageFormat.GIF)
        {
            try
            {
                var gifInstance = new GifInstance(value) { IterationCount = IterationCount.Infinite, };
                if (gifInstance.GifPixelSize.Width < 1 || gifInstance.GifPixelSize.Height < 1)
                {
                    return;
                }

                this.gifInstance = gifInstance;
                _customVisual?.SendHandlerMessage(gifInstance);
            }
            catch
            {
            }
        }
        else if (imageFormat == ImageFormat.PNG)
        {
            try
            {
                // 检查是否是动画PNG
                var apngInstance = new ApngInstance(value);
                if (apngInstance.IsSimplePNG)
                {
                    isSimplePNG = apngInstance.IsSimplePNG;
                    apngInstance.Dispose();

                    // 重置流位置
                    value.Position = 0;
                    backingRTB = DecodeImage(value);
                }
                else
                {
                    apngInstance.IterationCount = IterationCount.Infinite;
                    if (apngInstance.ApngPixelSize.Width < 1 || apngInstance.ApngPixelSize.Height < 1)
                    {
                        apngInstance.Dispose();
                        return;
                    }

                    // 如果曾经有旧的实例，先释放它
                    if (gifInstance != null && gifInstance != apngInstance)
                    {
                        gifInstance.Dispose();
                    }

                    // 设置新的实例
                    gifInstance = apngInstance;
                    _customVisual?.SendHandlerMessage(gifInstance);
                }
            }
            catch (Exception ex)
            {
                Log.Error(nameof(Image2), ex, "ApngInstance fail", ex.StackTrace);
                SetNullValue();
                return;
            }
        }
        else
        {
            backingRTB = DecodeImage(value);
        }

        InvalidateArrange();
        InvalidateMeasure();
        Update();
    }

    AvaBitmap? DecodeImage(Stream stream)
    {
        try
        {
            if (stream == null || stream.CanRead == false || stream.Length == 0)
            {
                return null;
            }

            stream.Position = 0;

            #region 😣 因为 Bitmap.DecodeTo 有一定内存泄露问题暂时注释

            //var interpolationMode = RenderOptions.GetBitmapInterpolationMode(this);
            //if (DecodeWidth > 0)
            //{
            //    return Bitmap.DecodeToWidth(stream, DecodeWidth, interpolationMode);
            //}
            //else if (DecodeHeight > 0)
            //{
            //    return Bitmap.DecodeToHeight(stream, DecodeHeight, interpolationMode);
            //}

            #endregion

            //https://github.com/mono/SkiaSharp/issues/1551
            return new AvaBitmap(stream);
        }
        catch (Exception e)
        {
            Logger.Sink?.Log(LogEventLevel.Error, "AnimatedImage DecodeImage ", this, e.ToString());
            // 为了让程序不闪退无视错误
            return null;
        }
    }

    void Update()
    {
        if (_customVisual is null || gifInstance is null)
            return;

        var dpi = this.GetVisualRoot()?.RenderScaling ?? 1.0d;
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

        public override void OnMessage(object message)
        {
            if (message == StartMessage)
            {
                _running = true;
                _lastServerTime = null;
                RegisterForNextAnimationFrameUpdate();
            }
            else if (message == StopMessage)
            {
                _running = false;
            }
            else if (message is IImageInstance instance)
            {
                _currentInstance?.Dispose();
                _currentInstance = instance;
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
                if (_lastServerTime.HasValue) _animationElapsed += CompositionNow - _lastServerTime.Value;
                _lastServerTime = CompositionNow;
            }

            try
            {
                if (_currentInstance is null || _currentInstance.IsDisposed)
                    return;

                var bitmap = _currentInstance.ProcessFrameTime(_animationElapsed);
                if (bitmap is not null)
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
            catch (Exception e)
            {
                Logger.Sink?.Log(LogEventLevel.Error, "Image2 Renderer ", this, e.ToString());
            }
        }
    }

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                StopAndDispose();
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null

            disposedValue = true;
        }
    }

    void IDisposable.Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}