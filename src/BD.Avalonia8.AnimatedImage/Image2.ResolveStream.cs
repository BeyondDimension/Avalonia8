namespace BD.Avalonia8.AnimatedImage;

#pragma warning disable SA1600 // Elements should be documented

partial class Image2
{
    public static async ValueTask<Stream?> ResolveObjectToStream(object? obj, Image2 img, CancellationToken token = default)
    {
        Stream? value = null;
        if (obj is string rawUri)
        {
            if (rawUri == string.Empty)
                return null;

            if (File.Exists(rawUri))
                value = new FileStream(rawUri, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            else if (String2.IsHttpUrl(rawUri))
            {
                var isCache = img.EnableCache;

                // Android doesn't allow network requests on the main thread, even though we are using async apis.
                if (OperatingSystem.IsAndroid())
                    await Task.Run(async () =>
                    {
                        var imageHttpClientService = Ioc.Get_Nullable<IImageHttpClientService>();
                        if (imageHttpClientService == null)
                            return;

                        value = await imageHttpClientService.GetImageMemoryStreamAsync(rawUri, cache: isCache, cacheFirst: isCache, cancellationToken: token);

                        if (value == null)
                            return;
                    }, CancellationToken.None);
                else
                {
                    var imageHttpClientService = Ioc.Get_Nullable<IImageHttpClientService>();
                    if (imageHttpClientService == null)
                        return null;

                    value = await imageHttpClientService.GetImageMemoryStreamAsync(rawUri, cache: isCache, cacheFirst: isCache, cancellationToken: token);
                }

                if (value == null)
                    return null;

                var isImage = FileFormat.IsImage(value, out var _);

                if (!isImage)
                    return null;
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    img.Source = value;
                }, DispatcherPriority.Render, CancellationToken.None);
            }
            else if (Uri.TryCreate(rawUri, UriKind.RelativeOrAbsolute, out var uri))
                try
                {
                    if (AssetLoader.Exists(uri))
                        value = AssetLoader.Open(uri);
                }
                catch
                { }
        }
        else if (obj is Uri uri)
        {
            if (uri.OriginalString.Trim().StartsWith("resm"))
            {
                if (AssetLoader.Exists(uri))
                {
                    value = AssetLoader.Open(uri);
                }
            }
        }
        else if (obj is Stream stream)
        {
            value = stream;
        }
        else if (obj is byte[] bytes)
        {
            value = new MemoryStream(bytes);
        }
        else if (obj is ReadOnlyMemory<byte> rom)
        {
            value = new MemoryStream();
            value.Write(rom.Span);
        }
        else if (obj is Memory<byte> m)
        {
            value = new MemoryStream();
            value.Write(m.Span);
        }
        else if (obj is IEnumerable<byte> byte_enum)
        {
            value = new MemoryStream();
            foreach (var item in byte_enum)
            {
                value.WriteByte(item);
            }
        }
        else if (obj is CommonImageSource commonImageSource)
        {
            value = commonImageSource.Stream;
        }

        if (value == null || !value.CanRead || value.Length == 0)
            return null;

        value.Position = 0;

        return value;
    }
}
