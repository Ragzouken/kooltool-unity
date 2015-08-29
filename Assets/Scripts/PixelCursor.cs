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
        
        [SerializeField] private Image overlayIcon;

        [Header("Sprites")]
        [SerializeField] private Sprite pencilSprite;
        [SerializeField] private Sprite pickSprite;
        [SerializeField] private Sprite fillSprite;
        [SerializeField] private Sprite lineSprite;

        [HideInInspector] public Modes.Draw mode;
        [HideInInspector] public Vector2 end;

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

            overlayIcon.sprite = null;

            if (mode.tool == Modes.Draw.Tool.Pick)   overlayIcon.sprite = pickSprite;
            if (mode.tool == Modes.Draw.Tool.Pencil) overlayIcon.sprite = pencilSprite;
            if (mode.tool == Modes.Draw.Tool.Fill)   overlayIcon.sprite = fillSprite;
            if (mode.tool == Modes.Draw.Tool.Line)   overlayIcon.sprite = lineSprite;

            overlayIcon.gameObject.SetActive(overlayIcon.sprite != null);
            overlayIcon.transform.position = Input.mousePosition;

            rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mode.thickness);
            rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   mode.thickness);
        }
    }
}
