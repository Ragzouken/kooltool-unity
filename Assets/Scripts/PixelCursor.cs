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

        public Color colour;
        public bool correct;
        public Sprite preview;

        [Header("Sprites")]
        [SerializeField] private Sprite pencilSprite;
        [SerializeField] private Sprite pickSprite;
        [SerializeField] private Sprite fillSprite;
        [SerializeField] private Sprite lineSprite;

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

            if (mode.tool == Modes.Draw.Tool.Pick)   editor.toolIcon.sprite = pickSprite;
            if (mode.tool == Modes.Draw.Tool.Pencil) editor.toolIcon.sprite = pencilSprite;
            if (mode.tool == Modes.Draw.Tool.Fill)   editor.toolIcon.sprite = fillSprite;
            if (mode.tool == Modes.Draw.Tool.Line)   editor.toolIcon.sprite = lineSprite;

            rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mode.thickness);
            rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   mode.thickness);
        }
    }
}
