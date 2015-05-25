using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

public class Tilemap : MonoDrawing
{
    protected const int Size = 32;

    [SerializeField] protected Image TilePrefab;

    protected TiledDrawing Tiled;

	protected SparseGrid<Image> Images
		= new SparseGrid<Image>(Size);
	
    protected SparseGrid<Tileset.Tile> Tiles
        = new SparseGrid<Tileset.Tile>(Size);

    protected bool NewTile(Point cell, out Image renderer)
    {
        renderer = Instantiate<Image>(TilePrefab);
        var block = renderer.gameObject;
        
		Images.Set(cell, renderer);

        block.layer = LayerMask.NameToLayer("World");
        block.transform.SetParent(transform, false);
        block.transform.localPosition = new Vector2(cell.x * Size, 
                                                    cell.y * Size);

		return true;
    }

    public void Awake()
    {
        Tiled = new TiledDrawing(new Point(Size, Size));
        Drawing = Tiled; 
    }

    public override void Apply()
    {
        Drawing.Apply();
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
