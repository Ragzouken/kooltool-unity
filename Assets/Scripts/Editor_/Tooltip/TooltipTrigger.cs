using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;
using kooltool.Editor;

public class TooltipTrigger : MonoBehaviour, 
                              IPointerEnterHandler,
                              IPointerExitHandler,
                              IPointerClickHandler
{
    public string text;

    private void OnDisable()
    {
        Editor.Instance.tooltip.Exit(this);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        Editor.Instance.tooltip.Exit(this);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Editor.Instance.tooltip.Enter(this);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        Editor.Instance.tooltip.Exit(this);
    }
}
