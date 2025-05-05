using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;

public static class PdfExportUtility
{
    public static void CreatePdfFromImage(string imagePath, string pdfPath)
    {
        if (!File.Exists(imagePath)) return;

        var doc = new PdfDocument();
        var page = doc.AddPage();

        var img = XImage.FromFile(imagePath);

        page.Width = XUnit.FromPoint(img.PixelWidth);
        page.Height = XUnit.FromPoint(img.PixelHeight);

        var gfx = XGraphics.FromPdfPage(page);
        gfx.DrawImage(img, XUnit.FromPoint(0), XUnit.FromPoint(0));

        doc.Save(pdfPath);
    }
}