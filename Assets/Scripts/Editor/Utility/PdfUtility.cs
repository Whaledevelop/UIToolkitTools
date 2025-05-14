using System.Collections.Generic;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Annotations;
using PdfSharp.Pdf.Advanced;
using System.IO;
using System.Text.RegularExpressions;
using PdfSharp.Pdf.IO;
using UnityEngine;
using UnityEngine.UIElements;

public static class PdfUtility
{
    const float PointsPerInch = 72f;

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

    public static void AddManualLinkAnnotations(string pdfPath, List<UIDocumentImportContentData.ManualLinkRect> linkRects, int dpi = 150)
    {
        var doc = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Modify);
        var page = doc.Pages[0];
        var scale = PointsPerInch / dpi;

        foreach (var link in linkRects)
        {
            var r = link.rect;
            var pdfX = r.x * scale;
            var pdfY = (page.Height.Point - (r.y + r.height) * scale);
            var pdfWidth = r.width * scale;
            var pdfHeight = r.height * scale;

            var rect = new PdfRectangle(
                new XPoint(pdfX, pdfY),
                new XPoint(pdfX + pdfWidth, pdfY + pdfHeight)
            );

            var annotation = new PdfLinkAnnotation
            {
                Rectangle = rect
            };

            var action = new PdfDictionary(page.Owner)
            {
                Elements =
                {
                    ["/S"] = new PdfName("/URI"),
                    ["/URI"] = new PdfString(link.url)
                }
            };

            annotation.Elements["/A"] = action;
            page.Annotations.Add(annotation);

            //Debug.Log($"[PdfUtility] Manual link to '{link.url}' at PDF rect ({pdfX:0.##}, {pdfY:0.##}, {pdfWidth:0.##}, {pdfHeight:0.##})");
        }

        doc.Save(pdfPath);
    }

}
