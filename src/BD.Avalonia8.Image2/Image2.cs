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

            // 恢复动画实例的播放
            if (gifInstance != null)
            {
                _customVisual?.SendHandlerMessage(gifInstance);
            }

            // 仅在实际可见和启动自动播放时才启动动画
            if (IsEffectivelyVisible && AutoStart)
            {
                _customVisual?.SendHandlerMessage(CustomVisualHandler.StartMessage);
            }

            Update();
        }

        // 注册可见性变化事件
        this.GetPropertyChangedObservable(IsVisibleProperty)
            .Subscribe(OnVisibilityChanged);

        base.OnAttachedToVisualTree(e);
    }

    /// <inheritdoc/>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        // 当控件从视觉树分离时暂停动画并释放不必要的资源

        if (_customVisual != null)
        {
            _customVisual.SendHandlerMessage(CustomVisualHandler.StopMessage);
        }

        base.OnDetachedFromVisualTree(e);
    }

    private void OnVisibilityChanged(AvaloniaPropertyChangedEventArgs e)
    {
        // 当控件可见性变化时相应地处理动画状态
        if (e.NewValue is bool isVisible)
        {
            if (isVisible && AutoStart)
            {
                // 控件变为可见时恢复动画
                _customVisual?.SendHandlerMessage(CustomVisualHandler.StartMessage);
            }
            else
            {
                // 控件变为不可见时暂停动画以节省资源
                _customVisual?.SendHandlerMessage(CustomVisualHandler.StopMessage);
            }
        }
    }

    private void OnEffectiveVisibilityChanged(AvaloniaPropertyChangedEventArgs e)
    {
        // 当控件有效可见性变化时处理动画状态
        if (e.NewValue is bool isEffectivelyVisible)
        {
            if (isEffectivelyVisible && AutoStart)
            {
                // 控件变为有效可见时恢复动画
                _customVisual?.SendHandlerMessage(CustomVisualHandler.StartMessage);
            }
            else
            {
                // 控件变为有效不可见时暂停动画以节省资源
                _customVisual?.SendHandlerMessage(CustomVisualHandler.StopMessage);
            }
        }
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
                    backingRTB = DecodeImage(apngInstance!.Stream!);
                    apngInstance.Dispose();
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
        TimeSpan _lastFrameRenderTime;
        IImageInstance? _currentInstance;
        bool _running;
        bool _needsUpdate = false;
        AvaBitmap? _lastRenderedBitmap;

        // 帧率控制
        readonly TimeSpan _minFrameInterval = TimeSpan.FromMilliseconds(16); // 约60fps

        public static readonly object StopMessage = new();
        public static readonly object StartMessage = new();

        public override void OnMessage(object message)
        {
            if (message == StartMessage)
            {
                _running = true;
                _lastServerTime = null;
                _needsUpdate = true;
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
                _needsUpdate = true;
            }
        }

        public override void OnAnimationFrameUpdate()
        {
            if (!_running)
                return;

            // 计算时间差，确定是否需要更新帧
            var now = CompositionNow;
            var timeSinceLastFrame = _lastServerTime.HasValue ? now - _lastServerTime.Value : TimeSpan.Zero;

            // 只有当时间间隔超过最小帧间隔或者有强制更新标记时才更新
            if (timeSinceLastFrame >= _minFrameInterval || _needsUpdate)
            {
                _needsUpdate = false;
                Invalidate();
            }

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
                if (_currentInstance is null || _currentInstance.IsDisposed)
                    return;

                // 计算距离上次渲染帧的时间
                var timeSinceLastRender = CompositionNow - _lastFrameRenderTime;

                // 如果时间间隔太短且已有渲染过的位图，直接使用上次的位图避免频繁处理
                if (timeSinceLastRender < _minFrameInterval && _lastRenderedBitmap != null)
                {
                    RenderBitmap(_lastRenderedBitmap, drawingContext);
                    return;
                }

                // 处理新帧
                var bitmap = _currentInstance.ProcessFrameTime(_animationElapsed);
                if (bitmap is not null)
                {
                    _lastFrameRenderTime = CompositionNow;
                    _lastRenderedBitmap = bitmap;
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