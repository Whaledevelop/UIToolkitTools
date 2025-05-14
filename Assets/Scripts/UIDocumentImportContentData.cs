using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Screenshot/UIDocument Import Data")]
public class UIDocumentImportContentData : ScriptableObject
{
    [Serializable]
    public class ManualLinkRect
    {
        public string url;
        public Rect rect;
    }

    [SerializeField] private List<MarkdownBindings.Binding> _bindings = new();
    [SerializeField, TableList] private List<ManualLinkRect> _links = new();

    public List<MarkdownBindings.Binding> Bindings => _bindings;
    public List<ManualLinkRect> Links => _links;

    private const string SaveFileName = "uidoc_content.json";

    [Button("Save All To File")]
    public void Save()
    {
        var json = JsonUtility.ToJson(new SaveData { bindings = _bindings, links = _links }, true);
        var path = GetSavePath();
        File.WriteAllText(path, json);
        Debug.Log($"[UIDocumentContentData] Saved to: {path}");
    }

    [Button("Load All From File")]
    public void Load()
    {
        var path = GetSavePath();
        if (!File.Exists(path))
        {
            Debug.LogWarning("[UIDocumentContentData] No saved file found.");
            return;
        }

        var json = File.ReadAllText(path);
        var loaded = JsonUtility.FromJson<SaveData>(json);
        _bindings = loaded.bindings ?? new();
        _links = loaded.links ?? new();
        Debug.Log($"[UIDocumentContentData] Loaded {_bindings.Count} bindings, {_links.Count} links.");
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, SaveFileName);
    }

    [Serializable]
    private class SaveData
    {
        public List<MarkdownBindings.Binding> bindings;
        public List<ManualLinkRect> links;
    }
}