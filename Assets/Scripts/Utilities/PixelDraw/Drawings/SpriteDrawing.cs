using UnityEngine;

namespace PixelDraw
{
    public struct SpriteDrawing : IDrawing
    {
        public Sprite Sprite;

        public SpriteDrawing(Sprite sprite)
        {
            Sprite = sprite;
        }

        public void Brush(Point offset, Sprite image, Blend.BlendFunction blend)
        {
            //offset = offset - new Point(image.pivot);

            //offset = offset.Offset(Sprite.textureRect.position);

            //Assert.True(Sprite.textureRect.Contains(offset.Vector2()), "Offset out of bounds!");
            //Assert.True(Sprite.textureRect.Contains(offset.Vector2() + image.rect.size - Vector2.one), "Image out of bounds!");

            Rect destRect = new Rect(offset.x, offset.y, image.rect.width, image.rect.height);

            /*
            PixelDraw.Brush.Apply(Sprite.texture, destRect,
                                  image.texture, image.textureRect,
                                  blend);
            */

            PixelDraw.Brush.Apply(image,  offset,
                                  Sprite, new Point(0, 0),
                                  blend);
        }

        public void Fill(Point pixel, Color color)
        {
            Sprite.texture.FloodFillAreaNPO2(pixel.x, pixel.y, 
                                             color, 
                                             Sprite.textureRect);
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
}
