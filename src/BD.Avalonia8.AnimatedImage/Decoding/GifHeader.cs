// Licensed under the MIT License.
// Copyright (C) 2018 Jumar A. Macato, All Rights Reserved.

namespace BD.Avalonia8.AnimatedImage.Decoding;

#pragma warning disable SA1600 // Elements should be documented

public record struct GifHeader
{
    public bool HasGlobalColorTable;
    public int GlobalColorTableSize;
    public ulong GlobalColorTableCacheId;
    public int BackgroundColorIndex;
    public long HeaderSize;
    internal int Iterations = -1;
    public GifRepeatBehavior? IterationCount;
    public GifRect Dimensions;
    public GifColor[]? GlobarColorTable;

    public GifHeader()
    {
    }
}