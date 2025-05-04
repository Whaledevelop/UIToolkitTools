using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using System.Collections;
using System.IO;
using Sirenix.OdinInspector.Editor;

public class UIDocumentExporterWindow : OdinEditorWindow
{
    [Title("UI Document Exporter")]
    [Required]
    public VisualTreeAsset uxml;

    [Required]
    public PanelSettings panelSettings;

    [FolderPath]
    public string exportFolder = "Assets/Exports";

    [MinValue(100)]
    public int exportWidth = 794;

    [MinValue(100)]
    public int exportHeight = 1123;

    [Button("Export as PNG")]
    private void ExportAsPNG()
    {
        if (uxml == null || panelSettings == null)
        {
            Debug.LogError("UXML or PanelSettings not assigned.");
            return;
        }

        var go = new GameObject("UIDocumentCapture");
        var uiDoc = go.AddComponent<UIDocument>();
        uiDoc.visualTreeAsset = uxml;
        uiDoc.panelSettings = panelSettings;

        var cameraGO = new GameObject("UICaptureCamera");
        var camera = cameraGO.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.white;
        camera.orthographic = true;
        camera.enabled = false;

        var renderTex = new RenderTexture(exportWidth, exportHeight, 24);
        camera.targetTexture = renderTex;

        EditorApplication.delayCall += () =>
        {
            var tex = new Texture2D(exportWidth, exportHeight, TextureFormat.RGBA32, false);
            camera.Render();
            RenderTexture.active = renderTex;
            tex.ReadPixels(new Rect(0, 0, exportWidth, exportHeight), 0, 0);
            tex.Apply();

            var bytes = tex.EncodeToPNG();
            var filePath = Path.Combine(exportFolder, "UIDocumentExport.png");
            File.WriteAllBytes(filePath, bytes);
            Debug.Log("Saved PNG to: " + filePath);

            RenderTexture.active = null;
            Object.DestroyImmediate(tex);
            Object.DestroyImmediate(renderTex);
            Object.DestroyImmediate(go);
            Object.DestroyImmediate(cameraGO);

            AssetDatabase.Refresh();
        };
    }

    [MenuItem("Tools/UIDocument Exporter")]
    private static void Open()
    {
        GetWindow<UIDocumentExporterWindow>().titleContent = new GUIContent("UI Exporter");
    }
}