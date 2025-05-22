using ApprovalTests.Approvers;
using ApprovalTests.Core;
using ApprovalTests.Core.Exceptions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace BD.Avalonia8.UnitTest.Utils;

[SupportedOSPlatform("windows")]
public sealed class ImageFileApprover(IApprovalWriter writer, IApprovalNamer namer, bool normalizeLineEndingsForTextFiles = false) : FileApprover(writer, namer, normalizeLineEndingsForTextFiles)
{
    static Bitmap GetBitmap(string filePath)
    {
        lock (ApprovalImageWriter.fileRWLock)
        {
            try
            {
                // 命令行执行测试并发可能导致 new Bitmap(string) 时引发 System.IO.IOException : The process cannot access the file {0} because it is being used by another process.
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                return new(fileStream);
            }
            catch (Exception ex)
            {
                throw new Exception($"GetBitmap fail, filePath: {filePath}", ex);
            }

        }
    }


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

        using var approvedImg = GetBitmap(approvedPath);
        using var receivedImg = GetBitmap(receivedPath);

        var approvedByte = BitmapToByte(approvedImg);
        var receivedByte = BitmapToByte(receivedImg);

        return !Compare(receivedByte, approvedByte) ?
                new ApprovalMismatchException(receivedPath, approvedPath) :
                null;
    }

    static byte[] BitmapToByte(Bitmap bmp)
    {
        var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        var bDt = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

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