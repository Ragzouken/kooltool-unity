using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;

namespace kooltool.Editor
{
    public class TileIndicator : MonoBehaviour,
                                 IBeginDragHandler,
                                 IEndDragHandler
    {
        [SerializeField] protected Image Image;
        public Toggle Toggle;

        public kooltool.Data.Tile Tile { get; protected set; }

        private Toolbox toolbox;

        public void SetTile(kooltool.Data.Tile tile, Toolbox toolbox)
        {
            this.toolbox = toolbox;

            Tile = tile;

            tile.InitTest();

            Image.sprite = tile.sprites[0];
            Image.SetNativeSize();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            toolbox.BeginDrag(Tile, Tile.sprites[0]);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            toolbox.CancelDrag();
        }
    }
}
