using System.Collections.Generic;
using System.IO;
using UIToolkitTools;
using UnityEngine;
using UnityEngine.UIElements;

public static class MarkdownBindingUtility
{
    public static bool TryGetValidElement(MarkdownBindings.Binding binding, VisualElement container, out VisualElement element)
    {
        element = null;

        if (string.IsNullOrEmpty(binding.elementName) || string.IsNullOrEmpty(binding.markdownPath))
            return false;

        element = container.Q<VisualElement>(binding.elementName);
        if (element == null)
        {
            Debug.LogWarning($"Skipped: element not found {binding.elementName}");
            return false;
        }

        if (!File.Exists(binding.markdownPath))
        {
            Debug.LogWarning($"Skipped: file not found {binding.markdownPath}");
            return false;
        }

        return true;
    }

    public static string TryReadMarkdown(string path)
    {
        return File.Exists(path) ? File.ReadAllText(path) : null;
    }

    public static void ApplyRichText(VisualElement element, string richText)
    {
        element.Clear();
        element.Add(new TextElement
        {
            enableRichText = true,
            text = richText,
            style = { whiteSpace = WhiteSpace.Normal }
        });
    }

    public static void ApplyContactLinks(VisualElement container, MarkdownColorHex hex)
    {
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

            var contactRichText = MarkdownToRichTextUtility.Convert(pair.Value, hex.Link, hex.Header, hex.Span);
            ApplyRichText(element, contactRichText);
        }
    }
}
