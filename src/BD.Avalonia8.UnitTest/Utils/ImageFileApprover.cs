namespace BD.Avalonia8.UnitTest.Utils;

#pragma warning disable SA1600 // Elements should be documented

[SupportedOSPlatform("windows")]
public sealed class ImageFileApprover(IApprovalWriter writer, IApprovalNamer namer, bool normalizeLineEndingsForTextFiles = false) : FileApprover(writer, namer, normalizeLineEndingsForTextFiles)
{
    public override ApprovalException? Approve(string approvedPath, string receivedPath)
    {
        if (Path.GetExtension(approvedPath) != ".png")
            return base.Approve(approvedPath, receivedPath);

        if (!File.Exists(approvedPath))
        {
            return new ApprovalMissingException(receivedPath, approvedPath);
        }

        // FIXME: I have no idea to compare bitmap with Avalonia.Media.Imaging
        //        This logic use System.Drawing, So only run on Windows.

        using var approvedImg = new SDBitmap(approvedPath);
        using var receivedImg = new SDBitmap(receivedPath);

        var approvedByte = BitmapToByte(approvedImg);
        var receivedByte = BitmapToByte(receivedImg);

        return !Compare(receivedByte, approvedByte) ?
                new ApprovalMismatchException(receivedPath, approvedPath) :
                null;
    }

    static byte[] BitmapToByte(SDBitmap bmp)
    {
        var rect = new SDRectangle(0, 0, bmp.Width, bmp.Height);
        var bDt = bmp.LockBits(rect, ImageLockMode.ReadOnly, SDPixelFormat.Format24bppRgb);

        var bary = new byte[bmp.Width * bmp.Height * 3];

        var ptr = bDt.Scan0;
        var lineLen = bmp.Width * 3;
        for (int i = 0; i < bmp.Height; ++i)
        {
            Marshal.Copy(ptr, bary, i * lineLen, lineLen);
            ptr += bDt.Stride;
        }

        bmp.UnlockBits(bDt);

        return bary;
    }

    static bool Compare(byte[] bytes1, byte[] bytes2) => bytes1.SequenceEqual(bytes2);
}