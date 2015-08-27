using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using kooltool.Serialization;

namespace kooltool.Editor.Modes
{
    public class Tile : Mode
    {
        public enum Tool
        {
            Pencil,
            Fill,
            Picker,
            Promote,
            Demote,
        }

        private ITileable hovering;
        private ITileable dragging;

        public kooltool.Tile paintTile;
        public Tool tool;

        public Tile(Editor editor, TileCursor cursor) : base(editor)
        {
            paintTile = editor.project_.tileset.tiles[0];
        }

        public static Point Vector2Cell(Vector2 point)
        {
            Point cell, offset;

            var test = new SparseGrid<bool>(32);

            test.Coords(new Point(point), out cell, out offset);

            return cell;
        }

        private Serialization.TileInstance? hoveredTile
        {
            get
            {
                editor.hovered.OfType<ITileable>().FirstOrDefault();

                Serialization.TileInstance tile;

                var cell = Vector2Cell(editor.currCursorWorld);

                bool existing = hovering.Tilemap.Get(cell, out tile);

                if (existing)
                {
                    return tile;
                }
                else
                {
                    return null;
                }
            }
        }

        public override void Update()
        {
            highlights.Clear();

            hovering = editor.hovered.OfType<ITileable>().FirstOrDefault();

            var cell = Vector2Cell(editor.currCursorWorld);
            var tile = hoveredTile;

            if (hovering != null)
            {
                var mode = tile.HasValue ? Modes.Tile.Tool.Demote
                                         : Modes.Tile.Tool.Promote;

                if (tool == Tool.Promote
                 || tool == Tool.Demote)
                {
                    tool = mode;
                }
            }

            if (dragging != null)
            {
                var s = Vector2Cell(editor.prevCursorWorld);
                var e = Vector2Cell(editor.currCursorWorld);

                if (tool == Modes.Tile.Tool.Pencil && paintTile != null)
                {
                    PixelDraw.Bresenham.Line(s.x, s.y, e.x, e.y, (x, y) =>
                    {
                        dragging.Tilemap.Set(new Point(x, y), new kooltool.Serialization.TileInstance { tile = paintTile });

                        return true;
                    });
                }
                else if (tool == Modes.Tile.Tool.Pencil && paintTile == null)
                {
                    PixelDraw.Bresenham.Line(s.x, s.y, e.x, e.y, (x, y) =>
                    {
                        dragging.Tilemap.Unset(new Point(x, y));

                        return true;
                    });
                }
            }
        }

        public void SetDrag(ITileable target)
        {
            dragging = target;
        }

        private void Reset()
        {
            highlights.Clear();
            dragging = null;
        }

        public override void Enter()
        {
            Reset();
        }

        public override void Exit()
        {
            Reset();
        }

        public override void CursorInteractStart()
        {
            hovering = editor.hovered.OfType<ITileable>().FirstOrDefault();

            var cell = Vector2Cell(editor.currCursorWorld);
            var tile = hoveredTile;

            if (tool == Modes.Tile.Tool.Pencil)
            {
                dragging = hovering;
            }
            else if (tool == Modes.Tile.Tool.Picker)
            {
                paintTile = tile.HasValue ? tile.Value.tile : null;
            }
            else if (tool == Modes.Tile.Tool.Demote)
            {
                // TODO: copy bg to new tile
                hovering.Tilemap.Unset(cell);
            }
            else if (tool == Modes.Tile.Tool.Promote)
            {
                // TODO: copy tile to bg, gc tile if necc
            }
        }

        public override void CursorInteractFinish()
        {
            dragging = null;
        }
    }
}
