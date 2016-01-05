using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CycleHue : MonoBehaviour
{
    [Range(0, 2)]
    public float period = 0.75f;
    [Range(0, 100)]
    public int Saturation = 100;
    [Range(0, 100)]
    public int Lightness = 75;
    [Range(0, 1)]
    public float alpha = 1;

    protected Graphic Image;

    protected void Awake()
    {
        Image = GetComponent<Graphic>();
    }

    protected void Update()
    {
        float hue = (Time.timeSinceLevelLoad / period) % 1f;
        
        IList<double> RGB = HUSL.HUSLPToRGB(new double[] { hue * 360, Saturation, Lightness });
        
        Image.color = new Color((float) RGB[0], (float) RGB[1], (float) RGB[2], alpha);
    }
}
