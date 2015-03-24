using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace PixelDraw
{
    public static class IDrawingPaint
    {
        public static void DrawLine(this IDrawing drawing,
                                    Vector2 start, 
                                    Vector2 end, 
                                    int thickness,
                                    Color color,
                                    Blend.BlendFunction blend)
        {
            var tl = new Vector2(Mathf.Min(start.x, end.x),
                                 Mathf.Min(start.y, end.y));

            var brush = Brush.Line(new Point(start - tl), 
                                   new Point(end - tl), 
                                   color, 
                                   thickness);          

            drawing.Brush(new Point(start), brush, blend);
        }
    }
}
