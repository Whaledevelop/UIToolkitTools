using System.Collections.Generic;

[System.Serializable]
public class MarkdownBindings
{
    public List<Binding> bindings = new();

    [System.Serializable]
    public class Binding
    {
        public string elementName;
        public string markdownPath;
    }
}