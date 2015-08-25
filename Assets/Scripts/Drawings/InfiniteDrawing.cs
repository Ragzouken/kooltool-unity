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

        drawing = NewCell_(cell, texture);
        layer.drawing.Add(cell, texture);

        return true;
    }

    private IDrawing NewCell_(Point cell, Texture2D texture)
    {
        Debug.Log("New drawing cell: " + cell);

        Sprite sprite = Sprite.Create(texture,
                                      new Rect(0, 0, Size, Size),
                                      Vector2.zero, 100f);
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

        block.GetComponent<kooltool.Editor.DrawingEditable>().drawing = this;

        block.transform.SetParent(transform, false);
        block.transform.localPosition = new Vector2(cell.x * Size,
                                                    cell.y * Size);

        return drawing;
    }

    public void SetLayer(Layer layer)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Tiled = new TiledDrawing(new Point(Size, Size), NewCell);
        Drawing = Tiled;

        this.layer = layer;

        foreach (var pair in layer.drawing)
        {
            Debug.Log(pair.Value.file.path + " -> " + pair.Key);

            NewCell_(pair.Key, pair.Value);
        }
    }
}
