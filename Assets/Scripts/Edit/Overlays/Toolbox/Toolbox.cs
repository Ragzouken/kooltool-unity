﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class Toolbox : MonoBehaviour
    {
        [Header("Dragging")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image draggedItemImage;

        [Header("Tabs")]
        [SerializeField] private Toggle pixelTabToggle;
        [SerializeField] private Toggle tileTabToggle;
        [SerializeField] private Toggle notesTabToggle;

        public PixelTab pixelTab;
        public TileTab tileTab;
        public NotesTab notesTab;

        public Editor editor { get; private set; }

        public object draggedItem;

        private void Awake()
        {
            pixelTabToggle.onValueChanged.AddListener(OnToggledPixelTab);
            tileTabToggle.onValueChanged.AddListener(OnToggledTileTab);
            notesTabToggle.onValueChanged.AddListener(OnToggledNotesTab);

            draggedItemImage.gameObject.SetActive(false);
        }

        public void SetProject(Editor editor,
                               ProjectOld project)
        {
            this.editor = editor;

            pixelTab.SetProject(project);
        }

        public void Show()
        {
            var rtrans = transform as RectTransform;
            var ptrans = transform.parent.transform as RectTransform;

            Vector3 pos;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(ptrans, Input.mousePosition + Vector3.up * 64, null, out pos);

            rtrans.position = pos;

            Vector3 minPosition = ptrans.rect.min - rtrans.rect.min;
            Vector3 maxPosition = ptrans.rect.max - rtrans.rect.max;
            
            pos.x = Mathf.Clamp(rtrans.localPosition.x, minPosition.x, maxPosition.x);
            pos.y = Mathf.Clamp(rtrans.localPosition.y, minPosition.y, maxPosition.y);

            pos.x = Mathf.Floor(pos.x) + 0.5f;
            pos.y = Mathf.Floor(pos.y) + 0.5f;

            rtrans.localPosition = pos;

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            CancelDrag();
        }

        private void Update()
        {
            draggedItemImage.transform.position = Input.mousePosition;
        }

        public void BeginDrag(object item, Sprite sprite)
        {
            draggedItem = item;
            draggedItemImage.gameObject.SetActive(true);
            draggedItemImage.sprite = sprite;
            canvasGroup.interactable = false;
        }

        public void CancelDrag()
        {
            draggedItem = null;
            draggedItemImage.gameObject.SetActive(false);
            canvasGroup.interactable = true;
        }

        public void OnToggledPixelTab(bool active)
        {
            if (active) Editor.Instance.SetPixelTool();
        }

        public void OnToggledTileTab(bool active)
        {
            if (active) Editor.Instance.SetTileTool();
        }

        public void OnToggledNotesTab(bool active)
        {
            if (active) Editor.Instance.SetNotesMode();
        }
    }
}
