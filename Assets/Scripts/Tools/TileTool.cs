using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class TileTool : ITool
    {
        public enum ToolMode
        {
            Pencil,
            Fill,
            Eraser,
            Picker,
        }

        protected Editor Editor;

        public ToolMode Tool;

        public kooltool.Tile PaintTile;

        public TileTool(Editor editor)
        {
            Editor = editor;

            PaintTile = Editor.project_.tileset.tiles[0];
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
                kooltool.Serialization.TileInstance sampled;
                
                if (Editor.Layer.Tilemap.Get(Vector2Cell(start), out sampled))
                {
                    PaintTile = sampled.tile;

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
                PixelDraw.Bresenham.PlotFunction plot;

                if (Tool == ToolMode.Pencil)
                {
                    plot = delegate (int x, int y)
                    {
                        Editor.Layer.Tilemap.Set(new Point(x, y), new kooltool.Serialization.TileInstance { tile = PaintTile });
                        
                        return true;
                    };
                }
                else
                {
                    plot = delegate (int x, int y)
                    {
                        Editor.Layer.Tilemap.Unset(new Point(x, y));
                        
                        return true;
                    };
                }

                var s = Vector2Cell(start);
                var e = Vector2Cell(end);

                PixelDraw.Bresenham.Line(s.x, s.y, e.x, e.y, plot);
            }
            else if (Tool == ToolMode.Picker)
            {
                kooltool.Serialization.TileInstance sampled;
                
                if (Editor.Layer.Tilemap.Get(Vector2Cell(end), out sampled))
                {
                    PaintTile = sampled.tile;
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
}
