using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

public class PixelCursor : MonoBehaviour
{
    public Sprite Blank;
    public Image Preview;

    public PixelTool Tool;

    public Vector2 lstart;
    public Vector2 lend;

    public void Update()
    {
        var rtrans = transform as RectTransform;
        var ptrans = Preview.transform as RectTransform;

        if (Tool.Tool == PixelTool.ToolMode.Line && Tool.dragging)
        {
            Preview.color = Color.white;

            Vector2 start = Tool.start;
            Vector2 end = rtrans.anchoredPosition;

                var tl = new Vector2(Mathf.Min(start.x, end.x),
                                     Mathf.Min(start.y, end.y));
                
                var preview = Brush.Line(new Point(start - tl), 
                                         new Point(end - tl), 
                                         Tool.Color, Tool.Thickness);

                preview.texture.Apply();

                var pivot = new Vector2(preview.pivot.x / preview.rect.width,
                                        preview.pivot.y / preview.rect.height);
                
                ptrans.pivot = pivot;
                rtrans.anchoredPosition = start;
                
                Preview.sprite = preview;
                Preview.SetNativeSize();
        }
        else
        {
            Preview.color = Tool.Color;

            Preview.sprite = Blank;

            ptrans.anchorMin = Vector2.zero;
            ptrans.anchorMax = Vector2.one;
            ptrans.offsetMin = Vector2.zero;
            ptrans.offsetMax = Vector2.zero;
            ptrans.pivot = Vector2.one * 0.5f;

            rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Tool.Thickness);
            rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   Tool.Thickness);
        }
    }
}
