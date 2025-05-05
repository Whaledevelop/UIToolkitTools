using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "UIDocumentExportSettings", menuName = "Export/UIDocumentExportSettings")]
public class UIDocumentExportSettings : ScriptableObject
{
    public VisualTreeAsset uxml;
    public PanelSettings panelSettings;
    public string exportFolder;
    public int width;
    public int height;
    public bool shouldExport;

    private const string AssetPath = "Assets/Editor/UIDocumentExportSettings.asset";

    public static void Create(VisualTreeAsset uxml, PanelSettings panelSettings, string folder, int width, int height)
    {
        var settings = AssetDatabase.LoadAssetAtPath<UIDocumentExportSettings>(AssetPath);
        if (settings == null)
        {
            if (!Directory.Exists("Assets/Editor")) Directory.CreateDirectory("Assets/Editor");
            settings = ScriptableObject.CreateInstance<UIDocumentExportSettings>();
            AssetDatabase.CreateAsset(settings, AssetPath);
        }

        settings.uxml = uxml;
        settings.panelSettings = panelSettings;
        settings.exportFolder = folder;
        settings.width = width;
        settings.height = height;
        settings.shouldExport = true;
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
    }

    public static UIDocumentExportSettings Load() =>
        AssetDatabase.LoadAssetAtPath<UIDocumentExportSettings>(AssetPath);
}