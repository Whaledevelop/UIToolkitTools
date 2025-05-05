using System.IO;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;

public class ScreenshotWindow : OdinEditorWindow
{
    [Sirenix.OdinInspector.FilePath(Extensions = "png")]
    public string exportPath = "Assets/Exports/Test.png";

    [MenuItem("Tools/ScreenshotWindow")]
    private static void Open()
    {
        GetWindow<ScreenshotWindow>().titleContent = new GUIContent("UI Exporter");
    }

    [Button]
    private void CreateGameViewScreenshot()
    {
        var uiDocument = FindFirstObjectByType<UIDocument>();
        ScreenshotUtility.Capture(exportPath, uiDocument);
    }

    [Button]
    private void CreateScreenshotWithRecorder()
    {
        RecorderUtility.CaptureScreenshot(exportPath);
        AssetDatabase.Refresh();
    }

    [Button]
    private void CreatePdfFromScreenshot()
    {
        var pngPath = exportPath;
        var pdfPath = Path.ChangeExtension(pngPath, ".pdf");

        PdfExportUtility.CreatePdfFromImage(pngPath, pdfPath);
        Debug.Log("PDF created at: " + pdfPath);
        
                AssetDatabase.Refresh();
    }
}