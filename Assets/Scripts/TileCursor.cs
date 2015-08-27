using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class TileCursor : MonoBehaviour
    {
        [SerializeField] private Sprite EraserSprite;
        [SerializeField] private Sprite PickerSprite;
        [SerializeField] private Sprite promoteSprite;
        [SerializeField] private Sprite demoteSprite;

        public Image Preview;
        public Image Border;

        public Modes.Tile mode;

        public void Update()
        {
            if (mode.tool == Modes.Tile.Tool.Pencil)
            {
                if (mode.paintTile != null)
                {
                    SetIcon(mode.paintTile.sprites[0]);
                }
                else
                {
                    SetIcon(EraserSprite);
                }
            }
            else if (mode.tool == Modes.Tile.Tool.Picker)
            {
                SetIcon(PickerSprite);
            }
            else if (mode.tool == Modes.Tile.Tool.Promote)
            {
                SetIcon(promoteSprite);
            }
            else if (mode.tool == Modes.Tile.Tool.Demote)
            {
                SetIcon(demoteSprite);
            }
        }

        public void SetIcon(Sprite sprite)
        {
            Preview.sprite = sprite;
        }
    }
}
