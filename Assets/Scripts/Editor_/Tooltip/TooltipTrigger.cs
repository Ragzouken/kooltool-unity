using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;
using kooltool.Editor;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private string text;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Editor.Instance.tooltip.SetText(text);
    }
}
