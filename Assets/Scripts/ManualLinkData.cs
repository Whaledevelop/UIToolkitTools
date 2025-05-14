using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Screenshot/Manual Link Data")]
public class ManualLinkData : ScriptableObject
{
    [Serializable]
    public class ManualLinkRect
    {
        public string url;
        public Rect rect;
    }

    [SerializeField]
    private List<ManualLinkRect> _links = new();

    public List<ManualLinkRect> Links => _links;

    private const string SaveFileName = "manual_links.json";

    [Button("Save Links To File")]
    public void Save()
    {
        var json = JsonUtility.ToJson(new ManualLinkList { links = _links }, true);
        var path = GetSavePath();
        File.WriteAllText(path, json);
        Debug.Log($"[ManualLinkData] Links saved to: {path}");
    }

    [Button("Load Links From File")]
    public void Load()
    {
        var path = GetSavePath();
        if (!File.Exists(path))
        {
            Debug.LogWarning("[ManualLinkData] No saved links found.");
            return;
        }

        var json = File.ReadAllText(path);
        var loaded = JsonUtility.FromJson<ManualLinkList>(json);
        _links = loaded.links ?? new List<ManualLinkRect>();
        Debug.Log($"[ManualLinkData] Loaded {_links.Count} links from: {path}");
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, SaveFileName);
    }

    [Serializable]
    private class ManualLinkList
    {
        public List<ManualLinkRect> links;
    }
}