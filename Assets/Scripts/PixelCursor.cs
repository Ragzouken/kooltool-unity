using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PixelCursor : MonoBehaviour
{
    public Image Preview;

    public PixelTool Tool;

    public void Update()
    {
        var rtrans = transform as RectTransform;

        rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Tool.Thickness);
        rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   Tool.Thickness);

        Preview.color = Tool.Color;
    }
}
