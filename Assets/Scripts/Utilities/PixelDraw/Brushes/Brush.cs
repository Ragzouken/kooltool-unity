using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace PixelDraw
{
    public class Brush
    {
        public static Sprite Line(Point start,
                                  Point end,
                                  Color color, 
                                  int thickness)
        {
            int left = Mathf.FloorToInt(thickness / 2f);
            int right = thickness - 1 - left;
            
            Point size = (end - start).Size + new Point(thickness, thickness);
            
            Texture2D brush = BlankTexture.New(size.x, size.y, 
                                               new Color32(0, 0, 0, 0));

            var pivot = start.Vector2() + Vector2.one * left;

            Sprite sprite = Sprite.Create(brush, 
                                          new Rect(0, 0, brush.width, brush.height), 
                                          new Vector2(pivot.x / brush.width, pivot.y / brush.height));

            Bresenham.PlotFunction plot = delegate (int x, int y)
            {
                for (int cy = -left; cy <= right; ++cy)
                {
                    for (int cx = -left; cx <= right; ++cx)
                    {
                        brush.SetPixel(x + cx, y + cy, color);
                    }
                }
                
                return true;
            };
            
            Bresenham.Line(start.x + left, start.y + left, 
                           end.x   + left, end.y   + left, 
                           plot);
            
            return sprite;
        }
    }
}
