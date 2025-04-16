using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Buffers;

namespace BD.Avalonia8.Image2;

public sealed class ApngInstance : IImageInstance, IDisposable
{
    public Stream? Stream { get; private set; }

    public IterationCount IterationCount { get; set; }

    public bool AutoStart { get; private set; } = true;

    public PixelSize ApngPixelSize { get; private set; }

    public bool IsSimplePNG { get; private set; }

    /// <inheritdoc/>
    public bool IsDisposed => disposedValue;

    /// <inheritdoc/>
    public double Height => ApngPixelSize.Height;

    /// <inheritdoc/>
    public double Width => ApngPixelSize.Width;

    public CancellationTokenSource CurrentCts { get; } = new();

    private APNG _apng;
    private WriteableBitmap? _targetBitmap;
    private WriteableBitmap? _compositeBitmap; // 用于合成帧的位图
    private WriteableBitmap? _prevCompositeBitmap; // 保存上一帧合成后的结果
    private TimeSpan _totalTime;
    private readonly List<TimeSpan> _frameTimes;
    private DisposeOps _prevDisposeOp = DisposeOps.APNGDisposeOpNone;
    private AvaRect _prevFrameRect; // 上一帧的区域
    public AvaPoint _targetOffset;
    private int _currentFrameIndex = -1;
    private uint _iterationCount;
    public bool _hasNewFrame;
    private bool disposedValue;

    // 帧缓存，用于存储已经解码的帧，避免重复解码
    private readonly Dictionary<int, WriteableBitmap> _frameCache = new();

    // 缓存容量控制
    private const int MAX_CACHE_SIZE = 8;

    // 是否正在前台显示（性能优化用）
    private bool _isVisible = true;

    public ApngInstance(Stream stream)
    {
        Stream = stream;

        if (!stream.CanSeek)
            throw new InvalidDataException("The provided stream is not seekable.");

        if (!stream.CanRead)
            throw new InvalidOperationException("Can't read the stream provided.");

        stream.Seek(0, SeekOrigin.Begin);

        try
        {
            _apng = new APNG(Stream);
            IsSimplePNG = _apng.IsSimplePNG;
        }
        finally
        {
            Stream.Position = 0;
        }

        if (IsSimplePNG)
        {
            _targetBitmap = WriteableBitmap.Decode(Stream);
            _frameTimes = [];
            return;
        }
        else if (_apng.Frames.Length != 0)
        {
            var firstFrame = _apng.Frames.First();
            var iHDRChunk = firstFrame.IHDRChunk.ThrowIsNull();
            ApngPixelSize = new PixelSize(iHDRChunk.Width, iHDRChunk.Height);

            // 初始化合成帧和目标帧
            _compositeBitmap = new WriteableBitmap(
                new PixelSize(ApngPixelSize.Width, ApngPixelSize.Height),
                new AvaVector(96, 96),
                AvaPixelFormat.Bgra8888,
                AlphaFormat.Premul);

            _prevCompositeBitmap = new WriteableBitmap(
                new PixelSize(ApngPixelSize.Width, ApngPixelSize.Height),
                new AvaVector(96, 96),
                AvaPixelFormat.Bgra8888,
                AlphaFormat.Premul);

            // 用透明色填充初始帧
            using (var context = _compositeBitmap.Lock())
            {
                unsafe
                {
                    var pixelData = (byte*)context.Address;
                    var totalBytes = ApngPixelSize.Width * ApngPixelSize.Height * 4;
                    for (int i = 0; i < totalBytes; i += 4)
                    {
                        pixelData[i] = 0; // B
                        pixelData[i + 1] = 0; // G
                        pixelData[i + 2] = 0; // R
                        pixelData[i + 3] = 0; // A (透明)
                    }
                }
            }

            _targetBitmap = _compositeBitmap;
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

    /// <inheritdoc/>
    public AvaSize GetSize(double scaling) => ApngPixelSize.ToSize(scaling);

    /// <inheritdoc/>
    public AvaBitmap? ProcessFrameTime(TimeSpan stopwatchElapsed)
    {
        if (!IterationCount.IsInfinite && _iterationCount > IterationCount.Value)
            return null;

        if (CurrentCts.IsCancellationRequested || _compositeBitmap is null || _frameTimes.Count == 0)
            return null;

        var elapsedTicks = stopwatchElapsed.Ticks;
        var timeModulus = TimeSpan.FromTicks(elapsedTicks % _totalTime.Ticks);
        var targetFrame = _frameTimes.FirstOrDefault(x => timeModulus < x);
        var currentFrame = _frameTimes.IndexOf(targetFrame);
        if (currentFrame == -1) currentFrame = 0;

        if (_currentFrameIndex == currentFrame)
            return _compositeBitmap;

        // 如果帧序列不连续或重新开始，需要处理中间帧
        if (_currentFrameIndex != -1 &&
            ((currentFrame != (_currentFrameIndex + 1) % _apng.Frames.Length) ||
             (currentFrame == 0 && _currentFrameIndex != _apng.Frames.Length - 1)))
        {
            // 帧跳跃或循环开始，需要重置合成状态
            ResetComposite();

            // 如果不是从第一帧开始，需要渲染所有前面的帧
            if (currentFrame > 0)
            {
                for (int i = 0; i < currentFrame; i++)
                {
                    ProcessFrameIndex(i, true);
                }
            }
        }

        _iterationCount = unchecked((uint)(elapsedTicks / _totalTime.Ticks));

        return ProcessFrameIndex(currentFrame, false);
    }

    private void ResetComposite()
    {
        if (_compositeBitmap != null)
        {
            using (var context = _compositeBitmap.Lock())
            {
                unsafe
                {
                    var pixelData = (byte*)context.Address;
                    var totalBytes = ApngPixelSize.Width * ApngPixelSize.Height * 4;
                    for (int i = 0; i < totalBytes; i += 4)
                    {
                        pixelData[i] = 0; // B
                        pixelData[i + 1] = 0; // G
                        pixelData[i + 2] = 0; // R
                        pixelData[i + 3] = 0; // A (透明)
                    }
                }
            }
        }

        _prevDisposeOp = DisposeOps.APNGDisposeOpNone;
        _prevFrameRect = new AvaRect();
    }

    internal AvaBitmap ProcessFrameIndex(int frameIndex, bool silent = false)
    {
        try
        {
            // 如果控件不可见且不是静默模式，可以跳过复杂处理，直接返回上一帧
            if (!_isVisible && !silent && _compositeBitmap != null)
            {
                _currentFrameIndex = frameIndex;
                return _compositeBitmap;
            }

            var currentFrame = _apng.Frames[frameIndex];
            var fcTLChunk = currentFrame.fcTLChunk.ThrowIsNull();
            var blendOp = fcTLChunk.BlendOp;
            var disposeOp = fcTLChunk.DisposeOp;

            _targetOffset = new AvaPoint(fcTLChunk.XOffset, fcTLChunk.YOffset);
            var frameWidth = fcTLChunk.Width;
            var frameHeight = fcTLChunk.Height;

            var currentFrameRect = new AvaRect(
                fcTLChunk.XOffset,
                fcTLChunk.YOffset,
                frameWidth,
                frameHeight);

            // 应用前一帧的处置操作
            ApplyDisposeOperation();

            // 尝试从缓存获取帧位图
            WriteableBitmap frameBitmap;
            if (!_frameCache.TryGetValue(frameIndex, out frameBitmap!))
            {
                // 如果缓存中没有，解码当前帧
                var frameStream = currentFrame.GetStream();
                frameBitmap = WriteableBitmap.Decode(frameStream);

                // 添加到缓存，如果缓存已满，移除最早添加的项
                if (_frameCache.Count >= MAX_CACHE_SIZE)
                {
                    var oldestKey = _frameCache.Keys.First();
                    var oldBitmap = _frameCache[oldestKey];
                    oldBitmap.Dispose();
                    _frameCache.Remove(oldestKey);
                }

                // 克隆位图添加到缓存，避免后续被修改
                var cacheBitmap = new WriteableBitmap(frameBitmap.PixelSize, frameBitmap.Dpi, frameBitmap.Format,
                    frameBitmap.AlphaFormat);
                using (var targetContext = cacheBitmap.Lock())
                using (var sourceContext = frameBitmap.Lock())
                {
                    unsafe
                    {
                        var source = (byte*)sourceContext.Address;
                        var target = (byte*)targetContext.Address;
                        var size = frameBitmap.PixelSize.Width * frameBitmap.PixelSize.Height * 4;
                        Buffer.MemoryCopy(source, target, size, size);
                    }
                }

                _frameCache[frameIndex] = cacheBitmap;
            }

            // 合成当前帧到合成位图
            ComposeFrame(frameBitmap, currentFrameRect, blendOp);

            // 保存当前帧的处置操作和区域，用于下一帧
            _prevDisposeOp = disposeOp;
            _prevFrameRect = currentFrameRect;

            // 保存合成结果，用于处置操作
            SwapCompositeBitmaps();

            // 更新当前帧索引
            _currentFrameIndex = frameIndex;

            // 如果不是来自缓存的位图，需要释放
            if (!_frameCache.ContainsValue(frameBitmap))
            {
                frameBitmap.Dispose();
            }

            return _compositeBitmap!;
        }
        catch (Exception ex)
        {
            Avalonia.Logging.Logger.Sink?.Log(
                Avalonia.Logging.LogEventLevel.Error,
                "ApngInstance",
                this,
                $"处理APNG帧出错: {ex.Message}");

            // 如果处理出错，返回当前合成位图
            return _compositeBitmap!;
        }
    }

    /// <summary>
    /// 设置APNG实例的可见性状态，用于性能优化
    /// </summary>
    /// <param name="isVisible">是否可见</param>
    public void SetVisibility(bool isVisible)
    {
        _isVisible = isVisible;
    }

    private void ApplyDisposeOperation()
    {
        if (_compositeBitmap == null || _prevCompositeBitmap == null)
            return;

        // 应用前一帧的处置操作
        switch (_prevDisposeOp)
        {
            case DisposeOps.APNGDisposeOpNone:
                // 不需要做任何处理，保留前一帧的渲染结果
                break;

            case DisposeOps.APNGDisposeOpBackground:
                // 将前一帧区域清除为透明
                using (var context = _compositeBitmap.Lock())
                {
                    unsafe
                    {
                        var pixelData = (byte*)context.Address;
                        ClearRegion(pixelData, _prevFrameRect);
                    }
                }

                break;

            case DisposeOps.APNGDisposeOpPrevious:
                // 恢复到渲染前一帧之前的状态
                CopyBitmap(_prevCompositeBitmap, _compositeBitmap);
                break;
        }
    }

    private unsafe void ClearRegion(byte* pixelData, AvaRect region)
    {
        int startX = Math.Max(0, (int)region.X);
        int startY = Math.Max(0, (int)region.Y);
        int endX = Math.Min(ApngPixelSize.Width, (int)(region.X + region.Width));
        int endY = Math.Min(ApngPixelSize.Height, (int)(region.Y + region.Height));

        int stride = ApngPixelSize.Width * 4;

        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                int offset = y * stride + x * 4;
                pixelData[offset] = 0; // B
                pixelData[offset + 1] = 0; // G
                pixelData[offset + 2] = 0; // R
                pixelData[offset + 3] = 0; // A (透明)
            }
        }
    }

    private void ComposeFrame(WriteableBitmap frameBitmap, AvaRect frameRect, BlendOps blendOp)
    {
        if (_compositeBitmap == null)
            return;

        using (var frameContext = frameBitmap.Lock())
        using (var compositeContext = _compositeBitmap.Lock())
        {
            unsafe
            {
                var framePixels = (byte*)frameContext.Address;
                var compositePixels = (byte*)compositeContext.Address;

                int startX = Math.Max(0, (int)frameRect.X);
                int startY = Math.Max(0, (int)frameRect.Y);
                int endX = Math.Min(ApngPixelSize.Width, (int)(frameRect.X + frameRect.Width));
                int endY = Math.Min(ApngPixelSize.Height, (int)(frameRect.Y + frameRect.Height));

                int frameStride = frameBitmap.PixelSize.Width * 4;
                int compositeStride = ApngPixelSize.Width * 4;

                for (int y = startY; y < endY; y++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        int frameX = x - startX;
                        int frameY = y - startY;

                        // 确保不会越界
                        if (frameX < 0 || frameX >= frameBitmap.PixelSize.Width ||
                            frameY < 0 || frameY >= frameBitmap.PixelSize.Height)
                            continue;

                        int frameOffset = frameY * frameStride + frameX * 4;
                        int compositeOffset = y * compositeStride + x * 4;

                        byte frameB = framePixels[frameOffset];
                        byte frameG = framePixels[frameOffset + 1];
                        byte frameR = framePixels[frameOffset + 2];
                        byte frameA = framePixels[frameOffset + 3];

                        byte compositeB = compositePixels[compositeOffset];
                        byte compositeG = compositePixels[compositeOffset + 1];
                        byte compositeR = compositePixels[compositeOffset + 2];
                        byte compositeA = compositePixels[compositeOffset + 3];

                        // 执行混合操作，优化版本
                        if (blendOp == BlendOps.APNGBlendOpSource || frameA == 255)
                        {
                            // 直接覆盖，批量写入4个字节以提高性能
                            *(uint*)(compositePixels + compositeOffset) = *(uint*)(framePixels + frameOffset);
                        }
                        else if (frameA > 0)
                        {
                            // 如果背景完全透明，可以直接使用前景
                            if (compositeA == 0)
                            {
                                *(uint*)(compositePixels + compositeOffset) = *(uint*)(framePixels + frameOffset);
                            }
                            else
                            {
                                // 优化的混合像素算法，使用整数运算代替浮点运算
                                int alpha = frameA;
                                int inverseAlpha = 255 - alpha;
                                int outAlpha = alpha + (compositeA * inverseAlpha) / 255;

                                if (outAlpha > 0)
                                {
                                    // 使用整数算法，避免浮点运算
                                    int alphaRatio = (alpha * 255) / outAlpha;
                                    int inverseAlphaRatio = 255 - alphaRatio;

                                    compositePixels[compositeOffset] =
                                        (byte)((frameB * alphaRatio + compositeB * inverseAlphaRatio) / 255);
                                    compositePixels[compositeOffset + 1] =
                                        (byte)((frameG * alphaRatio + compositeG * inverseAlphaRatio) / 255);
                                    compositePixels[compositeOffset + 2] =
                                        (byte)((frameR * alphaRatio + compositeR * inverseAlphaRatio) / 255);
                                    compositePixels[compositeOffset + 3] = (byte)outAlpha;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void SwapCompositeBitmaps()
    {
        if (_compositeBitmap == null || _prevCompositeBitmap == null)
            return;

        // 如果下一帧需要恢复到渲染前状态，复制当前合成帧到备份
        if (_prevDisposeOp == DisposeOps.APNGDisposeOpPrevious)
        {
            CopyBitmap(_compositeBitmap, _prevCompositeBitmap);
        }
    }

    private void CopyBitmap(WriteableBitmap source, WriteableBitmap destination)
    {
        using (var sourceContext = source.Lock())
        using (var destContext = destination.Lock())
        {
            unsafe
            {
                var sourcePixels = (byte*)sourceContext.Address;
                var destPixels = (byte*)destContext.Address;

                int totalBytes = ApngPixelSize.Width * ApngPixelSize.Height * 4;
                for (int i = 0; i < totalBytes; i++)
                {
                    destPixels[i] = sourcePixels[i];
                }
            }
        }
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                _targetBitmap?.Dispose();
                _compositeBitmap?.Dispose();
                _prevCompositeBitmap?.Dispose();
                CurrentCts.Cancel();
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}