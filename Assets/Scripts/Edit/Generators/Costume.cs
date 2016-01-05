using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool.Generators
{
    public static class Costume
    {
        private static Data.Frame Frame(Data.Project project,
                                        int width, 
                                        int height)
        {
            return new Data.Frame
            {
                texture = project.index.CreateTexture(128, 128),
                rect = new Data.Rect { x = 0, y = 0, w = 128, h = 128},
                pivotX = 64,
                pivotY = 64,
            };
        }

        public static Data.Costume Smiley(Data.Project project,
                                          int width, 
                                          int height) 
        {
            var costume = new Data.Costume
            {
                name = "Smiley",
            };

            string[] dirs = { "n", "e", "s", "w" };

            foreach (string dir in dirs)
            {
                costume.SetFlipbook("idle", dir, new Data.Flipbook
                {
                    name = "idle",
                    tag = dir,
                    frames = new List<Data.Frame>
                    {
                        Frame(project, width, height),
                        Frame(project, width, height),
                        Frame(project, width, height),
                    },
                });
            }

            costume.TestInit();

            Color face = new Color(Random.value,
                                   Random.value,
                                   Random.value);

            Color feature = new Color(Random.value,
                                      Random.value,
                                      Random.value);


            foreach (string dir in dirs)
            {
                foreach (Data.Frame frame in costume.GetFlipbook("idle", dir).frames)
                {
                    int ox = Random.Range(-4, 5);
                    int oy = Random.Range(-4, 5);

                    Brush.Apply(Brush.Circle(width, face),
                                new Point(0, 0),
                                frame,
                                new Point(0, 0),
                                Blend.Alpha);

                    Sprite eye = Brush.Circle(width / 8, feature);

                    Brush.Apply(eye, new Point(-width / 4 - ox, height / 4 + oy),
                                frame, new Point(0, 0),
                                Blend.Alpha);

                    Brush.Apply(eye, new Point(width / 4 + ox, height / 4 + oy),
                                frame, new Point(0, 0),
                                Blend.Alpha);

                    frame.texture.texture.Apply();
                }
            }

            costume.SetFlipbook("idle", "", costume.GetFlipbook("idle", "s"));

            return costume;
        }
    }
}
