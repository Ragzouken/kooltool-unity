using UnityEngine;

public struct Point
{
    public readonly int x;
    public readonly int y;

    public static Point Zero = new Point(0, 0);
    public static Point One = new Point(1, 1);

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Point(float x, float y)
        : this((int) x, (int) y)
    {
    }

    public Point(Vector2 point)
        : this(point.x, point.y)
    {
    }

    public Point Offset(Vector2 offset)
    {
        return this + new Point(offset);
    }

    public Vector2 Vector2()
    {
        return new UnityEngine.Vector2(x, y);
    }

    public Point Size
    {
        get
        {
            return new Point(Mathf.Abs(x), Mathf.Abs(y));
        }
    }

    public override bool Equals (object obj)
    {
        if (obj is Point)
        {
            return Equals((Point) obj);
        }

        return false;
    }

    public bool Equals(Point other)
    {
        return other.x == x
            && other.y == y;
    }

    public override int GetHashCode()
    {
        return string.Format("{0},{1}", x, y).GetHashCode();
    }

    public override string ToString ()
    {
        return string.Format("Point({0}, {1})", x, y);
    }

    public static Point operator +(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y);
    }

    public static Point operator -(Point a, Point b)
    {
        return new Point(a.x - b.x, a.y - b.y);
    }

    public static Point operator +(Point a, Vector2 b)
    {
        return new Point(a.x + b.x, a.y + b.y);
    }
    
    public static Point operator -(Point a, Vector2 b)
    {
        return new Point(a.x - b.x, a.y - b.y);
    }

    public static Point operator *(Point a, int scale)
    {
        return new Point(a.x * scale, a.y * scale);
    }
}
