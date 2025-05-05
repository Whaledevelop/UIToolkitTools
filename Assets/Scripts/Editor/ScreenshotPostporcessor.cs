using UnityEditor;
using UnityEngine;

public class ScreenshotPostprocessor : AssetPostprocessor
{
    public static string ExpectedPath;
    public static System.Action OnScreenshotImported;

    // static void OnPostprocessAllAssets(string[] imported, string[] _, string[] __, string[] ___)
    // {
    //     Debug.Log("OnPostprocessAllAssets");
    //     if (string.IsNullOrEmpty(ExpectedPath) || OnScreenshotImported == null)
    //         return;
    //
    //     foreach (var path in imported)
    //     {
    //         if (path == ExpectedPath)
    //         {
    //             Debug.Log("Screenshot file detected via AssetPostprocessor");
    //             OnScreenshotImported.Invoke();
    //             ExpectedPath = null;
    //             OnScreenshotImported = null;
    //             break;
    //         }
    //     }
    // }
    //
    // void OnPostprocessTexture(Texture2D texture)
    // {
    //     Debug.Log("OnPostprocessTexture");
    // }
    //
    // void OnPreprocessAsset()
    // {
    //     Debug.Log($"OnPreprocessAsset {assetPath}");
    // }
}