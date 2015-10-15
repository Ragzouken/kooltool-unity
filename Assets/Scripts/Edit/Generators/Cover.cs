using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool.Generators
{
    public static class Cover
    {
        private static Point RandomPoint(int max)
        {
            return new Point(Random.Range(0, max), Random.Range(0, max));
        }

        private static UnityEngine.Rect RandomRect()
        {
            return new UnityEngine.Rect(Random.Range(0, 64),
                                        Random.Range(0, 64),
                                        Random.Range(0, 64),
                                        Random.Range(0, 64));
        }

        public static Sprite Bleh()
        {
            var texture = BlankTexture.New(128, 128, new Color(Random.value, Random.value, Random.value));
            var sprite = Sprite.Create(texture, new UnityEngine.Rect(0, 0, 128, 128), Vector2.zero, 1);
            var drawing = new SpriteDrawing(sprite);

            Vector2 prev = Vector2.zero;
            Vector2 point = new Point(Random.Range(0, 128), Random.Range(0, 128));

            for (int i = 0; i < 25; ++i)
            {
                float angle = Random.value * Mathf.PI;
                float radius = Random.Range(32, 128);

                point = point + new Vector2(Mathf.Cos(angle) * radius,
                                            Mathf.Sin(angle) * radius);

                point = new Point(Mathf.Abs(point.x) % 128, Mathf.Abs(point.y) % 128);

                var color = new Color(Random.value, Random.value, Random.value);

                var points = new List<Point>();

                for (int j = 0; j < Random.Range(3, 5); ++j)
                {
                    points.Add(RandomPoint(128));
                }

                //drawing.DrawPolygon(points, color, Blend.Alpha);

                var stencil = Brush.Polygon(points, color);


                int size = Random.Range(16, 64);

                if (i % 3 == 0)
                {
                    var copy = Sprite.Create(texture, RandomRect(), Vector2.zero, 1);

                    drawing.Brush(RandomPoint(128) - Point.One * 32, copy, Blend.Alpha);
                }

                var brush = Brush.Circle(size, new Color(Random.value, Random.value, Random.value));

                Brush.Texture(brush, new Point(0, 0),
                              stencil, new Point(0, 0));

                drawing.Brush(new Point(-brush.pivot + point), brush, Blend.Alpha);

                drawing.Apply();

                prev = point;
            }

            return sprite;
        }
    }
}
