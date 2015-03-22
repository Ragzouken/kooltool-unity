using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

public class InfiniteDrawing : MonoDrawing
{
    [SerializeField] protected Image ImageBlockPrefab;

    protected TiledDrawing Tiled; 

    protected const int Size = 1024;

    protected void Awake()
    {
        Tiled = new TiledDrawing(new Point(Size, Size), NewCell);
        Drawing = Tiled;
    }

    protected bool NewCell(Point cell, out IDrawing drawing)
    {
        var texture = BlankTexture.New(Size, Size, new Color32(0, 0, 0, 0));
        
        Sprite sprite = Sprite.Create(texture, 
                                      Rect.MinMaxRect(0, 0, Size, Size), 
                                      Vector2.one * 0.5f, 100f);
        sprite.name = string.Format("InfiniteDrawing({0}, {1})",
                                    cell.x,
                                    cell.y);

        drawing = new SpriteDrawing(sprite);

        Tiled.Cells.Set(cell, drawing);

        var renderer = Instantiate<Image>(ImageBlockPrefab);
        var block = renderer.gameObject;
        
        renderer.sprite = sprite;
        renderer.SetNativeSize();

        block.transform.SetParent(transform, false);
        block.transform.localPosition = new Vector2(cell.x * Size, 
                                                    cell.y * Size);
        
        return true;
    }
}
