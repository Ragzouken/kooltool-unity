using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InfiniteDrawing : MonoBehaviour, IDrawing
{
    [SerializeField] protected Image ImageBlockPrefab;

    protected const int Size = 1024;

    protected SparseGrid<SpriteDrawing> Sprites
        = new SparseGrid<SpriteDrawing>(Size);

    protected SparseGrid<bool> Changed
        = new SparseGrid<bool>(Size);

    protected SpriteDrawing NewCell(Point cell)
    {
        var texture = BlankTexture.New(Size, Size, new Color32(0, 0, 0, 0));
        
        Sprite sprite = Sprite.Create(texture, 
                                      Rect.MinMaxRect(0, 0, Size, Size), 
                                      Vector2.one * 0.5f, 100f);
        sprite.name = string.Format("InfiniteDrawing({0}, {1})",
                                    cell.x,
                                    cell.y);

        SpriteDrawing drawing = new SpriteDrawing(sprite);

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
        SpriteDrawing drawing;
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

    public void Blit(Point offset, Sprite image, bool subtract)
    {

    }

    public void Fill(Point pixel, Color color)
    {
        SpriteDrawing drawing;
        Point grid, offset;
        
        Sprites.Coords(pixel, out grid, out offset);
        Sprites.GetDefault(grid, out drawing, NewCell);
        Changed.Set(grid, true);
        
        drawing.Fill(pixel, color);
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
            SpriteDrawing drawing;

            if (changed.Value && Sprites.Get(changed.Key, out drawing))
            {
                drawing.Apply();
            }
        }

        Changed.Clear();
    }
}
