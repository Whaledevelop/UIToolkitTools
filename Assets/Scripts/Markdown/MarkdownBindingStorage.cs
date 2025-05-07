using System.IO;
using UnityEngine;

public static class MarkdownBindingStorage
{
    private static string Path => System.IO.Path.Combine(Application.persistentDataPath, "markdown_bindings.json");

    public static void Save(MarkdownBindings data)
    {
        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path, json);
    }

    public static MarkdownBindings Load()
    {
        if (!File.Exists(Path))
            return new MarkdownBindings();

        var json = File.ReadAllText(Path);
        return JsonUtility.FromJson<MarkdownBindings>(json);
    }
}