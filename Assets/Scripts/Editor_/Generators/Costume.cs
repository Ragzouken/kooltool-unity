using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool.Generators
{
    public static class Costume
    {
        public static kooltool.Data.Costume Smiley(Data.Project project,
                                                            int width, 
                                                            int height) 
        {
            var costume = new Data.Costume
            {
                name = "Smiley",
                texture = project.index.CreateTexture(width, height),
            };

            costume.TestInit();

            Color face = new Color(Random.value,
                                   Random.value,
                                   Random.value);

            Color feature = new Color(Random.value,
                                      Random.value,
                                      Random.value);

            Brush.Apply(Brush.Circle(width, face), 
                        new Point(0, 0),
                        costume.sprite, 
                        new Point(0, 0),
                        Blend.Alpha);

            Sprite eye = Brush.Circle(width / 8, feature);

            Brush.Apply(eye, new Point(-width / 4, height / 4),
                        costume.sprite, new Point(0, 0),
                        Blend.Alpha);

            Brush.Apply(eye, new Point(width / 4, height / 4),
                        costume.sprite, new Point(0, 0),
                        Blend.Alpha);

            costume.texture.texture.Apply();

            return costume;
        }
    }
}
