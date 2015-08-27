using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool.Editor
{
    public class PixelCursor : MonoBehaviour
    {
        public Editor editor;

        public Image Preview;
        public Image Line;

        public PixelTool Tool;
        public Vector2 end;

        public void Refresh()
        {
            if (Tool == null) return; 

            Color previewColor = Tool.Color.a == 0 ? editor.GetFlashColour() : Tool.Color; 

            var rtrans = transform as RectTransform;

            Line.enabled = Tool.dragging && Tool.Tool == PixelTool.ToolMode.Line;

            if (Tool.Tool == PixelTool.ToolMode.Line && Tool.dragging)
            {
                var ltrans = Line.transform as RectTransform;

                Preview.color = Color.white;

                Vector2 start = Tool.start;

                var tl = new Vector2(Mathf.Min(start.x, end.x),
                                     Mathf.Min(start.y, end.y));
                
                var preview = Brush.Line(new Point(start - tl), 
                                         new Point(end - tl), 
                                         previewColor, Tool.Thickness);

                preview.texture.Apply();

                var pivot = new Vector2(preview.pivot.x / preview.rect.width,
                                        preview.pivot.y / preview.rect.height);

                Line.sprite = preview;
                Line.SetNativeSize();

                ltrans.pivot = pivot;
                ltrans.anchoredPosition = start;
            }
            else
            {
                var preview = Brush.Circle(Tool.Thickness, previewColor);
                preview.texture.Apply();

                Preview.sprite = preview;
            }

            rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Tool.Thickness);
            rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   Tool.Thickness);
        }
    }
}
