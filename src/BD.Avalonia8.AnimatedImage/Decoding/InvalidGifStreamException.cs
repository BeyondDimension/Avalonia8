// Licensed under the MIT License.
// Copyright (C) 2018 Jumar A. Macato, All Rights Reserved.

namespace BD.Avalonia8.AnimatedImage.Decoding;

#pragma warning disable SA1600 // Elements should be documented

[Serializable]
public sealed class InvalidGifStreamException : ApplicationException
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