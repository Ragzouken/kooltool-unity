using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class BlankTexture
{
    public static Texture2D New(int width, int height, Color32 color)
    {
        var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;

        var pixels = new Color32[width * height];
        
        for (int i = 0; i < pixels.Length; ++i)
        {
            pixels[i] = color;
        }
        
        texture.SetPixels32(pixels);
        texture.Apply();

        return texture;
    }
}
