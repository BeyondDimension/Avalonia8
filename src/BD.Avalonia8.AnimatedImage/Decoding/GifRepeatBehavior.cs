namespace BD.Avalonia8.AnimatedImage.Decoding;

#pragma warning disable SA1600 // Elements should be documented

public readonly record struct GifRepeatBehavior
{
    public readonly bool LoopForever { get; init; }

    public readonly int? Count { get; init; }
}
