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

        public kooltool.Serialization.Tile Tile { get; protected set; }

        public void SetTile(kooltool.Serialization.Tile tile)
        {
            Tile = tile;

            Image.sprite = tile.sprites[0];
            Image.SetNativeSize();
        }
    }
}
