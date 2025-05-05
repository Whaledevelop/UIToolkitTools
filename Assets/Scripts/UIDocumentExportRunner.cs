using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class UIDocumentExportRunner : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(CaptureRoutine());
    }

    private void CreateGameViewScreenshot(string path)
    {
        var width = Screen.width;
        var height = Screen.height;

        var rt = new RenderTexture(width, height, 24);
        ScreenCapture.CaptureScreenshotIntoRenderTexture(rt);

        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Debug.Log("Created screen shot in " + path);

        RenderTexture.active = null;
        rt.Release();

        Destroy(rt);
        Destroy(tex);
    }

    
    private System.Collections.IEnumerator CaptureRoutine()
    {
        var settings = UIDocumentExportSettings.Load();

        var go = new GameObject("UIDocument", typeof(UIDocument));
        var doc = go.GetComponent<UIDocument>();
        doc.visualTreeAsset = settings.uxml;
        doc.panelSettings = settings.panelSettings;

        Screen.SetResolution(settings.width, settings.height, false);

        yield return new WaitForEndOfFrame();

        var rt = new RenderTexture(settings.width, settings.height, 24);
        ScreenCapture.CaptureScreenshotIntoRenderTexture(rt);

        var tex = new Texture2D(settings.width, settings.height, TextureFormat.RGBA32, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, settings.width, settings.height), 0, 0);
        tex.Apply();

        if (!Directory.Exists(settings.exportFolder))
            Directory.CreateDirectory(settings.exportFolder);

        var path = Path.Combine(settings.exportFolder, "UIDocumentExport.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Debug.Log("[UIDocumentExport] Exported PNG to: " + path);

        RenderTexture.active = null;
        rt.Release();

        Destroy(rt);
        Destroy(tex);
        Destroy(go);

        settings.shouldExport = false;
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(settings);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }
}