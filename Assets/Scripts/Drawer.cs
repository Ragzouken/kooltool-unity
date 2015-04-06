using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;

using kooltool;
using kooltool.Editor;

public class Drawer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] protected Tilemap Tilemap;
    [SerializeField] protected InfiniteDrawing Drawing;

    [SerializeField] protected RectTransform World;

    [Header("Cursors")]
    [SerializeField] protected PixelCursor PixelCursor;
	[SerializeField] protected TileCursor TileCursor;

	public ITool ActiveTool;

    [SerializeField] protected Toolbox Toolbox;

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

        ActiveTool = Toolbox.PixelTool;

        PixelCursor.gameObject.SetActive(true);
    }

    public void SetTileTool()
    {
        SwitchTool();

        ActiveTool = Toolbox.TileTool;

        TileCursor.gameObject.SetActive(true);
    }

	public void Start()
	{
		Toolbox.PixelTool = new PixelTool(Tilemap, Drawing);
        Toolbox.TileTool = new TileTool(Tilemap, Editor.Project.Tileset);

        Toolbox.PixelTab.SetPixelTool(Toolbox.PixelTool);
        Toolbox.TileTab.SetTileTool(Toolbox.TileTool);

        PixelCursor.Tool = Toolbox.PixelTool;
        TileCursor.Tool = Toolbox.TileTool;

        Toolbox.PixelTool.CoroutineTarget = this;

        ActiveTool = Toolbox.PixelTool;
	}
	
    public void Update()
    {
		Vector2 cursor;

		RectTransformUtility.ScreenPointToLocalPointInRectangle(Tilemap.transform as RectTransform, 
		                                                        Input.mousePosition,
		                                                        null,
		                                                        out cursor);

        Point grid, dummy;

        Editor.Project.Grid.Coords(new Point(cursor), out grid, out dummy);

        float offset = (Toolbox.PixelTool.Thickness % 2 == 1) ? 0.5f : 0;

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
            World.localPosition += (Vector3) (cursor - pansite);
            World.localPosition = new Vector3(Mathf.Floor(World.localPosition.x),
                                              Mathf.Floor(World.localPosition.y),
                                              0f);
        }

		LastCursor = cursor;
	}
	
	public void OnPointerDown(PointerEventData data)
	{
		Vector2 start;
		
		RectTransformUtility.ScreenPointToLocalPointInRectangle(Tilemap.transform as RectTransform, 
                                                                data.position,
		                                                        data.pressEventCamera,
		                                                        out start);


        var go = data.pointerCurrentRaycast.gameObject;

        Debug.Log("clicked " + go.name, go);

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
		Vector2 end;
		
		RectTransformUtility.ScreenPointToLocalPointInRectangle(Tilemap.transform as RectTransform, 
		                                                        data.position,
		                                                        data.pressEventCamera,
		                                                        out end);

		dragging = false;

        panning = false;

        PixelCursor.end = Floor(end);
        PixelCursor.Update();
		ActiveTool.EndStroke(Floor(end));
	}
}
