using UnityEngine;

[CreateAssetMenu(menuName = "Markdown/Style Settings")]
public class MarkdownStyleSettings : ScriptableObject
{
    public Color LinkColor = new(0.2f, 0.54f, 0.53f);
    public Color HeaderColor = new(0f, 0.38f, 0.38f);
    public Color SpanColor = new(0f, 0.38f, 0.38f);
}