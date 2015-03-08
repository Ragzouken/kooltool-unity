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
        bool exists = Items.ContainsKey(cell);
        
        Items[cell] = item;
        
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

        offset = new Point(point.x % CellWidth,
                           point.y % CellHeight);
    }

    public IEnumerator<KeyValuePair<Point, T>> GetEnumerator()
    {
        return Items.GetEnumerator();
    }
}
