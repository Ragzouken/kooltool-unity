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
        [SerializeField] private TooltipTrigger tooltip;

        private System.Action action;

        private void Awake()
        {
            button.onClick.AddListener(OnClickedButton);
        }

        public void Setup(ObjectAction action)
        {
            iconImage.sprite = IconSettings.Instance[action.icon];
            this.action = action.action;
            tooltip.text = action.tooltip;
        }

        private void OnClickedButton()
        {
            action();
        }
    }
}
