using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool.Generators
{
    public static class Costume
    {
        public static kooltool.Costume Smiley(int width, int height) 
        {
            var texture = BlankTexture.New(width, height, Color.clear);
            
            var sprite = Sprite.Create(texture, 
                                       new Rect(0, 0, texture.width, texture.height),
                                       Vector2.one * 0.5f,
                                       1f);

            Color face = new Color(Random.value,
                                   Random.value,
                                   Random.value);

            Color feature = new Color(Random.value,
                                      Random.value,
                                      Random.value);

            Brush.Apply(Brush.Circle(width, face), 
                        new Point(0, 0),
                        sprite, 
                        new Point(0, 0),
                        Blend.Alpha);

            Sprite eye = Brush.Circle(width / 8, feature);

            Brush.Apply(eye, new Point(-width / 4, height / 4),
                        sprite, new Point(0, 0),
                        Blend.Alpha);

            Brush.Apply(eye, new Point(width / 4, height / 4),
                        sprite, new Point(0, 0),
                        Blend.Alpha);

            texture.Apply();

            return new kooltool.Costume("Smiley", sprite);
        }
    }
}
