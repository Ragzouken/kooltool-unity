using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

public static class Texture2DBlit
{
    public static void Blit(this Texture2D destination,
                            int x, int y,
                            Sprite image,
                            bool subtract = false)
    {
        Rect destRect = new Rect(x, y, image.rect.width, image.rect.height);

        Brush.Blend(destination, destRect,
                    image.texture, image.textureRect,
                    subtract ? Brush.SubtractBlend : Brush.AlphaBlend);
    }
}
