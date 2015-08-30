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
        }

        private ITileable hovering;
        private ITileable dragging;

        public kooltool.Tile paintTile;
        public Tool tool;

        private TileCursor cursor;

        public void SetToolOrReset(Tool tool)
        {
            this.tool = this.tool == tool ? Tool.Pencil : tool;
        }

        public Tile(Editor editor, TileCursor cursor) : base(editor)
        {
            paintTile = editor.project_.tileset.tiles[0];
            this.cursor = cursor;
        }

        public static Point Vector2Cell(Vector2 point)
        {
            Point cell, offset;

            var test = new SparseGrid<bool>(32);

            test.Coords(new Point(point), out cell, out offset);

            return cell;
        }

        public Serialization.TileInstance? hoveredTile
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
            tool = Tool.Pencil;

            if (Input.GetKey(KeyCode.LeftAlt)
             || Input.GetKey(KeyCode.RightAlt))
            {
                tool = Tool.Picker;
            }
            else if (Input.GetKey(KeyCode.LeftShift)
                  || Input.GetKey(KeyCode.RightShift))
            {
                tool = Tool.Fill;
            }
            else if (Input.GetKey(KeyCode.LeftControl)
                  || Input.GetKey(KeyCode.RightControl))
            {
                tool = Tool.Promote;
            }

            highlights.Clear();

            hovering = editor.hovered.OfType<ITileable>().FirstOrDefault();

            var cell = Vector2Cell(editor.currCursorWorld);
            var tile = hoveredTile;

            if (dragging != null && tool == Tool.Pencil)
            {
                var s = Vector2Cell(editor.prevCursorWorld);
                var e = Vector2Cell(editor.currCursorWorld);

                if (paintTile != null)
                {
                    PixelDraw.Bresenham.Line(s.x, s.y, e.x, e.y, (x, y) =>
                    {
                        dragging.Tilemap.Set(new Point(x, y), paintTile.Instance());

                        return true;
                    });
                }
                else
                {
                    PixelDraw.Bresenham.Line(s.x, s.y, e.x, e.y, (x, y) =>
                    {
                        dragging.Tilemap.Unset(new Point(x, y));

                        return true;
                    });
                }
            }

            cursor.Refresh();
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

            cursor.gameObject.SetActive(true);
        }

        public override void Exit()
        {
            Reset();

            cursor.gameObject.SetActive(false);
        }

        public override void CursorInteractStart()
        {
            hovering = editor.hovered.OfType<ITileable>().FirstOrDefault();

            var cell = Vector2Cell(editor.currCursorWorld);
            var tile = hoveredTile;

            if (tool == Tool.Pencil)
            {
                dragging = hovering;
            }
            else if (tool == Tool.Picker)
            {
                paintTile = tile.HasValue ? tile.Value.tile : null;
            }
            else if (tool == Tool.Fill)
            {
                hovering.Tilemap.Fill(cell, paintTile);
            }
            else if (tool == Tool.Promote)
            {
                if (tile.HasValue)
                {
                    hovering.Demote(cell);
                }
                else
                {
                    // TODO: copy tile to bg, gc tile if necc
                }
            }
        }

        public override void CursorInteractFinish()
        {
            dragging = null;
        }
    }
}
