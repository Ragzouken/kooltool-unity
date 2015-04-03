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

    public bool picking;

    public PixelTool(Tilemap tilemap, InfiniteDrawing drawing)
    {
        Tilemap = tilemap;
        Drawing = drawing;
    }

    public void SetErase()
    {
        Color = Color.clear;
    }

    public void BeginStroke(Vector2 start)
    {
        start = new Vector2(Mathf.Floor(start.x),
                            Mathf.Floor(start.y));

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

        dragging = false;
        picking = false;

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            Color sampled;
            
            if (Target.Sample(new Point(start), out sampled))
            {
                Color = sampled;
            }

            picking = true;
        }
        else
        {
            if (Tool == ToolMode.Fill)
            {
                Target.Fill(new Point(start), Color);
                Target.Apply();
            }
            else if (Tool == ToolMode.Line)
            {
                this.start = start;
            }

            dragging = true;
        }
    }

    public void ContinueStroke(Vector2 start, Vector2 end)
    {
        if (picking) return;

        if (Tool == ToolMode.Pencil)
        {
            Color color = Color.a > 0 ? Color : Color.white;
            var blend = Color.a == 0 ? Blend.Subtract : Blend.Alpha;

            Target.DrawLine(start, end, Thickness, color, blend);

            Target.Apply();
        }
    }

    public void EndStroke(Vector2 end)
    {
        if (picking) 
        {
            picking = false;

            return; 
        }

        if (Tool == ToolMode.Line)
        {
            Color color = Color.a > 0 ? Color : Color.white;
            var blend = Color.a == 0 ? Blend.Subtract : Blend.Alpha;
            
            Target.DrawLine(this.start, end, Thickness, color, blend);

            Target.Apply();
        }

        dragging = false;
    }
}
