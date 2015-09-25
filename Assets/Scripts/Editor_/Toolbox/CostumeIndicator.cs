using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;
using kooltool.Data;

namespace kooltool.Editor
{
    public class CostumeIndicator : MonoBehaviour
    {
        [SerializeField] private Image Image;
        [SerializeField] private Button Button;

        public Costume Costume { get; protected set; }

        private System.Action action;

        private void Awake()
        {
            Button.onClick.AddListener(OnClicked);
        }

        public void SetCostume(Costume costume, System.Action action)
        {
            Costume = costume;

            this.action = action;

            costume.TestInit();
            Image.sprite = Costume.sprite;
            Image.SetNativeSize();
        }

        private void OnClicked()
        {
            action();
        }
    }
}
