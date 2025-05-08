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

    [SerializeField]
    private UIDocument _targetDocument;

    [TitleGroup("Цвета Rich Text")]
    [ColorUsage(true, true)]
    public Color LinkColor = new(0.2f, 0.54f, 0.53f); // #338a87

    [ColorUsage(true, true)]
    public Color HeaderColor = new(0f, 0.38f, 0.38f); // #006160

    [ColorUsage(true, true)]
    public Color SpanColor = new(0f, 0.38f, 0.38f); // #006160
    
    [ColorUsage(true, true)]
    public Color ContactLinkColor = new(0f, 0.38f, 0.38f); // #006160

    [TableList]
    public List<MarkdownBindings.Binding> Bindings = new();

    [Button("Загрузить биндинги")]
    private void LoadBindings()
    {
        Bindings = MarkdownBindingStorage.Load().bindings;
        Debug.Log($"Загружено биндингов: {Bindings.Count}");
    }

    [Button("Сохранить биндинги")]
    private void SaveBindings()
    {
        var data = new MarkdownBindings { bindings = Bindings };
        MarkdownBindingStorage.Save(data);
        Debug.Log("Биндинги сохранены.");
    }

    [Button("Применить в сцене")]
    private void ApplyAll()
    {
        if (_targetDocument == null)
        {
            var found = FindAnyObjectByType<UIDocument>();
            if (found != null)
            {
                _targetDocument = found;
                Debug.LogWarning($"UIDocument не был назначен. Автоматически найден: {_targetDocument.name}");
            }
            else
            {
                Debug.LogError("UIDocument не найден в сцене.");
                return;
            }
        }

        var container = _targetDocument.rootVisualElement;

        string linkHex = $"#{ColorUtility.ToHtmlStringRGB(LinkColor)}";
        string headerHex = $"#{ColorUtility.ToHtmlStringRGB(HeaderColor)}";
        string spanHex = $"#{ColorUtility.ToHtmlStringRGB(SpanColor)}";

        foreach (var binding in Bindings)
        {
            if (string.IsNullOrEmpty(binding.elementName) || string.IsNullOrEmpty(binding.markdownPath))
                continue;

            var target = container.Q<VisualElement>(binding.elementName);
            if (target == null)
            {
                Debug.LogWarning($"Пропущен: не найден элемент {binding.elementName}");
                continue;
            }

            if (!File.Exists(binding.markdownPath))
            {
                Debug.LogWarning($"Пропущен: файл не найден {binding.markdownPath}");
                continue;
            }

            var md = File.ReadAllText(binding.markdownPath);
            var richText = MarkdownToRichTextUtility.Convert(md, linkHex, headerHex, spanHex);

            target.Clear();
            target.Add(new TextElement
            {
                enableRichText = true,
                text = richText,
                style = { whiteSpace = WhiteSpace.Normal }
            });
        }
        
        var contactLinkColor = $"#{ColorUtility.ToHtmlStringRGB(ContactLinkColor)}";

        ApplyContactLinks(container, contactLinkColor, headerHex, spanHex);
        Debug.Log("Все биндинги применены.");
    }

    private void ApplyContactLinks(VisualElement container, string linkHex, string headerHex, string spanHex)
    {
        var contactBindings = new Dictionary<string, string>
        {
            { "telegramLink", "[telegram](https://t.me/musli_kraba) (предпочтительно)" },
            { "githubLink", "[github](https://github.com/Whaledevelop)" },
            { "gmailLink", "[gmail](mailto:whaledevelop@gmail.com)" },
            { "linkedinLink", "[linkedin](https://www.linkedin.com/in/nikita-serebriakov-897644261/)" }
        };

        foreach (var pair in contactBindings)
        {
            var element = container.Q<VisualElement>(pair.Key);
            if (element == null)
            {
                Debug.LogWarning($"Контакт не найден: {pair.Key}");
                continue;
            }

            element.Clear();
            element.Add(new TextElement
            {
                enableRichText = true,
                text = MarkdownToRichTextUtility.Convert(pair.Value, linkHex, headerHex, spanHex),
                style = { whiteSpace = WhiteSpace.Normal }
            });
        }
    }
}
