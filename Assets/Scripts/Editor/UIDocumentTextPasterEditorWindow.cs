using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.IO;

public class UIDocumentTextPasterEditorWindow : OdinEditorWindow
{
    [MenuItem("Tools/UIDocument Text Paster")]
    public static void ShowWindow()
    {
        GetWindow<UIDocumentTextPasterEditorWindow>().Show();
    }
    [SerializeField] private MarkdownStyleSettings _styleSettings;
    [TableList] public List<MarkdownBindings.Binding> Bindings = new();

    [SerializeField, HideInInspector]
    private UIDocument _targetDocument;
    
    [Button("Load Bindings")]
    private void LoadBindings()
    {
        Bindings = MarkdownBindingStorage.Load().bindings;
        Debug.Log($"Loaded bindings: {Bindings.Count}");
    }

    [Button("Save Bindings")]
    private void SaveBindings()
    {
        var data = new MarkdownBindings { bindings = Bindings };
        MarkdownBindingStorage.Save(data);
        Debug.Log("Bindings saved.");
    }

    [Button("Apply to Scene")]
    private void ApplyAll()
    {
        if (_targetDocument == null)
        {
            var found = FindAnyObjectByType<UIDocument>();
            if (found != null)
            {
                _targetDocument = found;
                Debug.LogWarning($"UIDocument was not assigned. Found automatically: {_targetDocument.name}");
            }
            else
            {
                Debug.LogError("UIDocument not found in the scene.");
                return;
            }
        }

        if (_styleSettings == null)
        {
            Debug.LogError("MarkdownStyleSettings not assigned.");
            return;
        }

        var container = _targetDocument.rootVisualElement;

        var linkHex = $"#{ColorUtility.ToHtmlStringRGB(_styleSettings.LinkColor)}";
        var headerHex = $"#{ColorUtility.ToHtmlStringRGB(_styleSettings.HeaderColor)}";
        var spanHex = $"#{ColorUtility.ToHtmlStringRGB(_styleSettings.SpanColor)}";

        foreach (var binding in Bindings)
        {
            if (string.IsNullOrEmpty(binding.elementName) || string.IsNullOrEmpty(binding.markdownPath))
                continue;

            var target = container.Q<VisualElement>(binding.elementName);
            if (target == null)
            {
                Debug.LogWarning($"Skipped: element not found {binding.elementName}");
                continue;
            }

            if (!File.Exists(binding.markdownPath))
            {
                Debug.LogWarning($"Skipped: file not found {binding.markdownPath}");
                continue;
            }

            var md = File.ReadAllText(binding.markdownPath);
            var richText = MarkdownToRichTextUtility.Convert(md, linkHex, headerHex, spanHex);
            ApplyLink(target, richText);
        }

        var contactLinks = new Dictionary<string, string>
        {
            { "telegramLink", "[https://t.me/musli_kraba](https://t.me/musli_kraba) (preferred)" },
            { "githubLink", "[https://github.com/Whaledevelop](https://github.com/Whaledevelop)" },
            { "gmailLink", "[whaledevelop@gmail.com](mailto:whaledevelop@gmail.com)" },
            { "linkedinLink", "[https://www.linkedin.com/in/nikita-serebriakov-897644261/](https://www.linkedin.com/in/nikita-serebriakov-897644261/)" }
        };

        foreach (var pair in contactLinks)
        {
            var element = container.Q<VisualElement>(pair.Key);
            if (element == null)
            {
                Debug.LogWarning($"Link not found: {pair.Key}");
                continue;
            }

            var contactRichText = MarkdownToRichTextUtility.Convert(pair.Value, linkHex, headerHex, spanHex);
            ApplyLink(element, contactRichText);
        }
    }

    private void ApplyLink(VisualElement element, string richText)
    {
        element.Clear();
        element.Add(new TextElement
        {
            enableRichText = true,
            text = richText,
            style = { whiteSpace = WhiteSpace.Normal }
        });
    }
}
