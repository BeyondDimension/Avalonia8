namespace BD.Avalonia8.Image2.Decoding;

public readonly record struct GifRepeatBehavior
{
    public readonly bool LoopForever { get; init; }

    public readonly int? Count { get; init; }
}
