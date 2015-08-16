using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;
using kooltool.Serialization;

namespace kooltool.Editor
{
    public class ProjectTile : MonoBehaviour, 
                               IPointerEnterHandler,
                               IPointerExitHandler, 
                               IPointerClickHandler
    {
        [SerializeField] private Image coverImage;
        [SerializeField] private InputField titleInput;
        [SerializeField] private InputField blurbInput;
        [SerializeField] private GameObject textContainer;

        [SerializeField] private Button deleteButton;

        public Summary summary { get; protected set; }

        public System.Action<Summary> OnClickedOpen = delegate { };
        public System.Action<Summary> OnClickedDelete = delegate { };

        private void Awake()
        {
            deleteButton.onClick.AddListener(() => OnClickedDelete(summary));

            titleInput.onEndEdit.AddListener(OnChanged);
            blurbInput.onEndEdit.AddListener(OnChanged);
        }

        public void SetSummary(Summary summary)
        {
            this.summary = summary;

            coverImage.sprite = summary.iconSprite;
            titleInput.text = summary.title;
            blurbInput.text = summary.description;
        }

        private void OnChanged(string dummy)
        {
            summary.title = titleInput.text;
            summary.description = blurbInput.text;

            ProjectTools.SaveSummary(summary);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData data)
        {
            textContainer.SetActive(true);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData data)
        {
            textContainer.SetActive(false);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData data)
        {
            if (data.clickCount == 2)
            {
                OnClickedOpen(summary);
            }
        }
    }
}
