// Licensed under the MIT License.
// Copyright (C) 2018 Jumar A. Macato, All Rights Reserved.

namespace BD.Avalonia8.Image2.Decoding;

public record class GifHeader
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
}