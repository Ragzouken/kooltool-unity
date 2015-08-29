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

            bool lining = mode.tool == Modes.Draw.Tool.Line && mode.drawing != null && mode.brush != null;

            Line.enabled = lining;

            if (lining)
            {
                var ltrans = Line.transform as RectTransform;

                Preview.color = Color.white;

                Line.sprite = mode.brush;
                Line.SetNativeSize();

                ltrans.position = -mode.brush.pivot;
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
