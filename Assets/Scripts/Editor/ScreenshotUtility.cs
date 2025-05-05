using UnityEngine;
using System.IO;
using System.Collections;
using UnityEditor;

public static class ScreenshotUtility
{
    public static void Capture(string path, MonoBehaviour context)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Debug.LogWarning("ScreenshotUtility can only be used in Play Mode.");
            return;
        }
#endif
        context.StartCoroutine(CaptureRoutine(path));
    }

    static IEnumerator CaptureRoutine(string path)
    {
        
        yield return new WaitForEndOfFrame();
        var camera = Camera.main;
        if (camera == null)
        {
            Debug.LogWarning("No main camera found.");
            yield break;
        }

        var width = Screen.width;
        var height = Screen.height;

        var rt = new RenderTexture(width, height, 24);
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        var prevRT = RenderTexture.active;
        camera.targetTexture = rt;
        camera.Render();
        yield return new WaitForEndOfFrame();
        RenderTexture.active = rt;

        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        yield return new WaitForEndOfFrame();

        camera.targetTexture = null;
        RenderTexture.active = prevRT;

        File.WriteAllBytes(path, tex.EncodeToPNG());
        Debug.Log("Screenshot saved to: " + path);

        yield return new WaitForEndOfFrame();
        rt.Release();
        Object.Destroy(rt);
        Object.Destroy(tex);
        yield return new WaitForEndOfFrame();
        AssetDatabase.Refresh();
    }
}