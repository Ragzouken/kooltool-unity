using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class ObjectActionButton : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Button button;

        private System.Action action;

        private void Awake()
        {
            button.onClick.AddListener(OnClickedButton);
        }

        public void Setup(Sprite icon, System.Action action)
        {
            iconImage.sprite = icon;
            this.action = action;
        }

        private void OnClickedButton()
        {
            action();
        }
    }
}
