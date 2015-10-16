using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;

using kooltool.Data;

namespace kooltool.Editor
{
    public class RegionElement : MonoBehaviour,
                                 IBeginDragHandler,
                                 IEndDragHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private TooltipTrigger tooltip;

        public Toggle toggle;

        public Region region { get; protected set; }

        private Toolbox toolbox;

        public void SetRegion(Region region, Toolbox toolbox)
        {
            this.toolbox = toolbox;

            this.region = region;

            image.sprite = region.icon.texture.FullSprite();
            image.SetNativeSize();

            tooltip.text = region.name;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            toolbox.BeginDrag(region, image.sprite);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            toolbox.CancelDrag();
        }
    }
}
