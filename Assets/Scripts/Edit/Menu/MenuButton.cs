using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace kooltool
{
    public class MenuButton : MonoBehaviour
    {
        [SerializeField] private Text label;
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image background;

        private Action action = delegate { };

        private void Awake()
        {
            toggle.onValueChanged.AddListener(active => { if (active) action(); });
        }

        public void Setup(string text, Action action)
        {
            label.text = text;
            this.action = action;

            toggle.interactable = action != null;
        }
    }
}
