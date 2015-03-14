using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TileTool : ITool
{
    public enum ToolMode
    {
        Pencil,
        Fill,
        Eraser,
        Picker,
    }

    public ToolMode Tool;

    public Tilemap Tilemap;
    public Tileset.Tile PaintTile;

    public TileTool(Tilemap tilemap)
    {
        Tilemap = tilemap;
        PaintTile = Tilemap.Tileset.Tiles[0];
    }

    public static Point Vector2Cell(Vector2 point)
    {
        Point cell, offset;

        var test = new SparseGrid<bool>(32);

        test.Coords(new Point(point), out cell, out offset);

        return cell;
    }

    public void BeginStroke(Vector2 start)
    {
        if (Tool == ToolMode.Fill)
        {

        }
        else if (Tool == ToolMode.Picker)
        {
            Tileset.Tile sampled;
            
            if (Tilemap.Get(Vector2Cell(start), out sampled))
            {
                PaintTile = sampled;

                Tool = ToolMode.Pencil;
            }
            else
            {
                Tool = ToolMode.Eraser;
            }
        }
    }

    public void ContinueStroke(Vector2 start, Vector2 end)
    {
        if (Tool == ToolMode.Pencil
         || Tool == ToolMode.Eraser)
        {
            Bresenham.PlotFunction plot;

            if (Tool == ToolMode.Pencil)
            {
                plot = delegate (int x, int y)
                {
                    Tilemap.Set(new Point(x, y), PaintTile);
                    
                    return true;
                };
            }
            else
            {
                plot = delegate (int x, int y)
                {
                    Tilemap.Unset(new Point(x, y));
                    
                    return true;
                };
            }

            var s = Vector2Cell(start);
            var e = Vector2Cell(end);

            Bresenham.Line(s.x, s.y, e.x, e.y, plot);
        }
        else if (Tool == ToolMode.Picker)
        {
            Tileset.Tile sampled;
            
            if (Tilemap.Get(Vector2Cell(end), out sampled))
            {
                PaintTile = sampled;
            }
            else
            {
                Tool = ToolMode.Eraser;
            }
        }
    }

    public void EndStroke(Vector2 end)
    {
    }
}
