using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tileset
{
	public enum Tiling { Single, Auto5, Auto16 }

	public class Tile
	{
		public Tileset Tileset;
		public Tiling Tiling;
		public IList<SpriteDrawing> Subtiles = new List<SpriteDrawing>();

		public Tile(Tileset tileset,
		            SpriteDrawing drawing)
		{
			Tileset = tileset;
			Tiling = Tiling.Single;
			Subtiles.Add(drawing);
		}

		public SpriteDrawing Drawing()
		{
			return Subtiles[0];
		}

		public Sprite Thumbnail
		{
			get
			{
				return Subtiles[0].Sprite;
			}
		}
	}

    protected Texture2D Texture;

    public IList<Tile> Tiles = new List<Tile>();

    public Tileset()
    {
        Texture = BlankTexture.New(1024, 1024, new Color(0, 0, 0, 0));
    }

    public Tile AddTile()
    {
        return AddTile(new Color(Random.value, Random.value, Random.value));
    }

    public Tile AddTile(Color color)
    {
        int index = Tiles.Count;

        var sprite = Sprite.Create(Texture, 
                                   new Rect(index * 32, 0, 32, 32),
                                   Vector2.one * 0.5f, 100f);

        var drawing = new SpriteDrawing(sprite);
        drawing.Fill(new Point(0, 0), color); // TODO: make efficient, don't use fill
        drawing.Apply();

		var tile = new Tile(this, drawing);

        Tiles.Add(tile);

        return tile;
    }

    public void Apply()
    {
        Texture.Apply();
    }
}
