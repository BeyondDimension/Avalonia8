namespace BD.Avalonia8.Image2;

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