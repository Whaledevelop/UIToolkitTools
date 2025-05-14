using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitTools
{
    public static class UIToolkitDebugUtility
    {
        public static VisualElement CreateDebugRects(IEnumerable<Rect> rects, string name = "DebugRects")
        {
            var debugRects = new VisualElement
            {
                name = name,
                pickingMode = PickingMode.Ignore,
                style =
                {
                    position = Position.Absolute,
                    top = 0,
                    left = 0,
                    right = 0,
                    bottom = 0
                }
            };
            foreach (var rect in rects)
            {
                var box = new VisualElement
                {
                    style =
                    {
                        position = Position.Absolute,
                        left = rect.x,
                        top = rect.y,
                        width = rect.width,
                        height = rect.height,
                        borderTopWidth = 2,
                        borderLeftWidth = 2,
                        borderRightWidth = 2,
                        borderBottomWidth = 2,
                        borderTopColor = Color.red,
                        borderLeftColor = Color.red,
                        borderRightColor = Color.red,
                        borderBottomColor = Color.red
                    }
                };
                debugRects.Add(box);
            }

            return debugRects;
        }
    }
}