using UnityEngine;

namespace UIToolkitTools
{
    public readonly struct MarkdownColorHex
    {
        public readonly string Link;
        public readonly string Header;
        public readonly string Span;

        public MarkdownColorHex(MarkdownStyleSettings settings)
        {
            Link = $"#{ColorUtility.ToHtmlStringRGB(settings.LinkColor)}";
            Header = $"#{ColorUtility.ToHtmlStringRGB(settings.HeaderColor)}";
            Span = $"#{ColorUtility.ToHtmlStringRGB(settings.SpanColor)}";
        }
    }

}