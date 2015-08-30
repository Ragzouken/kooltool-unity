using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class TileCursor : MonoBehaviour
    {
        [Header("sprites")]
        [SerializeField] private Sprite EraserSprite;
        [SerializeField] private Sprite PickerSprite;
        [SerializeField] private Sprite fillSprite;
        [SerializeField] private Sprite promoteSprite;
        [SerializeField] private Sprite demoteSprite;
        [SerializeField] private Sprite solidBorder;
        [SerializeField] private Sprite dashedBorder;

        [Header("Images")]
        [SerializeField] private Image tileImage;
        [SerializeField] private Image borderImage;
        [SerializeField] private Image toolIcon;

        public Modes.Tile mode;

        public Editor editor;

        public void Refresh()
        {
            bool showTile = false;
            Sprite icon = null;

            if (mode.tool == Modes.Tile.Tool.Pencil)
            {
                if (mode.paintTile != null)
                {
                    showTile = true;
                }
                else
                {
                    icon = EraserSprite;
                }
            }
            else if (mode.tool == Modes.Tile.Tool.Picker)
            {
                icon = PickerSprite;
            }
            else if (mode.tool == Modes.Tile.Tool.Fill)
            {
                showTile = true;
                icon = fillSprite;
            }
            else if (mode.tool == Modes.Tile.Tool.Promote)
            {
                if (mode.hoveredTile.HasValue)
                {
                    icon = demoteSprite;
                }
                else
                {
                    icon = promoteSprite;
                }
            }

            if (mode.paintTile != null) tileImage.sprite = mode.paintTile.sprites[0];
            tileImage.enabled = showTile;

            toolIcon.sprite = icon;
            toolIcon.gameObject.SetActive(toolIcon.sprite != null);

            borderImage.sprite = mode.hoveredTile.HasValue ? dashedBorder
                                                           : solidBorder;
        }
    }
}
