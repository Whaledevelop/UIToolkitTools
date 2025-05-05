using System.IO;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;

public class ScreenshotWindow : OdinEditorWindow
{
    [FolderPath]
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


}