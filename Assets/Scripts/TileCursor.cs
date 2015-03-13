using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TileCursor : MonoBehaviour
{
    [SerializeField] protected Sprite EraserSprite;
    [SerializeField] protected Sprite PickerSprite;

    public Image Preview;
    public Image Border;

    public TileTool Tool;

    public void Update()
    {
        Border.color = GetComponentInParent<Drawer>().highlight;

        if (Tool.Tool == TileTool.ToolMode.Pencil)
        {
            Preview.sprite = Tool.PaintTile.Thumbnail;
        }
        else if (Tool.Tool == TileTool.ToolMode.Picker)
        {
            Preview.sprite = PickerSprite;
        }
    }
}
