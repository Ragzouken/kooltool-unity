using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tileseter : MonoBehaviour
{
    [SerializeField] protected Drawer Drawer;
	[SerializeField] protected Tilemap Tilemap;
	[SerializeField] protected Image TilePrefab;

	protected Tileset Tileset;

	public void Start()
	{
		Tileset = Tilemap.Tileset;

		SetTiles();
	}

	public void OnEnable()
	{
		if (Tileset == null) return;

		SetTiles();
	}

	public void SetTiles()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		foreach (Tileset.Tile tile in Tileset.Tiles)
		{
			var image = Instantiate<Image>(TilePrefab);

			image.sprite = tile.Thumbnail;
			image.SetNativeSize();
			
			image.transform.SetParent(transform, false);

            var button = image.GetComponent<Button>();
            var set = tile;

            button.onClick.AddListener(delegate {
                Drawer.SetTile(set);
            });
		}
	}
}
