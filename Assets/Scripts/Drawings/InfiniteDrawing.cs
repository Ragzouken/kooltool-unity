using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;
using kooltool.Serialization;

public class InfiniteDrawing : MonoDrawing
{
    [SerializeField] protected Image ImageBlockPrefab;

    protected TiledDrawing Tiled; 

    protected const int Size = 1024;

    private Layer layer;

    protected void Awake()
    {
        Tiled = new TiledDrawing(new Point(Size, Size), NewCell);
        Drawing = Tiled;
    }

    protected bool NewCell(Point cell, out IDrawing drawing)
    {
        var texture = layer.world.project.index.CreateTexture(Size, Size);
        
        Sprite sprite = Sprite.Create(texture, 
                                      Rect.MinMaxRect(0, 0, Size, Size), 
                                      Vector2.zero, 100f);
        sprite.name = string.Format("InfiniteDrawing({0}, {1})",
                                    cell.x,
                                    cell.y);

        drawing = new SpriteDrawing(sprite);

        Tiled.Cells.Set(cell, drawing);

        layer.drawing.Add(cell, texture);

        var renderer = Instantiate<Image>(ImageBlockPrefab);
        var block = renderer.gameObject;
        
        renderer.sprite = sprite;
        renderer.SetNativeSize();

        block.transform.SetParent(transform, false);
        block.transform.localPosition = new Vector2(cell.x * Size, 
                                                    cell.y * Size);

        return true;
    }

    public void SetLayer(Layer layer)
    {
        this.layer = layer;

        foreach (var pair in layer.drawing)
        {
            var cell = pair.Key;
            var texture = pair.Value;

            Sprite sprite = Sprite.Create(texture,
                                      Rect.MinMaxRect(0, 0, Size, Size),
                                      Vector2.zero, 100f);
            sprite.name = string.Format("InfiniteDrawing({0}, {1})",
                                        cell.x,
                                        cell.y);

            var drawing = new SpriteDrawing(sprite);

            Tiled.Cells.Set(cell, drawing);

            var renderer = Instantiate<Image>(ImageBlockPrefab);
            var block = renderer.gameObject;

            renderer.sprite = sprite;
            renderer.SetNativeSize();

            block.transform.SetParent(transform, false);
            block.transform.localPosition = new Vector2(cell.x * Size,
                                                        cell.y * Size);
        }
    }
}
