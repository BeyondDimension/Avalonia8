using ApprovalTests.Core;
using Avalonia.Media.Imaging;
using BD.Avalonia8.UnitTest.Utils;

namespace BD.Avalonia8.UnitTest;

public sealed class ApprovalImageWriter(Bitmap image, object parameter) : IApprovalWriter
{
    public Bitmap Data { get; set; } = image ?? throw new ArgumentNullException(nameof(image));

    public string Parameter { get; } = parameter?.ToString() ?? "null";

    public string GetApprovalFilename(string baseName)
    {
        return $"{baseName}#{Parameter}.approved.png";
    }

    public string GetReceivedFilename(string baseName)
    {
        return $"{baseName}#{Parameter}.received.png";
    }

    internal static readonly Lock fileRWLock = new();

    public string WriteReceivedFile(string received)
    {
        var dir = Path.GetDirectoryName(received);
        if (dir is not null)
            Directory.CreateDirectory(dir);
        lock (fileRWLock)
        {
            IOPath.FileTryDelete(received);
            Data.Save(received);
        }
        return received;
    }
}