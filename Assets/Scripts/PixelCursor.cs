using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool.Editor
{
    public class PixelCursor : MonoBehaviour
    {
        [SceneOnly] public Editor editor;
        
        public Image Preview;
        public Image Line;

        public Color colour;
        public bool correct;
        public Sprite preview;

        [HideInInspector] public Modes.Draw mode;

        public void Refresh()
        {
            if (mode == null) return;

            var rtrans = transform as RectTransform;

            {
                var ltrans = Line.transform as RectTransform;

                Preview.color = Color.white;

                Line.sprite = preview;
                Line.SetNativeSize();

                if (correct)
                {
                    ltrans.position = -preview.pivot;
                }
                else
                {
                    Vector2 pos = (Vector2) rtrans.position - preview.pivot + Vector2.one * 0.5f;

                    if (preview.rect.width == 1) pos = rtrans.position;

                    ltrans.position = pos.Round();
                }
            }

            rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mode.thickness);
            rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   mode.thickness);
        }
    }
}
