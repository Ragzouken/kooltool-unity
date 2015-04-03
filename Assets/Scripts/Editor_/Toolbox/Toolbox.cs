using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class Toolbox : MonoBehaviour
    {
        [SerializeField] protected Drawer Drawer;
        [SerializeField] protected Toggle PixelTabToggle;
        [SerializeField] protected Toggle TileTabToggle;

        private void Awake()
        {
            PixelTabToggle.onValueChanged.AddListener(OnToggledPixelTab);
            TileTabToggle.onValueChanged.AddListener(OnToggledTileTab);
        }

        public void Show()
        {
            var rtrans = transform as RectTransform;
            var ptrans = transform.parent.transform as RectTransform;

            Vector3 pos;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(ptrans, Input.mousePosition, null, out pos);

            rtrans.position = pos;

            Vector3 minPosition = ptrans.rect.min - rtrans.rect.min;
            Vector3 maxPosition = ptrans.rect.max - rtrans.rect.max;
            
            pos.x = Mathf.Clamp(rtrans.localPosition.x, minPosition.x, maxPosition.x);
            pos.y = Mathf.Clamp(rtrans.localPosition.y, minPosition.y, maxPosition.y);

            pos.x = Mathf.Floor(pos.x);
            pos.y = Mathf.Floor(pos.y);

            rtrans.localPosition = pos;

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnToggledPixelTab(bool active)
        {
            if (active) Drawer.SetPixelTool();
        }

        public void OnToggledTileTab(bool active)
        {
            if (active) Drawer.SetTileTool();
        }
    }
}
