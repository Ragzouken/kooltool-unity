using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using kooltool.Data;

namespace kooltool.Editor
{
    public class TileTab : MonoBehaviour
    {
        [SerializeField] private Toolbox toolbox;

        [Header("Tools")]
        [SerializeField] protected Button NewButton;
        [SerializeField] private GameObject trashIcon;

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

            foreach (Tile tile in toolbox.editor.project_.tileset.tiles)
            {
                TileIndicator element = Tiles.Add();

                element.SetTile(tile, toolbox);

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
            NewButton.gameObject.SetActive(toolbox.draggedItem == null);
            trashIcon.gameObject.SetActive(toolbox.draggedItem != null);
        }

        public void OnClickedNew()
        {
            tileMode.paintTile = Editor.Instance.project_.tileset.TestTile();

            Refresh();
        }

        public void OnDroppedTrash()
        {
            var tile = toolbox.draggedItem as Tile;

            if (tile != null)
            {
                toolbox.CancelDrag();
                toolbox.editor.project_.RemoveTile(tile);
                toolbox.editor.RefreshTilemap();
                Refresh();
            }
        }
    }
}
