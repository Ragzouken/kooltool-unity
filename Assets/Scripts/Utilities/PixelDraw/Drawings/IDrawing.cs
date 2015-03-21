using UnityEngine;

namespace PixelDraw
{
    public interface IDrawing
    {
        void Brush(Point position, Sprite brush);
        void Fill(Point position, Color color);
        bool Sample(Point position, out Color color);
    }
}
