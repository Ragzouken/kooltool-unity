using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;
using kooltool.Data;

public class Tilemap : MonoDrawing
{
    protected const int Size = 32;

    [SerializeField] protected Image TilePrefab;

    protected TiledDrawing Tiled;

    protected SparseGrid<TileInstance> Tiles
        = new SparseGrid<TileInstance>(Size);

    private MonoBehaviourPooler<Point, Image> images;

    private Layer layer;

    public void Awake()
    {
        Tiled = new TiledDrawing(new Point(Size, Size));
        Drawing = Tiled;

        images = new MonoBehaviourPooler<Point, Image>(TilePrefab,
                                                       transform,
                                                       InitialiseTile);
    }

    private void InitialiseTile(Point cell, Image image)
    {
        image.gameObject.layer = LayerMask.NameToLayer("World");
        image.transform.localPosition = new Vector2(cell.x * Size,
                                                    cell.y * Size);

        image.GetComponent<kooltool.Editor.TileEditable>().drawing = Tiled;

        if (layer.tiles.ContainsKey(cell))
        {
            var tile = layer.tiles[cell].tile;

            tile.InitTest();

            image.sprite = tile.sprites[0];

            Tiled.Cells.Set(cell, new SpriteDrawing(image.sprite));
        }
    }

    public void SetLayer(Layer layer)
    {
        this.layer = layer;

        images.SetActive(layer.tiles.Keys);
    }

    public override void Apply()
    {
        Drawing.Apply();
    }

    public bool Get(Point cell, out TileInstance tile)
    {
        return layer.tiles.TryGetValue(cell, out tile);
    }

    public void Set(Point cell, TileInstance tile)
	{
        Image image = images.Get(cell);

		image.sprite = tile.tile.sprites[0];

		Tiled.Cells.Set(cell, new SpriteDrawing(tile.tile.sprites[0]));

        layer.tiles[cell] = tile;

        if (Tiles.Set(cell, tile))
        {
            GetComponent<AudioSource>().Play();
        }
    }

    public void Set(Point cell, kooltool.Tile tile)
    {
        if (tile == null)
        {
            Unset(cell);
        }
        else
        {
            Set(cell, tile.Instance());
        }
    }

    public void Unset(Point cell)
    {
        TileInstance tile;
        IDrawing drawing;

        if (Tiles.Unset(cell, out tile))
        {
            GetComponent<AudioSource>().Play();

            Tiled.Cells.Unset(cell, out drawing);
            images.Discard(cell);
        }

        if (layer.tiles.ContainsKey(cell)) layer.tiles.Remove(cell);
    }

    public void Fill(Point first, kooltool.Tile tile, int limit=128)
    {
        TileInstance existing;
        kooltool.Tile original;

        original = Get(first, out existing) ? existing.tile
                                           : null;

        var check = new Queue<Point>();

        System.Action<Point> process = delegate(Point cell)
        {
            Set(cell, tile);

            check.Enqueue(cell + Point.Right);
            check.Enqueue(cell + Point.Down);
            check.Enqueue(cell + Point.Left);
            check.Enqueue(cell + Point.Up);
        };

        process(first);

        for (int i = 0; i < limit && check.Count > 0; ++i)
        {
            Point next = check.Dequeue();

            bool exists = Get(next, out existing);

            if ((exists && existing.tile == original)
             || (!exists && original == null))
            {
                process(next);
            }
        }
    }

    public IEnumerator<KeyValuePair<Point, TileInstance>> GetEnumerator()
    {
        return Tiles.GetEnumerator();
    }
}
