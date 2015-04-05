using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool.Editor
{
    public class TileIndicator : MonoBehaviour
    {
        [SerializeField] protected Image Image;
        public Toggle Toggle;

        public Tileset.Tile Tile { get; protected set; }

        public void SetTile(Tileset.Tile tile)
        {
            Tile = tile;

            Image.sprite = tile.Thumbnail;
            Image.SetNativeSize();
        }
    }
}
