namespace BD.Avalonia8.Image2.Decoding;

public enum FrameDisposal : byte
{
    Unknown = 0,
    Leave = 1,
    Background = 2,
    Restore = 3,
}