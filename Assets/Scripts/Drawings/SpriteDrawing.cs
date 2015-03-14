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

    public void Point(Point pixel, Color color)
    {
        pixel = pixel.Offset(Sprite.textureRect.position);
        Assert.True(Sprite.textureRect.Contains(pixel.Vector2()), "Point out of bounds!");

        Sprite.texture.SetPixel(pixel.x, pixel.y, color);
    }

    public void Line(Point start, Point end, Color color)
    {
        start = start.Offset(Sprite.textureRect.position);
        end = end.Offset(Sprite.textureRect.position);

        Assert.True(Sprite.textureRect.Contains(start.Vector2()), "Line starts out of bounds!");
        Assert.True(Sprite.textureRect.Contains(end.Vector2()), "Line ends out of bounds!");

        Texture2D texture = Sprite.texture;

        Bresenham.PlotFunction plot = delegate (int x, int y)
        {
            texture.SetPixel(x, y, color);
            
            return true;
        };
        
        Bresenham.Line(start.x, start.y, end.x, end.y, plot);
    }

    public void Blit(Point offset, Sprite image, bool subtract = false)
    {
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
