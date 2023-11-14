namespace BD.Avalonia8.AnimatedImage.Decoding;

#pragma warning disable SA1600 // Elements should be documented

public enum FrameDisposal : byte
{
    Unknown = 0,
    Leave = 1,
    Background = 2,
    Restore = 3,
}