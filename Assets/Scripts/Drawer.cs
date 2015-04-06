using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;

using kooltool;
using kooltool.Editor;

public class Drawer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Cursors")]
    [SerializeField] protected PixelCursor PixelCursor;
	[SerializeField] protected TileCursor TileCursor;

	public ITool ActiveTool;

	protected Vector2 LastCursor;
	protected bool dragging;
    bool panning;
    Vector2 pansite;

    public Editor Editor;

    public void SwitchTool()
    {
        PixelCursor.gameObject.SetActive(false);
        TileCursor.gameObject.SetActive(false);
    }

    public void SetPixelTool()
    {
        SwitchTool();

        ActiveTool = Editor.Toolbox.PixelTool;

        PixelCursor.gameObject.SetActive(true);
    }

    public void SetTileTool()
    {
        SwitchTool();

        ActiveTool = Editor.Toolbox.TileTool;

        TileCursor.gameObject.SetActive(true);
    }

	public void Start()
	{
        PixelCursor.Tool = Editor.Toolbox.PixelTool;
        TileCursor.Tool = Editor.Toolbox.TileTool;

        ActiveTool = Editor.Toolbox.PixelTool;
	}
	
    public void Update()
    {
        Vector2 cursor = Editor.ScreenToWorld(Input.mousePosition);

        Point grid, dummy;

        Editor.Project.Grid.Coords(new Point(cursor), out grid, out dummy);

        float offset = (Editor.Toolbox.PixelTool.Thickness % 2 == 1) ? 0.5f : 0;

        PixelCursor.end = Floor(cursor);
        PixelCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.FloorToInt(cursor.x) + offset,
                                                                                 Mathf.FloorToInt(cursor.y) + offset);
        TileCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2((grid.x + 0.5f) * Editor.Project.Grid.CellWidth, 
                                                                                (grid.y + 0.5f) * Editor.Project.Grid.CellHeight);

		if (dragging)
		{
			ActiveTool.ContinueStroke(LastCursor, cursor);
		}
        else if (panning)
        {
            Editor.World.localPosition += (Vector3) (cursor - pansite);
            Editor.World.localPosition = new Vector3(Mathf.Floor(Editor.World.localPosition.x),
                                                     Mathf.Floor(Editor.World.localPosition.y),
                                              0f);
        }

		LastCursor = cursor;
	}
	
	public void OnPointerDown(PointerEventData data)
	{
        Vector2 start = Editor.ScreenToWorld(data.position);

        if (data.button == PointerEventData.InputButton.Left)
        {
    		ActiveTool.BeginStroke(Floor(start));

    		dragging = true;
        }
        else
        {
            panning = true;

            pansite = start;
        }
	}

    public Vector2 Floor(this Vector2 vector)
    {
        return new Vector2(Mathf.Floor(vector.x),
                           Mathf.Floor(vector.y));
    }

	public void OnPointerUp(PointerEventData data)
	{
        Vector2 end = Editor.ScreenToWorld(data.position);

		dragging = false;

        panning = false;

        PixelCursor.end = Floor(end);
        PixelCursor.Update();
		ActiveTool.EndStroke(Floor(end));
	}
}
