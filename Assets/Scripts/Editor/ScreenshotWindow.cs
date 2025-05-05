using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;

public class ScreenshotWindow : OdinEditorWindow
{
    [Flags]
    private enum Format : byte
    {
        PNG = 1 << 0,
        PDF = 1 << 1
    }

    [SerializeField, FolderPath]
    private string _createDirectory = "Assets/Exports/";

    [SerializeField]
    private string _nameFormat = "Screenshot_{0}";

    [SerializeField] 
    private Format _format;
    
    private bool _pendingCapture;
    
    [MenuItem("Tools/ScreenshotWindow")]
    private static void Open()
    {
        GetWindow<ScreenshotWindow>().Show();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EditorApplication.update += OnEditorUpdate;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EditorApplication.update -= OnEditorUpdate;
    }

    [Button(ButtonSizes.Large)]
    private void CaptureScreenshot()
    {
        if (Application.isPlaying)
        {
            CaptureAsync().Forget();
        }
        else
        {
            _pendingCapture = true;
            EditorApplication.EnterPlaymode();
        }
    }

    private string CombinePath()
    {
        var baseName = string.Format(_nameFormat, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        return Path.Combine(_createDirectory, baseName).Replace("\\", "/");
    }

    private void OnEditorUpdate()
    {
        if (_pendingCapture && Application.isPlaying)
        {
            _pendingCapture = false;
            CaptureWithAppPlayAsync().Forget();
        }
    }

    private async UniTaskVoid CaptureWithAppPlayAsync()
    {
        await UniTask.Delay(2000);
        await CaptureAsync();
        await UniTask.Delay(1000);
        EditorApplication.ExitPlaymode();
    }
    
    private async UniTask CaptureAsync()
    {
        if (!_format.HasFlag(Format.PNG) && !_format.HasFlag(Format.PDF))
        {
            Debug.LogError("No format selected for screenshot");
            return;
        }
        var path = CombinePath();
        RecorderUtility.CaptureScreenshot(path);
        await UniTask.Delay(1000);
        AssetDatabase.Refresh();
        await UniTask.Delay(1000);
        if (_format.HasFlag(Format.PDF))
        {
            var imagePath = path + ".png";
            if (!File.Exists(imagePath))
            {
                Debug.LogErrorFormat("No image ar path {0}", imagePath);
                return;
            }
            var pdfPath = Path.ChangeExtension(imagePath, ".pdf");
            PdfExportUtility.CreatePdfFromImage(imagePath, pdfPath);
        }
        await UniTask.Delay(1000);
        AssetDatabase.Refresh();
    }
}
