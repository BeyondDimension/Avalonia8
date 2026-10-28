namespace BD.Avalonia8.Image2;

partial class Image2
{
    static bool TryFileExists(string filePath)
    {
        try
        {
            return File.Exists(filePath);
        }
        catch
        {
            return false;
        }
    }

    // The returned stream is owned by the caller; Source is never replaced with it.
    public static ValueTask<Stream?> ResolveObjectToStream(object? obj, Image2 img,
        CancellationToken token = default) =>
        ResolveObjectToStreamCore(obj, img.KeepSourceStreamOpen, img.EnableCache, token);

    static async ValueTask<Stream?> ResolveObjectToStreamCore(object? obj, bool keepOpen,
        bool cache, CancellationToken token)
    {
        Stream? value = !keepOpen ? obj as Stream : null;
        try
        {
            token.ThrowIfCancellationRequested();
            if (obj is string rawUri)
            {
                if (rawUri.Length == 0)
                    return null;
                if (TryFileExists(rawUri))
                    value = new FileStream(rawUri, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                else if (String2.IsHttpUrl(rawUri))
                {
                    var service = Ioc.Get_Nullable<IImageHttpClientService>();
                    if (service == null)
                        return null;
                    value = await service.GetImageMemoryStreamAsync(rawUri, cache: cache,
                        cacheFirst: cache, cancellationToken: token).ConfigureAwait(false);
                }
                else if (Uri.TryCreate(rawUri, UriKind.RelativeOrAbsolute, out var uri) && AssetLoader.Exists(uri))
                    value = AssetLoader.Open(uri);
            }
            else if (obj is Uri uri)
            {
                if (AssetLoader.Exists(uri))
                    value = AssetLoader.Open(uri);
            }
            else if (obj is Stream stream)
            {
                // Borrowed sources are copied once, restoring their position. This also
                // isolates GIF frame seeking from metadata editing and other controls.
                if (keepOpen)
                {
                    lock (stream)
                        value = stream.SafeCopyToRecyclableMemoryStream("Image2.BorrowedSource");
                }
                else
                    value = stream;
            }
            else if (obj is byte[] bytes)
                value = new MemoryStream(bytes, writable: false);
            else if (obj is ReadOnlyMemory<byte> rom)
            {
                value = MemoryStreamManager.GetStream("Image2.ReadOnlyMemory");
                value.Write(rom.Span);
            }
            else if (obj is Memory<byte> memory)
            {
                value = MemoryStreamManager.GetStream("Image2.Memory");
                value.Write(memory.Span);
            }
            else if (obj is IEnumerable<byte> sequence)
            {
                value = MemoryStreamManager.GetStream("Image2.Sequence");
                foreach (var item in sequence)
                {
                    token.ThrowIfCancellationRequested();
                    value.WriteByte(item);
                }
            }
            else if (obj is CommonImageSource commonImageSource && commonImageSource.Stream is { } commonStream)
            {
                lock (commonStream)
                    value = commonStream.SafeCopyToRecyclableMemoryStream("Image2.CommonImageSource");
            }

            token.ThrowIfCancellationRequested();
            if (value == null)
                return null;
            if (!value.CanRead)
            {
                value.Dispose();
                return null;
            }
            if (!value.CanSeek)
                value = value.SafeCopyToRecyclableMemoryStream("Image2.NonSeekable", true);
            if (value.Length == 0)
            {
                value.Dispose();
                return null;
            }
            value.Position = 0;
            return value;
        }
        catch
        {
            value?.Dispose();
            throw;
        }
    }
}
