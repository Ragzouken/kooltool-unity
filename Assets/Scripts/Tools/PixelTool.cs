using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

public class PixelTool : ITool
{
    public enum ToolMode
    {
        Pencil,
        Fill,
        Eraser,
        Picker,
        Line,
    }

    public ToolMode Tool;
    public ToolMode LastTool;

    public int Thickness = 1;
    public Color Color = Color.red;
    public Tilemap Tilemap;
    public InfiniteDrawing Drawing;

    public IDrawing Target;
    public MonoBehaviour CoroutineTarget;

    public bool dragging;
    public Vector2 start;

    public PixelTool(Tilemap tilemap, InfiniteDrawing drawing)
    {
        Tilemap = tilemap;
        Drawing = drawing;
    }

    public void BeginStroke(Vector2 start)
    {
        var cell = TileTool.Vector2Cell(start);
        Tileset.Tile tile;

        if (Tilemap.Get(cell, out tile))
        {
            Target = Tilemap;
        }
        else
        {
            Target = Drawing;
        }

        if (Tool == ToolMode.Fill)
        {
            Target.Fill(new Point(start), Color);
            Target.Apply();
        }
        else if (Tool == ToolMode.Picker)
        {
            Color sampled;
            
            if (Target.Sample(new Point(start), out sampled))
            {
                Color = sampled;
            }
        }
        else if (Tool == ToolMode.Line)
        {
            this.start = start;
        }

        dragging = true;
    }

    public void ContinueStroke(Vector2 start, Vector2 end)
    {
        if (Tool == ToolMode.Pencil
         || Tool == ToolMode.Eraser)
        {
            Color color = Color.a > 0 ? Color : Color.white;
            Blend.BlendFunction blend = Color.a == 0 ? Blend.Subtract : Blend.Alpha;

            Target.DrawLine(start, end, Thickness, color, blend);

            Target.Apply();
        }
    }

    public void EndStroke(Vector2 end)
    {
        if (Tool == ToolMode.Line)
        {
            Color color = Color.a > 0 ? Color : Color.white;
            Blend.BlendFunction blend = Color.a == 0 ? Blend.Subtract : Blend.Alpha;
            
            Target.DrawLine(this.start, end, Thickness, color, blend);
            
            Target.Apply();
        }

        dragging = false;
    }
}
