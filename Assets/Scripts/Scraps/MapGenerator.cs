using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;
using kooltool;
using kooltool.Editor;

public class MapGenerator : MonoBehaviour
{
    private class Turtle
    {
        Tilemap Tilemap;
        kooltool.Serialization.Tile Tile;

        Point Position;
        int Direction;

        public Turtle(Tilemap tilemap, kooltool.Serialization.Tile tile, Point position, int direction)
        {
            Tilemap = tilemap;
            Tile = tile;
            Position = position;
            Direction = direction;

            Tilemap.Set(Position, Tile.Instance());
        }

        public void Forward()
        {
            if (Direction == 0) Position = Position + new Point( 1,  0);
            if (Direction == 1) Position = Position + new Point( 0, -1);
            if (Direction == 2) Position = Position + new Point(-1,  0);
            if (Direction == 3) Position = Position + new Point( 0,  1);

            Tilemap.Set(Position, Tile.Instance());
        }

        public void Room(int width, int height)
        {
            for (int y = -height; y < height; ++y)
            {
                for (int x = -width; x < width; ++x)
                {
                    Tilemap.Set(Position + new Point(x, y), Tile.Instance());
                }
            }
        }

        public void Spin()
        {
            Direction = Random.Range(0, 4);
        }
    }

    public Tilemap Tilemap;

    int turtles = 0;

    public void Go(kooltool.Serialization.Project project)
    {
        project.tileset.TestTile();
        project.tileset.TestTile();

        int agents = Random.Range(3, 8);

        for (int i = 0; i < agents; ++i)
        {
            StartCoroutine(Generate(project));
        }
    }

    public IEnumerator Generate(kooltool.Serialization.Project project)
    {
        turtles += 1;

        var turtle = new Turtle(Tilemap, project.tileset.tiles[0], Point.Zero, 0);
        int paths = Random.Range(5, 8);

        for (int y = 0; y < paths; ++y)
        {
            turtle.Spin();

            if (Random.value > 0.125f)
            {
                int limit = Random.Range(4, 8);

                for (int x = 0; x < limit; ++x)
                {
                    turtle.Forward();

                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                turtle.Room(Random.Range(1, 3), Random.Range(1, 3));

                yield return new WaitForEndOfFrame();
            }
        }

        turtles -= 1;

        CheckOutline(project);
    }

    public void CheckOutline(kooltool.Serialization.Project project)
    {
        if (turtles == 0)
        {
            StartCoroutine(Outline(project));
        }
    }

    public IEnumerator Outline(kooltool.Serialization.Project project)
    {
        var outlines = new Queue<Point>();

        foreach (var tile in Tilemap)
        {
            for (int y = -1; y <= 1; ++y)
            {
                for (int x = -1; x <= 1; ++x)
                {
                    kooltool.Serialization.TileInstance dummy;

                    Point outline = tile.Key + new Point(x, y);

                    if (!Tilemap.Get(outline, out dummy))
                    {
                        outlines.Enqueue(outline);
                    }
                }
            }
        }

        int done = 0;

        foreach (var outline in outlines)
        {
            Tilemap.Set(outline, project.tileset.tiles[1].Instance());

            if (done > 16)
            {
                done = 0;

                yield return new WaitForEndOfFrame();
            }

            done++;
        }

        StartCoroutine(DrawTiles(project));
    }

    public IEnumerator DrawTiles(kooltool.Serialization.Project project)
    {
        kooltool.Serialization.Tile tile = project.tileset.tiles[1];
        IDrawing drawing = Tilemap.Drawing;

        Vector2 prev = Vector2.zero;
        Vector2 point = Vector2.zero;

        for (int i = 0; i < 20; ++i)
        {
            float angle = Random.value * Mathf.PI;
            float radius = Random.Range(8, 32);

            point = point + new Vector2(Mathf.Cos(angle) * radius, 
                                        Mathf.Sin(angle) * radius);

            var color = new Color(Random.value, Random.value, Random.value);

            //drawing.DrawLine(prev, point, 3, color, Blend.Alpha);
            //drawing.DrawCircle(point, Random.Range(4, 16), color, Blend.Alpha);

            var points = new Point[]
            {
                new Point(-4, -4),
                new Point( 4, -4),
                new Point( 4,  4),
                new Point( 0,  0),
                new Point(-4,  4),
            };

            //drawing.DrawPolygon(points, color, Blend.Alpha);

            var stencil = Brush.Polygon(points, color);

            var brush = Brush.Circle(32, Color.white);

            ///*
            Brush.Texture(brush,   new Point(0, 0),
                          stencil, new Point(0, 0));
            //*/

            /*
            Brush.Apply(stencil, new Point(point) + Vector2.up * 16,
                        brush,   new Point(point),
                        Blend.Multiply);
            */

            /*
            Point offset;

            Brush.StencilKeep(brush,   new Point(-brush.pivot),
                              stencil, new Point(point + Vector2.right * 4),
                              out brush, out offset);

            drawing.Brush(offset, brush, Blend.Alpha);
            */

            drawing.Brush(new Point(-brush.pivot + point), brush, Blend.Alpha);

            drawing.Apply();

            prev = point;

            yield return new WaitForSeconds(0.10f);
        }
    }
}
