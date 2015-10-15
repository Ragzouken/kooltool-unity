using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class UIScaler : MonoBehaviour 
{
    [SerializeField] private LayoutElement layout;
    [SerializeField] private RectTransform target;

    [Range(0, 1)]
    [SerializeField] private float scale;

    [SerializeField]
    private float width, height;

    private void Awake()
    {
        //width = target.rect.width;
        //height = target.rect.height;
    }

    private void Update()
    {
        layout.preferredWidth = width;
        layout.preferredHeight = height;

        target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width / scale);
        target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height / scale);
        target.localScale = Vector3.one * scale;
    }
}
