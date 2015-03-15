using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tilemap : MonoBehaviour, IDrawing
{
    protected const int Size = 32;

    [SerializeField] protected Image TilePrefab;

    protected SparseGrid<IDrawing> Sprites
        = new SparseGrid<IDrawing>(Size);

	protected SparseGrid<Image> Images
		= new SparseGrid<Image>(Size);
	
    protected SparseGrid<Tileset.Tile> Tiles
        = new SparseGrid<Tileset.Tile>(Size);

	public Tileset Tileset;

    protected Image NewTile(Point cell)
    {
        //var block = new GameObject("Image Block");
        //var renderer = block.AddComponent<SpriteRenderer>();
        var renderer = Instantiate<Image>(TilePrefab);
        var block = renderer.gameObject;
        
		Images.Set(cell, renderer);
        
        block.transform.SetParent(transform, false);
        block.transform.localPosition = new Vector2(cell.x * Size, 
                                                    cell.y * Size);

		return renderer;
    }

    public void Awake()
    {
        Tileset = new Tileset();
    }

    public void Blit(Point pixel, Sprite image, bool subtract = false)
    {
        Point grid, offset;

        Sprites.Coords(pixel, out grid, out offset);

        var gw = Mathf.CeilToInt((image.rect.width  + offset.x) / Size);
        var gh = Mathf.CeilToInt((image.rect.height + offset.y) / Size);

        int bw = (int) image.rect.width;
        int bh = (int) image.rect.height;

        int ch = 0;

        for (int y = 0; y < gh; ++y)
        {
            int sy = y == 0 ? 0 : Size - offset.y;
            int sh = Size;

            if (y == 0) sh = Mathf.Min(sh, Size - offset.y);
            if (y == gh - 1) sh = Mathf.Min(sh, bh - ch);

            int cw = 0;

            for (int x = 0; x < gw; ++x)
            {
				IDrawing drawing;
				
				int sx = x == 0 ? 0 : Size - offset.x;
                int sw = Size;

                if (x == 0) sw = Mathf.Min(sw, Size - offset.x);
                if (x == gw - 1) sw = Mathf.Min(sw, bw - cw);

                var rect = new Rect(sx, sy, sw, sh);

                var slice = Sprite.Create(image.texture, rect, Vector2.zero);

                if (Sprites.Get(new Point(grid.x + x,
                                          grid.y + y), out drawing))
                {
                    drawing.Blit(new Point(x == 0 ? offset.x : 0, 
                                           y == 0 ? offset.y : 0), 
                                 slice,
                                 subtract);
                }

                cw += sw;
            }

            ch += sh;
        }
    }

    public void Fill(Point pixel, Color color)
    {
		IDrawing drawing;
        Point grid, offset;
        
        Sprites.Coords(pixel, out grid, out offset);
        
        if (Sprites.Get(grid, out drawing))
        {
            drawing.Fill(offset, color);
        }
    }

    public IEnumerator Fill(Point pixel, Color color, int chunksize)
    {
        IDrawing drawing;
        Point grid, offset;
        
        Sprites.Coords(pixel, out grid, out offset);
        
        if (Sprites.Get(grid, out drawing))
        {
            IEnumerator e = drawing.Fill(offset, color, chunksize);

            while (e.MoveNext()) yield return e.Current;
        }
    }

    public bool Sample(Point pixel, out Color color)
    {
		IDrawing drawing;
        Point grid, offset;

        Sprites.Coords(pixel, out grid, out offset);

        if (Sprites.Get(grid, out drawing))
        {
            return drawing.Sample(offset, out color);
        }

        color = Color.black;

        return false;
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

		Sprites.Set(cell, tile.Drawing());

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

            Sprites.Unset(cell, out drawing);
            Images.Unset(cell, out image);

            Destroy(image.gameObject);
        }
    }

    public IEnumerator<KeyValuePair<Point, Tileset.Tile>> GetEnumerator()
    {
        return Tiles.GetEnumerator();
    }
}
