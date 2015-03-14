using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InfiniteDrawing : MonoBehaviour, IDrawing
{
    [SerializeField] protected Image ImageBlockPrefab;

    protected const int Size = 1024;

    protected SparseGrid<IDrawing> Sprites
        = new SparseGrid<IDrawing>(Size);

    protected SparseGrid<bool> Changed
        = new SparseGrid<bool>(Size);

    protected IDrawing NewCell(Point cell)
    {
        var texture = BlankTexture.New(Size, Size, new Color32(0, 0, 0, 0));
        
        Sprite sprite = Sprite.Create(texture, 
                                      Rect.MinMaxRect(0, 0, Size, Size), 
                                      Vector2.one * 0.5f, 100f);
        sprite.name = string.Format("InfiniteDrawing({0}, {1})",
                                    cell.x,
                                    cell.y);

        var drawing = new SpriteDrawing(sprite);

        Sprites.Set(cell, drawing);
        
        //var block = new GameObject("Image Block");
        //var renderer = block.AddComponent<SpriteRenderer>();
        var renderer = Instantiate<Image>(ImageBlockPrefab);
        var block = renderer.gameObject;
        
        renderer.sprite = sprite;
        renderer.SetNativeSize();

        block.transform.SetParent(transform, false);
        block.transform.localPosition = new Vector2(cell.x * Size, 
                                                    cell.y * Size);
        
        return drawing;
    }

    public void Point(Point pixel, Color color)
    {
        IDrawing drawing;
        Point grid, offset;
        
        Sprites.Coords(pixel, out grid, out offset);
        Sprites.GetDefault(grid, out drawing, NewCell);
        Changed.Set(grid, true);

        drawing.Point(pixel, color);
    }

    public void Line(Point start, Point end, Color color)
    {
        Bresenham.PlotFunction plot = delegate (int x, int y)
        {
            Point(new Point(x, y), color);
            
            return true;
        };

        Bresenham.Line(start.x, start.y, end.x, end.y, plot);
    }

    public void Blit(Point pixel, Sprite image, bool subtract)
    {
        Point grid, offset;
        
        Sprites.Coords(pixel, out grid, out offset);
        
        var gw = Mathf.CeilToInt((image.rect.width  + offset.x) / Size);
        var gh = Mathf.CeilToInt((image.rect.height + offset.y) / Size);
        
        int bw = (int) image.rect.width;
        int bh = (int) image.rect.height;
        
        int ch = 0;
        
        for (int y = 0; y < gh; ++y)
        {
            int sy = y == 0 ? 0 : Size - offset.y;
            int sh = Size;
            
            if (y == 0) sh = Mathf.Min(sh, Size - offset.y);
            if (y == gh - 1) sh = Mathf.Min(sh, bh - ch);
            
            int cw = 0;
            
            for (int x = 0; x < gw; ++x)
            {
                IDrawing drawing;
                
                int sx = x == 0 ? 0 : Size - offset.x;
                int sw = Size;
                
                if (x == 0) sw = Mathf.Min(sw, Size - offset.x);
                if (x == gw - 1) sw = Mathf.Min(sw, bw - cw);
                
                var rect = new Rect(sx, sy, sw, sh);
                
                var slice = Sprite.Create(image.texture, rect, Vector2.zero);
                
                Sprites.GetDefault(new Point(grid.x + x, grid.y + y), out drawing, NewCell);

                drawing.Blit(new Point(x == 0 ? offset.x : 0, 
                                       y == 0 ? offset.y : 0), 
                             slice,
                             subtract);
                drawing.Apply();
                
                cw += sw;
            }
            
            ch += sh;
        }
    }

    public void Fill(Point pixel, Color color)
    {
        IDrawing drawing;
        Point grid, offset;
        
        Sprites.Coords(pixel, out grid, out offset);
        Sprites.GetDefault(grid, out drawing, NewCell);
        Changed.Set(grid, true);
        
        drawing.Fill(pixel, color);
    }

    public IEnumerator Fill(Point pixel, Color color, int chunksize)
    {
        IDrawing drawing;
        Point grid, offset;
        
        Sprites.Coords(pixel, out grid, out offset);
        Sprites.GetDefault(grid, out drawing, NewCell);
        Changed.Set(grid, true);
        
        return drawing.Fill(pixel, color, chunksize);
    }


    public bool Sample(Point pixel, out Color color)
    {
        IDrawing drawing;
        Point grid, offset;

        Sprites.Coords(pixel, out grid, out offset);

        if (Sprites.Get(grid, out drawing))
        {
            return drawing.Sample(offset, out color);
        }
        else
        {
            color = Color.black;

            return false;
        }
    }

    public void Apply()
    {
        foreach (var changed in Changed)
        {
            IDrawing drawing;

            if (changed.Value && Sprites.Get(changed.Key, out drawing))
            {
                drawing.Apply();
            }
        }

        Changed.Clear();
    }
}
