using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class ColorIndicator : MonoBehaviour
    {
        [SerializeField] protected Image Image;

        public Toggle Toggle;

        public void SetColor(Color color)
        {
            Image.color = color;
        }
    }
}
