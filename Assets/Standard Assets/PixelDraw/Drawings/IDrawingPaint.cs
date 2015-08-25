﻿using UnityEngine;
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
            /*
            var tl = new Vector2(Mathf.Min(start.x, end.x),
                                 Mathf.Min(start.y, end.y));
            */

            var tl = start;

            var brush = Brush.Line(new Point(start - tl), 
                                   new Point(end - tl), 
                                   color, 
                                   thickness);

            drawing.Brush(start, brush, blend);
        }

        public static void DrawRect(this IDrawing drawing,
                                    Vector2 start, 
                                    Vector2 size,
                                    Color color,
                                    Blend.BlendFunction blend)
        {
            var brush = Brush.Rectangle((int) size.x, (int) size.y, color);          
            
            drawing.Brush(new Point(start), brush, blend);
        }

        public static void DrawCircle(this IDrawing drawing,
                                      Vector2 center,
                                      int radius,
                                      Color color,
                                      Blend.BlendFunction blend)
        {
            var brush = Brush.Circle(radius, color);

            drawing.Brush(new Point(center), brush, blend);
        }

        public static void DrawPolygon(this IDrawing drawing,
                                       IList<Point> points,
                                       Color color,
                                       Blend.BlendFunction blend)
        {
            var brush = Brush.Polygon(points, color);

            drawing.Brush(new Point(-brush.pivot), brush, blend);
        }
    }
}
