using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UIToolkitTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class UIDocumentToPdfExporterWindow : OdinEditorWindow
{
    private enum Mode
    {
        PngPdf,
        OnlyPNG,
        OnlyPDF
    }

    [FoldoutGroup("Size Settings"), SerializeField] private int _width = 720;
    [FoldoutGroup("Size Settings"), SerializeField] private int _height = 1560;

    [FoldoutGroup("Capture Settings"), SerializeField, EnumToggleButtons] private Mode _mode = Mode.PngPdf;
    [FoldoutGroup("Capture Settings"), ShowIf("@_mode == Mode.PngPdf || _mode == Mode.OnlyPDF")]
    [SerializeField] private int _dpi = 150;

    [FoldoutGroup("Capture Settings"), SerializeField, FolderPath] private string _createDirectory = "Assets/Exports/";
    [FoldoutGroup("Capture Settings"), SerializeField] private string _nameFormat = "Screenshot_{0}";

    [FoldoutGroup("Import content"), SerializeField] private UIDocumentImportContentData _importContentData;
    [FoldoutGroup("Import content"), SerializeField] private bool _showLinksAreas;
    [FoldoutGroup("Import content"), SerializeField] private MarkdownStyleSettings _styleSettings;
    
    private UIDocument _uiDocument;
    private bool _pendingCapture;


    [MenuItem("Tools/ScreenshotWindow")]
    private static void Open()
    {
        GetWindow<UIDocumentToPdfExporterWindow>().Show();
    }

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

    [FoldoutGroup("Import content"), Button("Apply to Scene")]
    private void ApplyMarkdownBindings()
    {
        if (!ValidateMarkdownState()) return;

        var container = _uiDocument.rootVisualElement;
        var hex = new MarkdownColorHex(_styleSettings);

        foreach (var binding in _importContentData.Bindings)
        {
            if (string.IsNullOrEmpty(binding.elementName)) continue;

            var element = container.Q<VisualElement>(binding.elementName);
            if (element == null)
            {
                Debug.LogWarning($"Skipped: element not found {binding.elementName}");
                continue;
            }

            var markdown = GetBindingText(binding);
            if (string.IsNullOrEmpty(markdown)) continue;

            var richText = MarkdownToRichTextUtility.Convert(markdown, hex.Link, hex.Header, hex.Span);
            MarkdownBindingUtility.ApplyRichText(element, richText);
        }

        MarkdownBindingUtility.ApplyContactLinks(container, hex);
    }

    private string GetBindingText(MarkdownBindings.Binding binding)
    {
        if (!string.IsNullOrEmpty(binding.rawText))
        {
            return binding.rawText;
        }

        if (!string.IsNullOrEmpty(binding.markdownPath) && File.Exists(binding.markdownPath))
        {
            return File.ReadAllText(binding.markdownPath);
        }

        Debug.LogWarning($"No valid markdown or raw text for: {binding.elementName}");
        return null;
    }

    private bool ValidateMarkdownState()
    {
        if (_uiDocument == null)
        {
            _uiDocument = Object.FindAnyObjectByType<UIDocument>();
            if (_uiDocument != null)
            {
                Debug.LogWarning($"UIDocument auto-assigned: {_uiDocument.name}");
            }
            else
            {
                Debug.LogError("UIDocument not found in the scene.");
                return false;
            }
        }

        if (_styleSettings == null)
        {
            Debug.LogError("MarkdownStyleSettings not assigned.");
            return false;
        }

        return true;
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

    private void OnEditorUpdate()
    {
        if (_pendingCapture && Application.isPlaying)
        {
            _pendingCapture = false;
            CaptureWithAppPlayAsync().Forget();
        }

        TryRenderOverlay();
    }

    private string CombinePath()
    {
        var baseName = string.Format(_nameFormat, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        return Path.Combine(_createDirectory, baseName).Replace("\\", "/");
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
        ApplyMarkdownBindings();
        _showLinksAreas = false;
        
        await UniTask.DelayFrame(2);
        
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
                Debug.LogError($"No image at path {imagePath}");
                return;
            }

            var pdfPath = Path.ChangeExtension(imagePath, ".pdf");
            PdfUtility.CreatePdfFromImage(imagePath, pdfPath, _dpi);
            PdfUtility.AddManualLinkAnnotations(pdfPath, _importContentData.Links, _dpi);

            if (_mode == Mode.OnlyPDF)
                File.Delete(imagePath);
        }

        await UniTask.Delay(1000);
        AssetDatabase.Refresh();
    }

    private void TryRenderOverlay()
    {
        if (_uiDocument == null)
        {
            _uiDocument = Object.FindAnyObjectByType<UIDocument>();
            if (_uiDocument == null)
            {
                return;
            }
        }

        var root = _uiDocument.rootVisualElement;
        var debugRects = root.Q<VisualElement>("DebugRects");
        if (debugRects != null)
        {
            root.Remove(debugRects);
        }

        if (_showLinksAreas)
        {
            var rects = _importContentData.Links.Select(linkData => linkData.rect);
            var rectsElement = UIToolkitDebugUtility.CreateDebugRects(rects, "DebugRects");
            root.Add(rectsElement);
        }
    }
}
