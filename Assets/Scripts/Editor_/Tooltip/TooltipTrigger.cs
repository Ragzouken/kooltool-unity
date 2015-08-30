using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private kooltool.Editor.Editor editor;
    [SerializeField] private string text;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        editor.tooltip.SetText(text);
    }
}
