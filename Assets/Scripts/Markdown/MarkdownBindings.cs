using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MarkdownBindings
{
    public List<Binding> bindings = new();

    [Serializable]
    public class Binding
    {
        public string elementName;
        public string markdownPath;
        [TextArea(3, 10)] public string rawText;
    }

}