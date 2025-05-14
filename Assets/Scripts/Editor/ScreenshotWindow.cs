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

    private enum SizePreset
    {
        Custom,
        Mobile_720x1560,
        A4_1240x1754
    }

    [Serializable]
    public class ManualLinkRect
    {
        public string url;
        public Rect rect;
    }

    [FoldoutGroup("Size Settings"), SerializeField, EnumToggleButtons]
    private SizePreset _sizePreset = SizePreset.Mobile_720x1560;

    [FoldoutGroup("Size Settings"), ShowIf(nameof(_sizePreset), SizePreset.Custom)]
    [SerializeField] private int _width = 720;

    [FoldoutGroup("Size Settings"), ShowIf(nameof(_sizePreset), SizePreset.Custom)]
    [SerializeField] private int _height = 1560;

    [FoldoutGroup("Capture Settings"), SerializeField, EnumToggleButtons]
    private Mode _mode = Mode.PngPdf;

    [FoldoutGroup("Capture Settings"), ShowIf("@_mode == Mode.PngPdf || _mode == Mode.OnlyPDF")]
    [SerializeField] private int _dpi = 150;

    [FoldoutGroup("Capture Settings"), SerializeField, FolderPath]
    private string _createDirectory = "Assets/Exports/";

    [FoldoutGroup("Capture Settings"), SerializeField]
    private string _nameFormat = "Screenshot_{0}";

    [FoldoutGroup("Manual Link Areas"), TableList(AlwaysExpanded = true)]
    public List<ManualLinkRect> ManualLinks = new();

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
            PdfUtility.AddManualLinkAnnotations(pdfPath, ManualLinks, _dpi);

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
            ShowDebugOverlay(doc, ManualLinks);
    }
    
    public static void ShowDebugOverlay(UIDocument doc, List<ScreenshotWindow.ManualLinkRect> links)
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
        var json = JsonUtility.ToJson(new ManualLinkList { links = ManualLinks }, true);
        var path = GetLinksSavePath();
        File.WriteAllText(path, json);
        Debug.Log($"[ScreenshotWindow] Links saved to: {path}");
    }

    [FoldoutGroup("Manual Link Areas"), Button("Load Links")]
    private void LoadLinks()
    {
        var path = GetLinksSavePath();
        if (!File.Exists(path))
        {
            Debug.LogWarning("[ScreenshotWindow] No saved links found.");
            return;
        }

        var json = File.ReadAllText(path);
        var loaded = JsonUtility.FromJson<ManualLinkList>(json);
        ManualLinks = loaded.links ?? new List<ManualLinkRect>();
        Debug.Log($"[ScreenshotWindow] Loaded {ManualLinks.Count} links from: {path}");
    }

    private string GetLinksSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "manual_links.json");
    }

    [Serializable]
    private class ManualLinkList
    {
        public List<ManualLinkRect> links;
    }
}
