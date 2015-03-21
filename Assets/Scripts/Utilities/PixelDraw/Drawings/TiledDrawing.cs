using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace PixelDraw
{
    public class TiledDrawing : IDrawing
    {
        public SparseGrid<IDrawing> Cells;
        protected SparseGrid<bool> Changed;

        SparseGrid<IDrawing>.Constructor NewCell;

        public TiledDrawing(Point cellsize, 
                            SparseGrid<IDrawing>.Constructor newCell = null)
        {
            Cells = new SparseGrid<IDrawing>(cellsize.x, cellsize.y);
            Changed = new SparseGrid<bool>(cellsize.x, cellsize.y);

            NewCell = newCell ?? delegate (Point coords, out IDrawing drawing)
            {
                drawing = null;

                return false;
            };
        }

        public virtual void Brush(Point pixel, Sprite image, Blend.BlendFunction blend)
        {
            pixel = pixel - new Point(image.pivot);
            
            Point grid, offset;
            
            Cells.Coords(pixel, out grid, out offset);
            
            var gw = Mathf.CeilToInt((image.rect.width  + offset.x) / Cells.CellWidth);
            var gh = Mathf.CeilToInt((image.rect.height + offset.y) / Cells.CellHeight);
            
            int bw = (int) image.rect.width;
            int bh = (int) image.rect.height;
            
            int ch = 0;
            
            for (int y = 0; y < gh; ++y)
            {
                int sy = y == 0 ? 0 : Cells.CellHeight - offset.y;
                int sh = Cells.CellHeight;
                
                if (y == 0) sh = Mathf.Min(sh, Cells.CellHeight - offset.y);
                if (y == gh - 1) sh = Mathf.Min(sh, bh - ch);
                
                int cw = 0;
                
                for (int x = 0; x < gw; ++x)
                {
                    IDrawing drawing;
                    
                    int sx = x == 0 ? 0 : Cells.CellWidth - offset.x;
                    int sw = Cells.CellWidth;
                    
                    if (x == 0) sw = Mathf.Min(sw, Cells.CellWidth - offset.x);
                    if (x == gw - 1) sw = Mathf.Min(sw, bw - cw);
                    
                    var rect = new Rect(sx, sy, sw, sh);
                    
                    var slice = Sprite.Create(image.texture, rect, Vector2.zero);
                    
                    Cells.GetDefault(new Point(grid.x + x, grid.y + y), out drawing, NewCell);
                    
                    drawing.Brush(new Point(x == 0 ? offset.x : 0, 
                                            y == 0 ? offset.y : 0), 
                                  slice,
                                  blend);
                    drawing.Apply();
                    
                    cw += sw;
                }
                
                ch += sh;
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
