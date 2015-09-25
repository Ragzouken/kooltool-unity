using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;
using kooltool.Data;

public class InfiniteDrawing : MonoDrawing
{
    [SerializeField] protected Image ImageBlockPrefab;

    protected TiledDrawing Tiled; 

    protected const int Size = 1024;

    private Layer layer;
    private Layer.Drawing drawing;

    protected void Awake()
    {
        Tiled = new TiledDrawing(new Point(Size, Size), NewCell);
        Drawing = Tiled;
    }

    protected bool NewCell(Point cell, out IDrawing drawing)
    {
        var texture = layer.world.project.index.CreateTexture(Size, Size);

        drawing = NewCell_(cell, texture);
        this.drawing.Add(cell, texture);

        return true;
    }

    private IDrawing NewCell_(Point cell, Texture2D texture)
    {
        Sprite sprite = texture.FullSprite();
        sprite.name = string.Format("InfiniteDrawing({0}, {1})",
                                    cell.x,
                                    cell.y);

        var drawing = new SpriteDrawing(sprite);

        Tiled.Cells.Set(cell, drawing);

        var renderer = Instantiate<Image>(ImageBlockPrefab);
        var block = renderer.gameObject;

        renderer.name = sprite.name;

        renderer.sprite = sprite;
        renderer.SetNativeSize();

        //block.GetComponent<kooltool.Editor.DrawingEditable>().drawing = this;

        block.transform.SetParent(transform, false);
        block.transform.localPosition = new Vector2(cell.x * Size,
                                                    cell.y * Size);

        return drawing;
    }

    public void SetLayer(Layer layer, Layer.Drawing drawing)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Tiled = new TiledDrawing(new Point(Size, Size), NewCell);
        Drawing = Tiled;

        this.layer = layer;
        this.drawing = drawing;

        foreach (var pair in drawing)
        {
            NewCell_(pair.Key, pair.Value);
        }
    }
}
