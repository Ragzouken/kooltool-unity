using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool.Editor
{
    public class Layer : MonoDrawing
    {
        [SerializeField] protected Editor Editor;

        public InfiniteDrawing Drawing;
        public Tilemap Tilemap;

        public IDrawing DrawingUnderPoint(Point point)
        {
            Point cell, offset;

            Editor.Project.Grid.Coords(point, out cell, out offset);

            Tileset.Tile tile;
            
            if (Tilemap.Get(cell, out tile))
            {
                return Tilemap;
            }
            else
            {
                return Drawing;
            }
        }
    }
}
