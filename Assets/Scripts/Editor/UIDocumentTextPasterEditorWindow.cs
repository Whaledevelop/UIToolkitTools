using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.IO;
using UIToolkitTools;

public class UIDocumentTextPasterEditorWindow : OdinEditorWindow
{
    [MenuItem("Tools/UIDocument Text Paster")]
    public static void ShowWindow()
    {
        GetWindow<UIDocumentTextPasterEditorWindow>().Show();
    }

    [SerializeField] private MarkdownStyleSettings _styleSettings;
    [SerializeField, HideInInspector] private UIDocument _targetDocument;

    [TableList] public List<MarkdownBindings.Binding> Bindings = new();

    [Button("Load Bindings")]
    private void LoadBindings()
    {
        Bindings = MarkdownBindingStorage.Load().bindings;
    }

    [Button("Save Bindings")]
    private void SaveBindings()
    {
        var data = new MarkdownBindings { bindings = Bindings };
        MarkdownBindingStorage.Save(data);
    }

    [Button("Apply to Scene")]
    private void ApplyAll()
    {
        if (!ValidateState()) return;

        var container = _targetDocument.rootVisualElement;
        var hex = new MarkdownColorHex(_styleSettings);

        foreach (var binding in Bindings)
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
    
    private bool ValidateState()
    {
        if (_targetDocument == null)
        {
            var found = Object.FindAnyObjectByType<UIDocument>();
            if (found != null)
            {
                _targetDocument = found;
                Debug.LogWarning($"UIDocument was not assigned. Found automatically: {_targetDocument.name}");
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
}
