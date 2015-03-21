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

        public static Rect Intersect(Rect a, Rect b)
        {
            return Rect.MinMaxRect(Mathf.Max(a.min.x, b.min.x),
                                   Mathf.Max(a.min.y, b.min.y),
                                   Mathf.Min(a.max.x, b.max.x),
                                   Mathf.Min(a.max.y, b.max.y));
        }

        public static void Apply(Texture2D canvas, Rect canvasRect,
                                 Texture2D brush,  Rect brushRect,
                                 Blend.BlendFunction blend)
        {
            Color[] canvasColors = canvas.GetPixelRect(canvasRect);
            Color[] brushColors  = brush.GetPixelRect(brushRect);

            Assert.True(canvasColors.Length == brushColors.Length, "Mismatched texture rects!");

            for (int i = 0; i < canvasColors.Length; ++i)
            {
                canvasColors[i] = blend(canvasColors[i], brushColors[i]);
            }

            canvas.SetPixelRect(canvasRect, canvasColors);
        }

        public static void Apply(Sprite brush,  Point brushPosition,
                                 Sprite canvas, Point canvasPosition,
                                 Blend.BlendFunction blend)
        {
            var b_offset = brushPosition - brush.pivot;
            var c_offset = canvasPosition - canvas.pivot;

            var world_rect_brush = new Rect(b_offset.x,
                                            b_offset.y,
                                            brush.rect.width,
                                            brush.rect.height);

            var world_rect_canvas = new Rect(c_offset.x,
                                             c_offset.y,
                                             canvas.rect.width,
                                             canvas.rect.height);

            var activeRect = Intersect(world_rect_brush, world_rect_canvas);
        }
    }
}
