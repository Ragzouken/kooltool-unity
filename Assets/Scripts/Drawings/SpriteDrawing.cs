using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public struct SpriteDrawing : IDrawing
{
    public Sprite Sprite;

    public SpriteDrawing(Sprite sprite)
    {
        Sprite = sprite;
    }

    public void Blit(Point offset, Sprite image, bool subtract = false)
    {
        offset = offset - new Point(image.pivot);

        offset = offset.Offset(Sprite.textureRect.position);

        Assert.True(Sprite.textureRect.Contains(offset.Vector2()), "Offset out of bounds!");
        Assert.True(Sprite.textureRect.Contains(offset.Vector2() + image.rect.size - Vector2.one), "Image out of bounds!");

        Sprite.texture.Blit(offset.x, offset.y, image, subtract);
    }

    public void Fill(Point pixel, Color color)
    {
        //pixel = pixel.Offset(Sprite.textureRect.position);
        
        //Assert.True(Sprite.textureRect.Contains(pixel.Vector2()), "Fill out of bounds!");

        Sprite.texture.FloodFillAreaNPO2(pixel.x, pixel.y, 
                                         color, 
                                         Sprite.textureRect);

        /*
        Sprite.texture.FloodFillArea(pixel.x, 
                                     pixel.y, 
                                     color,
                                     Sprite.textureRect);
        */
    }

    public IEnumerator Fill(Point pixel, Color color, int chunksize)
    {
        return Sprite.texture.FloodFillAreaCR(pixel.x, pixel.y, 
                                              color, 
                                              Sprite.textureRect, 
                                              chunksize);
    }
    
    public bool Sample(Point pixel, out Color color)
    {
        pixel = pixel.Offset(Sprite.textureRect.position);

        Assert.True(Sprite.textureRect.Contains(pixel.Vector2()), "Fill out of bounds!");

        color = Sprite.texture.GetPixel(pixel.x, pixel.y);

        return true;
    }

    public void Apply()
    {
        Sprite.texture.Apply();
    }
}
