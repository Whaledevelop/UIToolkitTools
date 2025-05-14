using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;

public class ScreenshotWindow : OdinEditorWindow
{
    private enum Mode
    {
        PngPdf,
        OnlyPNG,
        OnlyPDF
    }

    [FoldoutGroup("Size Settings"), SerializeField]
    private int _width = 720;

    [FoldoutGroup("Size Settings"), SerializeField]
    private int _height = 1560;

    [FoldoutGroup("Capture Settings"), SerializeField, EnumToggleButtons]
    private Mode _mode = Mode.PngPdf;

    [FoldoutGroup("Capture Settings"), ShowIf("@_mode == Mode.PngPdf || _mode == Mode.OnlyPDF")]
    [SerializeField] private int _dpi = 150;

    [FoldoutGroup("Capture Settings"), SerializeField, FolderPath]
    private string _createDirectory = "Assets/Exports/";

    [FoldoutGroup("Capture Settings"), SerializeField]
    private string _nameFormat = "Screenshot_{0}";

    [FoldoutGroup("Manual Link Areas"), SerializeField]
    private ManualLinkData _manualLinkData;

    [FoldoutGroup("Manual Link Areas")]
    public bool ShowLinksAreas;

    [FoldoutGroup("Size Settings"), Button(ButtonSizes.Large)]
    private void SetSize()
    {
        GameViewEditorUtility.SetGameViewSize(_width, _height);
    }

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
        return (_width, _height);
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

        if (ShowLinksAreas)
        {
            TryRenderOverlay();
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
            PdfUtility.CreatePdfFromImage(imagePath, pdfPath, _dpi);
            PdfUtility.AddManualLinkAnnotations(pdfPath, _manualLinkData.Links, _dpi);

            if (_mode == Mode.OnlyPDF)
                File.Delete(imagePath);
        }

        await UniTask.Delay(1000);
        AssetDatabase.Refresh();
    }

    private void TryRenderOverlay()
    {
        var doc = UnityEngine.Object.FindFirstObjectByType<UIDocument>();
        if (doc != null)
            ShowDebugOverlay(doc, _manualLinkData.Links);
    }

    public static void ShowDebugOverlay(UIDocument doc, List<ManualLinkData.ManualLinkRect> links)
    {
        var root = doc.rootVisualElement;
        var old = root.Q<VisualElement>("DebugOverlay");
        if (old != null) root.Remove(old);

        var overlay = new VisualElement
        {
            name = "DebugOverlay",
            pickingMode = PickingMode.Ignore
        };
        overlay.style.position = Position.Absolute;
        overlay.style.top = 0;
        overlay.style.left = 0;
        overlay.style.right = 0;
        overlay.style.bottom = 0;

        foreach (var link in links)
        {
            var box = new VisualElement();
            box.style.position = Position.Absolute;
            box.style.left = link.rect.x;
            box.style.top = link.rect.y;
            box.style.width = link.rect.width;
            box.style.height = link.rect.height;
            box.style.borderTopWidth = 2;
            box.style.borderLeftWidth = 2;
            box.style.borderRightWidth = 2;
            box.style.borderBottomWidth = 2;
            box.style.borderTopColor = Color.red;
            box.style.borderLeftColor = Color.red;
            box.style.borderRightColor = Color.red;
            box.style.borderBottomColor = Color.red;
            overlay.Add(box);
        }

        root.Add(overlay);
    }

    [FoldoutGroup("Manual Link Areas"), Button("Save Links")]
    private void SaveLinks()
    {
        _manualLinkData.Save();
    }

    [FoldoutGroup("Manual Link Areas"), Button("Load Links")]
    private void LoadLinks()
    {
        _manualLinkData.Load();
    }
}
