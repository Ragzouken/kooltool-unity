using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool.Editor
{
    public class CostumeIndicator : MonoBehaviour
    {
        [SerializeField] protected Image Image;
        public Toggle Toggle;

        public Costume Costume { get; protected set; }

        public void SetCostume(Costume costume)
        {
            Costume = costume;

            Image.sprite = Costume.Sprite;
            Image.SetNativeSize();
        }
    }
}
