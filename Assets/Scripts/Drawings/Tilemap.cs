﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;
using kooltool.Serialization;

public class Tilemap : MonoDrawing
{
    protected const int Size = 32;

    [SerializeField] protected Image TilePrefab;

    protected TiledDrawing Tiled;

    protected SparseGrid<kooltool.Serialization.TileInstance> Tiles
        = new SparseGrid<kooltool.Serialization.TileInstance>(Size);

    private MonoBehaviourPooler<Point, Image> images;

    private kooltool.Serialization.Layer layer;

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

        if (layer.tiles.ContainsKey(cell))
        {
            var tile = layer.tiles[cell].tile;

            tile.InitTest();

            image.sprite = tile.sprites[0];

            Tiled.Cells.Set(cell, new SpriteDrawing(image.sprite));
        }
    }

    public void SetLayer(kooltool.Serialization.Layer layer)
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

    public void Unset(Point cell)
    {
        kooltool.Serialization.TileInstance tile;
        IDrawing drawing;

        if (Tiles.Unset(cell, out tile))
        {
            GetComponent<AudioSource>().Play();

            Tiled.Cells.Unset(cell, out drawing);
            images.Discard(cell);
        }

        if (layer.tiles.ContainsKey(cell)) layer.tiles.Remove(cell);
    }

    public IEnumerator<KeyValuePair<Point, kooltool.Serialization.TileInstance>> GetEnumerator()
    {
        return Tiles.GetEnumerator();
    }
}
