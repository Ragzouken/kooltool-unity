using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class Texture2DBlit
{
    public static void Blit(this Texture2D destination,
                            int x, int y,
                            Sprite image,
                            bool subtract = false)
    {
        Rect destRect = new Rect(x, y, image.rect.width, image.rect.height);

        Color[] colors = destination.GetPixelRect(destRect);
        Color[] source = image.texture.GetPixelRect(image.textureRect);

        for (int i = 0; i < colors.Length; ++i)
        {
            Color a = colors[i];
            Color b = source[i];

            if (subtract)
            {
                colors[i] = a - b;
            }
            else
            {
                colors[i] = a * (1 - b.a) + b * b.a;
            }
        }

        destination.SetPixelRect(destRect, colors);
    }
}
