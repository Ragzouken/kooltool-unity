using UnityEngine;
using System.Collections.Generic;

namespace PixelDraw
{
    public class TiledDrawing : IDrawing
    {
        public SparseGrid<IDrawing> Cells;
        protected SparseGrid<bool> Changed;

        SparseGrid<IDrawing>.Constructor NewCell;

        protected bool NullCell(Point coords, out IDrawing drawing)
        {
            drawing = null;

            return false;
        }

        public TiledDrawing(Point cellsize, 
                            SparseGrid<IDrawing>.Constructor newCell = null)
        {
            Cells = new SparseGrid<IDrawing>(cellsize.x, cellsize.y);
            Changed = new SparseGrid<bool>(cellsize.x, cellsize.y);

            NewCell = newCell ?? NullCell;
        }

        public virtual void Brush(Point pixel, Sprite image, Blend.BlendFunction blend)
        {
            pixel = pixel - new Point(image.pivot);
            
            Point grid, offset;
            
            Cells.Coords(pixel, out grid, out offset);
            
            var gw = Mathf.CeilToInt((image.rect.width  + offset.x) / Cells.CellWidth);
            var gh = Mathf.CeilToInt((image.rect.height + offset.y) / Cells.CellHeight);

            for (int y = 0; y < gh; ++y)
            {
                for (int x = 0; x < gw; ++x)
                {
                    IDrawing drawing;

                    Point cell = new Point(grid.x + x, grid.y + y);
                    
                    if (Cells.GetDefault(cell, out drawing, NewCell))
                    {
                        var point = new Point(x * Cells.CellWidth, y * Cells.CellHeight);

                        drawing.Brush(offset - point + image.pivot, image, blend);

                        Changed.Set(cell, true);
                    }
                }
            }
        }

        public virtual void Fill(Point pixel, Color color)
        {
            IDrawing drawing;
            Point grid, offset;
            
            Cells.Coords(pixel, out grid, out offset);
            
            if (Cells.Get(grid, out drawing))
            {
                drawing.Fill(offset, color);
            }
        }

        public virtual bool Sample(Point pixel, out Color color)
        {
            IDrawing drawing;
            Point grid, offset;
            
            Cells.Coords(pixel, out grid, out offset);
            
            if (Cells.Get(grid, out drawing))
            {
                return drawing.Sample(offset, out color);
            }
            else
            {
                color = Color.black;
                
                return false;
            }
        }
        
        public virtual void Apply()
        {
            foreach (var changed in Changed)
            {
                IDrawing drawing;
                
                if (changed.Value && Cells.Get(changed.Key, out drawing))
                {
                    drawing.Apply();
                }
            }
            
            Changed.Clear();
        }
    }
}
