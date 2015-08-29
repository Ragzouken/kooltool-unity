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
        public Vector2 end;

        public void Refresh()
        {
            if (mode == null) return;

            bool erase = mode.paintColour.a == 0;

            Color previewColor = erase ? editor.GetFlashColour() 
                                       : mode.paintColour; 

            var rtrans = transform as RectTransform;

            bool lining = mode.tool == Modes.Draw.Tool.Line && mode.drawing != null;

            Line.enabled = lining;

            if (lining)
            {
                var ltrans = Line.transform as RectTransform;

                Preview.color = Color.white;

                Vector2 start = mode.start;

                var tl = new Vector2(Mathf.Min(start.x, end.x),
                                     Mathf.Min(start.y, end.y));
                
                var preview = Brush.Line(new Point(start - tl), 
                                         new Point(end - tl), 
                                         previewColor, mode.thickness);

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
