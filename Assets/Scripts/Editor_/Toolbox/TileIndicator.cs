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

        public Tile Tile { get; protected set; }

        public void SetTile(Tile tile)
        {
            Tile = tile;

            tile.InitTest();

            Image.sprite = tile.sprites[0];
            Image.SetNativeSize();
        }
    }
}
