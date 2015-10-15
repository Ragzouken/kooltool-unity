using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;
using kooltool.Data;

namespace kooltool.Editor
{
    public class CostumeIndicator : MonoBehaviour,
                                    IBeginDragHandler,
                                    IEndDragHandler
    {
        [SerializeField] private Image Image;
        [SerializeField] private Button Button;

        public Costume Costume { get; protected set; }

        private Toolbox toolbox;
        private System.Action action;

        private void Awake()
        {
            Button.onClick.AddListener(OnClicked);
        }

        public void SetCostume(Toolbox toolbox,
                               Costume costume, System.Action action)
        {
            this.toolbox = toolbox;

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

        public void OnBeginDrag(PointerEventData eventData)
        {
            toolbox.BeginDrag(Costume, Costume.sprite);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            toolbox.CancelDrag();
        }
    }
}
