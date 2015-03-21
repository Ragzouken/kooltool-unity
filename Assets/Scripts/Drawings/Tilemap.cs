using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

public class Tilemap : MonoBehaviour, IDrawing
{
    protected const int Size = 32;

    [SerializeField] protected Image TilePrefab;

    protected TiledDrawing Tiled;

	protected SparseGrid<Image> Images
		= new SparseGrid<Image>(Size);
	
    protected SparseGrid<Tileset.Tile> Tiles
        = new SparseGrid<Tileset.Tile>(Size);

	public Tileset Tileset;

    protected bool NewTile(Point cell, out Image renderer)
    {
        //var block = new GameObject("Image Block");
        //var renderer = block.AddComponent<SpriteRenderer>();
        renderer = Instantiate<Image>(TilePrefab);
        var block = renderer.gameObject;
        
		Images.Set(cell, renderer);
        
        block.transform.SetParent(transform, false);
        block.transform.localPosition = new Vector2(cell.x * Size, 
                                                    cell.y * Size);

		return true;
    }

    public void Awake()
    {
        Tileset = new Tileset();
        Tiled = new TiledDrawing(new Point(Size, Size));
    }

    public void Brush(Point pixel, Sprite image, Blend.BlendFunction blend)
    {
        Tiled.Brush(pixel, image, blend);
    }

    public void Fill(Point pixel, Color color)
    {
        Tiled.Fill(pixel, color);
    }

    public bool Sample(Point pixel, out Color color)
    {
        return Tiled.Sample(pixel, out color);
    }

    public void Apply()
    {
        Tileset.Apply();
    }

    public bool Get(Point cell, out Tileset.Tile tile)
    {
        return Tiles.Get(cell, out tile);
    }

	public void Set(Point cell, Tileset.Tile tile)
	{
		Image image;

		Images.GetDefault(cell, out image, NewTile);

		image.sprite = tile.Drawing().Sprite;

		Tiled.Cells.Set(cell, tile.Drawing());

        if (Tiles.Set(cell, tile))
        {
            GetComponent<AudioSource>().Play();
        }
    }

    public void Unset(Point cell)
    {
        Tileset.Tile tile;
        Image image;
        IDrawing drawing;

        if (Tiles.Unset(cell, out tile))
        {
            GetComponent<AudioSource>().Play();

            Tiled.Cells.Unset(cell, out drawing);
            Images.Unset(cell, out image);

            Destroy(image.gameObject);
        }
    }

    public IEnumerator<KeyValuePair<Point, Tileset.Tile>> GetEnumerator()
    {
        return Tiles.GetEnumerator();
    }
}
