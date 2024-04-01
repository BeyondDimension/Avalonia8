// Licensed under the MIT License.
// Copyright (C) 2018 Jumar A. Macato, All Rights Reserved.

namespace BD.Avalonia8.Image2.Decoding;

[Serializable]
public sealed class LzwDecompressionException : ApplicationException
{
    public LzwDecompressionException()
    {
    }

    public LzwDecompressionException(string message) : base(message)
    {
    }

    public LzwDecompressionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}