#pragma warning disable SA1600 // Elements should be documented

namespace BD.Avalonia8.UnitTest;

public sealed class ApprovalImageWriter(AvaBitmap image, object parameter) : IApprovalWriter
{
    public AvaBitmap Data { get; set; } = image ?? throw new ArgumentNullException(nameof(image));

    public string Parameter { get; } = parameter?.ToString() ?? "null";

    public string GetApprovalFilename(string baseName)
    {
        return $"{baseName}#{Parameter}.approved.png";
    }

    public string GetReceivedFilename(string baseName)
    {
        return $"{baseName}#{Parameter}.received.png";
    }

    public string WriteReceivedFile(string received)
    {
        var dir = Path.GetDirectoryName(received);
        if (dir is not null)
            Directory.CreateDirectory(dir);

        IOPath.FileTryDelete(received);
        Data.Save(received);
        return received;
    }
}