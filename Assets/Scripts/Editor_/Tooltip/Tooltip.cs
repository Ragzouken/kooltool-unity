using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class Tooltip : MonoBehaviour 
{
    [SerializeField] private Text text;

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
