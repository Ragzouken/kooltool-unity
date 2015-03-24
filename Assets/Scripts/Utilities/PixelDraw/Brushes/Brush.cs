using UnityEngine;

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

            Point size = (end - start).Size + new Point(thickness, thickness);
            var pivot  = start.Vector2() + Vector2.one * left;
            var anchor = new Vector2(pivot.x / size.x, pivot.y / size.y);
            var rect   = new Rect(0, 0, size.x, size.y);

            Texture2D image = BlankTexture.New(size.x, size.y, Color.clear);
            Sprite brush = Sprite.Create(image, rect, anchor);
            Sprite square = Rectangle(thickness, thickness, color, left, left);
            Sprite circle = Circle(thickness, color);

            Bresenham.PlotFunction plot = delegate (int x, int y)
            {
                Apply(circle, new Point(x, y),
                      brush,  new Point(pivot),
                      Blend.Alpha);
                
                return true;
            };

            Bresenham.Line(start.x + left, start.y + left, 
                           end.x   + left, end.y   + left, 
                           plot);
            
            return brush;
        }

        public static Sprite Rectangle(int width, int height,
                                       Color color,
                                       float px = 0, float py = 0)
        {
            Texture2D image = BlankTexture.New(width, height, color);

            Sprite brush = Sprite.Create(image, 
                                          new Rect(0, 0, width, height),
                                          new Vector2(px / width, py / height));

            return brush;
        }

        public static Sprite Circle(int diameter, Color color)
        {
            Texture2D image = BlankTexture.New(diameter, diameter, Color.clear);

            Sprite brush = Sprite.Create(image, 
                                         new Rect(0, 0, diameter, diameter),
                                         Vector2.one * 0.5f);

            int radius = diameter / 2;

            int x0 = radius;
            int y0 = radius;

            int x = radius;
            int y = 0;
            int radiusError = 1-x;
            
            while(x >= y)
            {
                for (int i = -x + x0; i <= x + x0; ++i)
                {
                    image.SetPixel(i,  y + y0, color);
                }

                for (int i = -x + x0; i <= x + x0; ++i)
                {
                    image.SetPixel(i, -y + y0, color);
                }

                for (int i = -y + y0; i <= y + y0; ++i)
                {
                    image.SetPixel(i,  x + y0, color);
                }

                for (int i = -y + y0; i <= y + y0; ++i)
                {
                    image.SetPixel(i, -x + y0, color);
                }

                y++;
                if (radiusError<0)
                {
                    radiusError += 2 * y + 1;
                }
                else
                {
                    x--;
                    radiusError += 2 * (y - x) + 1;
                }
            }

            return brush;
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

            if (activeRect.width < 1 || activeRect.height < 1)
            {
                Debug.Log(string.Format("No overlap between {0} and {1}.", world_rect_canvas, world_rect_brush));

                return;
            }

            var local_rect_brush = new Rect(activeRect.x - world_rect_brush.x + brush.textureRect.x,
                                            activeRect.y - world_rect_brush.y + brush.textureRect.y,
                                            activeRect.width,
                                            activeRect.height);

            var local_rect_canvas = new Rect(activeRect.x - world_rect_canvas.x + canvas.textureRect.x,
                                             activeRect.y - world_rect_canvas.y + canvas.textureRect.y,
                                             activeRect.width,
                                             activeRect.height);

            Apply(canvas.texture, local_rect_canvas,
                  brush.texture,  local_rect_brush,
                  blend);
        }
    }
}
