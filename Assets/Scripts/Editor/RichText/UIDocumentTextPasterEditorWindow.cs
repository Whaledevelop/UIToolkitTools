using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

public class UIDocumentTextPasterEditorWindow : OdinEditorWindow
{
    [MenuItem("Tools/UIDocument Text Paster")]
    public static void ShowWindow()
    {
        GetWindow<UIDocumentTextPasterEditorWindow>().Show();
    }

    [SerializeField]
    private UIDocument _targetDocument;

    [SerializeField]
    private string _elementName;

    [SerializeField, TextArea(10, 100)]
    private string _markdown;

    [Button("Применить в сцене")]
    private void ApplyToSceneDocument()
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
        var target = container.Q<VisualElement>(_elementName);

        if (target == null)
        {
            Debug.LogError($"Не найден элемент с name={_elementName}.");
            return;
        }

        target.Clear();

        var richText = MarkdownToRichTextUtility.Convert(_markdown);

        var textElement = new TextElement
        {
            enableRichText = true,
            text = richText,
            style =
            {
                whiteSpace = WhiteSpace.Normal
            }
        };

        target.Add(textElement);
        Debug.Log("Текст вставлен в сцену.");
    }
}