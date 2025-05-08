using PdfSharp.Drawing;
using PdfSharp.Pdf;

public static class PdfExportUtility
{
    public static void CreatePdfFromImage(string imagePath, string pdfPath, int dpi = 150)
    {
        var doc = new PdfDocument();
        var page = doc.AddPage();

        var img = XImage.FromFile(imagePath);
        page.Width = XUnit.FromInch(img.PixelWidth / (double)dpi);
        page.Height = XUnit.FromInch(img.PixelHeight / (double)dpi);

        var gfx = XGraphics.FromPdfPage(page);
        gfx.DrawImage(img, 0, 0, page.Width, page.Height);

        doc.Save(pdfPath);
    }
}