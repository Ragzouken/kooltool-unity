using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tilemap : MonoBehaviour, IDrawing
{
    protected const int Size = 32;

    [SerializeField] protected Image TilePrefab;

    protected SparseGrid<SpriteDrawing> Sprites
        = new SparseGrid<SpriteDrawing>(Size);

    protected Tileset Tileset;

    protected void NewTile(Point cell, SpriteDrawing drawing)
    {
        Sprites.Set(cell, drawing);
        
        //var block = new GameObject("Image Block");
        //var renderer = block.AddComponent<SpriteRenderer>();
        var renderer = Instantiate<Image>(TilePrefab);
        var block = renderer.gameObject;
        
        renderer.sprite = drawing.Sprite;
        renderer.SetNativeSize();
        
        block.transform.SetParent(transform, false);
        block.transform.localPosition = new Vector2(cell.x * Size, 
                                                    cell.y * Size);
    }

    public void Awake()
    {
        Tileset = new Tileset();

        Tileset.AddTile();
        Tileset.AddTile();
        Tileset.AddTile();

        for (int y = 0; y < 16; ++y)
        {
            for (int x = 0; x < 32; ++x)
            {
                SpriteDrawing drawing = Tileset.Tiles[Random.Range(0, Tileset.Tiles.Count)];

                NewTile(new Point(x, y), drawing);
            }
        }
    }

    public void Point(Point pixel, Color color)
    {
        SpriteDrawing drawing;
        Point grid, offset;
        
        Sprites.Coords(pixel, out grid, out offset);
        
        if (Sprites.Get(grid, out drawing))
        {
            drawing.Point(offset, color);
        }
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

    public void Blit(Point pixel, Sprite image, bool subtract = false)
    {
        Point grid, offset;

        Sprites.Coords(pixel, out grid, out offset);

        var gw = Mathf.CeilToInt((image.rect.width  + offset.x) / Size);
        var gh = Mathf.CeilToInt((image.rect.height + offset.y) / Size);

        int bw = (int) image.rect.width;
        int bh = (int) image.rect.height;

        int iw = Mathf.Max(0, gw - 2);
        int ih = Mathf.Max(0, gh - 2);

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
                SpriteDrawing drawing;

                int sx = x == 0 ? 0 : Size - offset.x;
                int sw = Size;

                if (x == 0) sw = Mathf.Min(sw, Size - offset.x);
                if (x == gw - 1) sw = Mathf.Min(sw, bw - cw);

                var rect = new Rect(sx, sy, sw, sh);

                var slice = Sprite.Create(image.texture, rect, Vector2.zero);

                if (Sprites.Get(new Point(grid.x + x,
                                          grid.y + y), out drawing))
                {
                    drawing.Blit(new Point(x == 0 ? offset.x : 0, 
                                           y == 0 ? offset.y : 0), 
                                 slice,
                                 subtract);
                }

                cw += sw;
            }

            ch += sh;
        }
    }

    public void Fill(Point pixel, Color color)
    {
        SpriteDrawing drawing;
        Point grid, offset;
        
        Sprites.Coords(pixel, out grid, out offset);
        
        if (Sprites.Get(grid, out drawing))
        {
            drawing.Fill(offset, color);
        }
    }

    public bool Sample(Point pixel, out Color color)
    {
        SpriteDrawing drawing;
        Point grid, offset;

        Sprites.Coords(pixel, out grid, out offset);

        if (Sprites.Get(grid, out drawing))
        {
            return drawing.Sample(offset, out color);
        }

        color = Color.black;

        return false;
    }

    public void Apply()
    {
        Tileset.Apply();
    }
}
