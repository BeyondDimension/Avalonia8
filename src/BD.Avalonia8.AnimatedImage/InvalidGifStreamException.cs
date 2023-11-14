namespace BD.Avalonia8.AnimatedImage;

#pragma warning disable SA1600 // Elements should be documented

[Serializable]
internal class InvalidGifStreamException : ApplicationException
{
    public InvalidGifStreamException()
    {
    }

    public InvalidGifStreamException(string message) : base(message)
    {
    }

    public InvalidGifStreamException(string message, Exception innerException) : base(message, innerException)
    {
    }
}