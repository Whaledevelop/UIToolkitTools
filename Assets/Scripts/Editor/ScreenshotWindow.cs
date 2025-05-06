using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;

public class ScreenshotWindow : OdinEditorWindow
{
    private enum Mode
    {
        PngPdf,
        OnlyPNG,
        OnlyPDF
    }

    private enum SizePreset
    {
        Custom,
        Mobile_720x1560,
        A4_1240x1754
    }

    [BoxGroup("Size Settings"), SerializeField, EnumToggleButtons]
    private SizePreset _sizePreset = SizePreset.Mobile_720x1560;

    [BoxGroup("Size Settings"), ShowIf(nameof(_sizePreset), SizePreset.Custom)]
    [SerializeField] private int _width = 720;

    [BoxGroup("Size Settings"), ShowIf(nameof(_sizePreset), SizePreset.Custom)]
    [SerializeField] private int _height = 1560;

    [BoxGroup("Size Settings"), Button]
    private void UpdateGameViewSize()
    {
        var (w, h) = GetSize();
        GameViewEditorUtility.SetGameViewSize(w, h);
    }

    [BoxGroup("Capture"), SerializeField, EnumToggleButtons]
    private Mode _mode = Mode.PngPdf;

    [BoxGroup("Capture"), ShowIf("@_mode == Mode.PngPdf || _mode == Mode.OnlyPDF")]
    [SerializeField] private int _dpi = 150;
    
    [BoxGroup("Capture"), SerializeField, FolderPath]
    private string _createDirectory = "Assets/Exports/";

    [BoxGroup("Capture"), SerializeField]
    private string _nameFormat = "Screenshot_{0}";

    [BoxGroup("Capture"), Button(ButtonSizes.Large)]
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

    private (int width, int height) GetSize()
    {
        return _sizePreset switch
        {
            SizePreset.Mobile_720x1560 => (720, 1560),
            SizePreset.A4_1240x1754 => (1240, 1754),
            _ => (_width, _height)
        };
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
        await UniTask.Delay(1000);
        await CaptureAsync();
        await UniTask.Delay(1000);
        EditorApplication.ExitPlaymode();
    }

    private async UniTask CaptureAsync()
    {
        var path = CombinePath();

        if (_mode is not Mode.OnlyPDF)
            RecorderEditorUtility.CaptureScreenshot(path);

        await UniTask.Delay(1000);
        AssetDatabase.Refresh();
        await UniTask.Delay(1000);

        if (_mode is Mode.PngPdf or Mode.OnlyPDF)
        {
            var imagePath = path + ".png";
            if (!File.Exists(imagePath))
            {
                Debug.LogErrorFormat("No image at path {0}", imagePath);
                return;
            }

            var pdfPath = Path.ChangeExtension(imagePath, ".pdf");
            PdfExportUtility.CreatePdfFromImage(imagePath, pdfPath, _dpi);

            if (_mode == Mode.OnlyPDF)
                File.Delete(imagePath);
        }

        await UniTask.Delay(1000);
        AssetDatabase.Refresh();
    }
}
