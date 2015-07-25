using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class TileCursor : MonoBehaviour
    {
        [SerializeField] protected Sprite EraserSprite;
        [SerializeField] protected Sprite PickerSprite;

        public Image Preview;
        public Image Border;

        public TileTool Tool;

        public void Update()
        {
            if (Tool.Tool == TileTool.ToolMode.Pencil)
            {
                Preview.sprite = Tool.PaintTile.sprites[0];
            }
            else if (Tool.Tool == TileTool.ToolMode.Picker)
            {
                Preview.sprite = PickerSprite;
            }
            else if (Tool.Tool == TileTool.ToolMode.Eraser)
            {
                Preview.sprite = EraserSprite;
            }
        }
    }
}
