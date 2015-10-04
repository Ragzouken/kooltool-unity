using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class TileTab : MonoBehaviour
    {
        [Header("Tools")]
        [SerializeField] protected Button NewButton;

        [Header("Tiles")]
        [SerializeField] protected ToggleGroup TileToggleGroup;
        [SerializeField] protected RectTransform TileContainer;
        [SerializeField] protected TileIndicator TilePrefab;
        [SerializeField] private Image tileBackgroundImage;

        private Modes.Tile tileMode;

        protected ChildElements<TileIndicator> Tiles;

        private void Awake()
        {
            NewButton.onClick.AddListener(OnClickedNew);

            Tiles = new ChildElements<TileIndicator>(TileContainer, TilePrefab);

            Refresh();
        }

        public void SetTileTool(Modes.Tile mode)
        {
            tileMode = mode;
        }

        public void Refresh()
        {
            Tiles.Clear();

            foreach (Tile tile in Editor.Instance.project_.tileset.tiles)
            {
                TileIndicator element = Tiles.Add();

                element.SetTile(tile);

                element.Toggle.group = TileToggleGroup;
                if (tile == tileMode.paintTile) element.Toggle.isOn = true;

                var set = tile;
               
                element.Toggle.onValueChanged.RemoveAllListeners();
                element.Toggle.onValueChanged.AddListener(delegate (bool active) 
                {
                    if (active) tileMode.paintTile = set;
                });
            }
        }

        public void Update()
        {
            //tileBackgroundImage.sprite = tileMode.paintTile.sprites[0];
        }

        public void OnClickedNew()
        {
            tileMode.paintTile = Editor.Instance.project_.tileset.TestTile();

            Refresh();
        }
    }
}
