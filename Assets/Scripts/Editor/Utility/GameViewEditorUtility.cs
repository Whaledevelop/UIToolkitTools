using UnityEditor;
using System;
using System.Reflection;

public static class GameViewEditorUtility
{
    public static void SetGameViewSize(int width, int height)
    {
        var gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var window = EditorWindow.GetWindow(gameViewType);

        var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singletonType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instanceProp = singletonType.GetProperty("instance");
        var instance = instanceProp.GetValue(null, null);
        var currentGroupProp = sizesType.GetProperty("currentGroupType");
        var groupEnum = currentGroupProp.GetValue(instance, null);

        var getGroup = sizesType.GetMethod("GetGroup");
        var group = getGroup.Invoke(instance, new[] { groupEnum });

        var addCustomSize = group.GetType().GetMethod("AddCustomSize");
        var gameViewSizeType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
        var gameViewSizeTypeEnum = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");

        var ctor = gameViewSizeType.GetConstructor(new[] { gameViewSizeTypeEnum, typeof(int), typeof(int), typeof(string) });
        var newSize = ctor.Invoke(new object[] { Enum.Parse(gameViewSizeTypeEnum, "FixedResolution"), width, height, $"{width}x{height}" });

        addCustomSize.Invoke(group, new object[] { newSize });

        var selectedSizeIndexProp = gameViewType.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var getCountMethod = group.GetType().GetMethod("GetTotalCount");
        var count = (int)getCountMethod.Invoke(group, null);

        selectedSizeIndexProp.SetValue(window, count - 1);
    }
}