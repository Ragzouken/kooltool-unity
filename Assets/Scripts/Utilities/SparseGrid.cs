using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SparseGrid<T>
{
	protected Dictionary<Point, T> Items
		= new Dictionary<Point, T>();

    protected int CellWidth, CellHeight;

    public delegate T Constructor(Point cell);

	public SparseGrid()
	{
	}

    public SparseGrid(int size)
        : this(size, size)
    {
    }

    public SparseGrid(int cellWidth, int cellHeight)
    {
        CellWidth = cellWidth;
        CellHeight = cellHeight;
    }

    public void Clear()
    {
        Items.Clear();
    }

    public bool Get(Point cell, out T item)
    {
        return Items.TryGetValue(cell, out item);
    }

    public bool Set(Point cell, T item)
    {
        T existing;

        bool change = !(Get(cell, out existing) && Equals(existing, item));

        Items[cell] = item;
        
        return change;
    }

    public bool Unset(Point cell, out T existing)
    {
        bool exists = Get(cell, out existing);

        if (exists)
        {
            Items.Remove(cell);
        }

        return exists;
    }

    public void GetDefault(Point cell, 
                           out T item,
                           Constructor constructor)
    {
        if (!Get(cell, out item))
        {
            item = constructor(cell);
        }
    }

    public void Coords(Point point, out Point grid, out Point offset)
    {
        grid = new Point(Mathf.FloorToInt(point.x / (float) CellWidth),
                         Mathf.FloorToInt(point.y / (float) CellHeight));

        int ox = point.x % CellWidth;
        int oy = point.y % CellHeight;

        offset = new Point(ox >= 0 ? ox : CellWidth  + ox,
                           oy >= 0 ? oy : CellHeight + oy);
    }

    public IEnumerator<KeyValuePair<Point, T>> GetEnumerator()
    {
        return Items.GetEnumerator();
    }
}
