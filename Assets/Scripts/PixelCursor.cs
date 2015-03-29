using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

public class PixelCursor : MonoBehaviour
{
    public Image Preview;
    public Image Line;

    public PixelTool Tool;
    public Vector2 end;

    public void Update()
    {
        var rtrans = transform as RectTransform;

        if (Tool.Tool == PixelTool.ToolMode.Line && Tool.dragging)
        {
            var ltrans = Line.transform as RectTransform;

            Preview.color = Color.white;

            Vector2 start = Tool.start;

            var tl = new Vector2(Mathf.Min(start.x, end.x),
                                 Mathf.Min(start.y, end.y));
            
            var preview = Brush.Line(new Point(start - tl), 
                                     new Point(end - tl), 
                                     Tool.Color, Tool.Thickness);

            preview.texture.Apply();

            var pivot = new Vector2(preview.pivot.x / preview.rect.width,
                                    preview.pivot.y / preview.rect.height);

            Line.sprite = preview;
            Line.SetNativeSize();

            ltrans.pivot = pivot;
            ltrans.anchoredPosition = start;
        }

        Preview.color = Tool.Color;

        rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Tool.Thickness);
        rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   Tool.Thickness);
    }
}
