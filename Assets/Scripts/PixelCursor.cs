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

        public Modes.Draw mode;
        public PixelTool Tool;
        public Vector2 end;

        public void Refresh()
        {
            if (Tool == null) return;

            bool erase = mode.paintColour.a == 0;

            Color previewColor = erase ? editor.GetFlashColour() 
                                       : mode.paintColour; 

            var rtrans = transform as RectTransform;

            Line.enabled = Tool.dragging && mode.tool == Modes.Draw.Tool.Line;

            if (mode.tool == Modes.Draw.Tool.Line && Tool.dragging)
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
                var preview = Brush.Circle(mode.thickness, previewColor);
                preview.texture.Apply();

                Preview.sprite = preview;
            }

            rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mode.thickness);
            rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   mode.thickness);
        }
    }
}
