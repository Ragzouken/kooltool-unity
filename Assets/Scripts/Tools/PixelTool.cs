using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PixelTool : ITool
{
    public enum ToolMode
    {
        Pencil,
        Fill,
        Eraser,
        Picker,
    }

    public ToolMode Tool;
    public ToolMode LastTool;

    public int Thickness = 1;
    public Color Color = Color.red;
    public Tilemap Tilemap;

    public PixelTool(Tilemap tilemap)
    {
        Tilemap = tilemap;
    }

    public void BeginStroke(Vector2 start)
    {
        if (Tool == ToolMode.Fill)
        {
            Tilemap.Fill(new Point(start), Color);
            Tilemap.Apply();
        }
        else if (Tool == ToolMode.Picker)
        {
            Color sampled;
            
            if (Tilemap.Sample(new Point(start), out sampled))
            {
                Color = sampled;
            }
        }
    }

    public void ContinueStroke(Vector2 start, Vector2 end)
    {
        if (Tool == ToolMode.Pencil
         || Tool == ToolMode.Eraser)
        {
            int left = Mathf.FloorToInt(Thickness / 2f);
            
            var tl = new Vector2(Mathf.Min(start.x, end.x),
                                 Mathf.Min(start.y, end.y));
            
            var sprite = LineBrush(new Point(start - tl), 
                                   new Point(end - tl), 
                                   Color.a > 0 ? Color : Color.white, 
                                   Thickness);

            Tilemap.Blit(new Point(tl) - new Point(left, left), sprite, Color.a == 0);
            Tilemap.Apply();
        }
    }

    public void EndStroke(Vector2 end)
    {
    }

    public static Sprite LineBrush(Point start, Point end, Color color, int thickness)
    {
        int left = Mathf.FloorToInt(thickness / 2f);
        int right = thickness - 1 - left;
        
        Point size = (end - start).Size + new Point(thickness, thickness);
        
        Texture2D brush = BlankTexture.New(size.x, size.y, 
                                           new Color32(0, 0, 0, 0));
        
        Sprite sprite = Sprite.Create(brush, 
                                      new Rect(0, 0, brush.width, brush.height), 
                                      Vector2.zero);
        
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
        
        Bresenham.Line(start.x + left, start.y + left, end.x + left, end.y + left, plot);
        
        return sprite;
    }
}
