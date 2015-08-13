using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;
using kooltool.Serialization;

namespace kooltool.Editor
{
    public class BrowserIcon : MonoBehaviour, IPointerClickHandler
    {
        public Image image;
        
        [SerializeField] private Button button;

        public Summary summary;
        public Action<Summary> single;
        public Action<Summary> @double;

        private void Awake()
        {
            button.onClick.AddListener(() => single(summary));
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData data)
        {
            if (data.clickCount == 2)
            {
                @double(summary);
            }
        }
    }
}
