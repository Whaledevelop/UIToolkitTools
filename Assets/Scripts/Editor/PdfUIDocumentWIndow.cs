using System.Collections.Generic;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Annotations;
using PdfSharp.Drawing;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PdfUIDocumentWindow : OdinEditorWindow
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField, Sirenix.OdinInspector.FilePath(Extensions = ".png")] private string _imagePath;
    [SerializeField, Sirenix.OdinInspector.FilePath(Extensions = ".pdf")] private string _pdfPath;
    [SerializeField] private int _dpi = 150;

    [TableList] public List<ClickableRegion> _regions = new();

    [MenuItem("Tools/PDF/PDF UI Document Window")]
    private static void Open()
    {
        GetWindow<PdfUIDocumentWindow>().Show();
    }

    [Button("Add Links To PDF", ButtonSizes.Large)]
    private void AddLinksToPdf()
    {
        if (!File.Exists(_imagePath) || !File.Exists(_pdfPath))
        {
            Debug.LogError("Invalid file path(s)");
            return;
        }

        var doc = PdfSharp.Pdf.IO.PdfReader.Open(_pdfPath, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Modify);
        var page = doc.Pages[0];

        // foreach (var region in _regions)
        // {
        //     var ve = _uiDocument.rootVisualElement.Q<VisualElement>(region.ElementName);
        //     if (ve == null)
        //     {
        //         Debug.LogWarning($"Element '{region.ElementName}' not found");
        //         continue;
        //     }
        //
        //     Rect bounds;
        //
        //     if (region.TextRangeMode == ClickableRegion.RangeMode.FullElement)
        //     {
        //         bounds = ve.worldBound;
        //     }
        //     else
        //     {
        //         if (ve is Label label && !string.IsNullOrEmpty(region.TextFragment))
        //         {
        //             var text = label.text;
        //             var index = text.IndexOf(region.TextFragment);
        //             if (index >= 0)
        //             {
        //                 var charWidth = ve.worldBound.width / text.Length;
        //                 var x = ve.worldBound.x + index * charWidth;
        //                 var w = region.TextFragment.Length * charWidth;
        //                 bounds = new Rect(x, ve.worldBound.y, w, ve.worldBound.height);
        //             }
        //             else
        //             {
        //                 Debug.LogWarning($"Text fragment '{region.TextFragment}' not found in element '{region.ElementName}'");
        //                 continue;
        //             }
        //         }
        //         else
        //         {
        //             Debug.LogWarning($"Element '{region.ElementName}' must be a Label to use TextFragment mode");
        //             continue;
        //         }
        //     }
        //
        //     var scale = _dpi / 96f;
        //     var x = bounds.x * scale;
        //     var y = (Screen.height - bounds.yMax) * scale;
        //     var width = bounds.width * scale;
        //     var height = bounds.height * scale;
        //
        //     var rect = new XRect(x, y, width, height);
        //
        //     var link = new PdfLinkAnnotation(doc)
        //     {
        //         Rectangle = rect,
        //         Uri = region.Url
        //     };
        //
        //     page.Annotations.Add(link);
        // }

        doc.Save(_pdfPath);
        AssetDatabase.Refresh();
        Debug.Log("PDF updated with links.");
    }

    [System.Serializable]
    public class ClickableRegion
    {
        [TableColumnWidth(120)] public string ElementName;
        [TableColumnWidth(500)] public string Url;
        [TableColumnWidth(150)] public RangeMode TextRangeMode;
        [ShowIf("TextRangeMode", RangeMode.TextFragment)]
        public string TextFragment;

        public enum RangeMode
        {
            FullElement,
            TextFragment
        }
    }
}