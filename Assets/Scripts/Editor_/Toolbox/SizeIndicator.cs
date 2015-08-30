using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool.Editor
{
    public class SizeIndicator : MonoBehaviour
    {
        [SerializeField] protected Image Image;
        public Toggle Toggle;

        public void SetSize(int value)
        {
            Sprite button = Brush.Rectangle(32, 32, Color.clear); 
            Sprite brush = Brush.Circle(value, Color.white);
            Brush.Apply(brush, Point.One * 16, 
                        button, Point.Zero, 
                        Blend.Alpha);

            brush.texture.Apply();

            Image.sprite = button;
            //Image.SetNativeSize();

            /*
            if (value % 2 == 1)
            {
                Image.GetComponent<RectTransform>().anchoredPosition = Vector2.one * 0.5f;
            }*/
        }
    }
}
