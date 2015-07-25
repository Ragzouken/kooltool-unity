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

    protected SparseGrid<kooltool.Serialization.TileInstance> Tiles
        = new SparseGrid<kooltool.Serialization.TileInstance>(Size);

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

    public void SetLayer(kooltool.Serialization.Layer layer)
    {

    }

    public override void Apply()
    {
        Drawing.Apply();
    }

    public bool Get(Point cell, out kooltool.Serialization.TileInstance tile)
    {
        return Tiles.Get(cell, out tile);
    }

    public void Set(Point cell, kooltool.Serialization.TileInstance tile)
	{
		Image image;

		Images.GetDefault(cell, out image, NewTile);

		image.sprite = tile.tile.sprites[0];

		Tiled.Cells.Set(cell, new SpriteDrawing(tile.tile.sprites[0]));

        if (Tiles.Set(cell, tile))
        {
            GetComponent<AudioSource>().Play();
        }
    }

    public void Unset(Point cell)
    {
        kooltool.Serialization.TileInstance tile;
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

    public IEnumerator<KeyValuePair<Point, kooltool.Serialization.TileInstance>> GetEnumerator()
    {
        return Tiles.GetEnumerator();
    }

    public kooltool.Serialization.Layer.Grid Serialize(kooltool.Serialization.Index index)
    {
        var tilemap = new kooltool.Serialization.Layer.Grid();
        
        foreach (var pair in Tiles)
        {
            tilemap.Add(pair.Key, pair.Value);
        }

        return tilemap;
    }
}
