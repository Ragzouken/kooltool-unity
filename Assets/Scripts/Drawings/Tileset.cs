using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tileset
{
    protected Texture2D Texture;

    public IList<SpriteDrawing> Tiles = new List<SpriteDrawing>();

    public Tileset()
    {
        Texture = BlankTexture.New(1024, 1024, new Color(0, 0, 0, 0));
    }

    public SpriteDrawing AddTile()
    {
        int index = Tiles.Count;

        var sprite = Sprite.Create(Texture, 
                                   new Rect(index * 32, 0, 32, 32),
                                   Vector2.one * 0.5f, 100f);

        var drawing = new SpriteDrawing(sprite);

        Tiles.Add(drawing);

        return drawing;
    }

    public void Apply()
    {
        Texture.Apply();
    }
}
