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
            Image.sprite = Brush.Circle(value, Color.white);
            Image.sprite.texture.Apply();
            Image.SetNativeSize();
        }
    }
}
