using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PointFont : MonoBehaviour
{
    [SerializeField] protected List<Font> Fonts;

    public void Awake()
    {
        foreach (var font in Fonts)
        {
            font.material.mainTexture.filterMode = FilterMode.Point;
        }
    }
}
