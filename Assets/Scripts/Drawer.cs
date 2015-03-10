using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Drawer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] protected Tilemap Tilemap;
    [SerializeField] protected InfiniteDrawing Drawing;
    [SerializeField] protected Image ColorButton;
	[SerializeField] protected RectTransform TileCursor;

	public ITool ActiveTool;

	public void SetSize(int value) { PixelTool.Thickness = value; }
	public void SetPencil() { ActiveTool = PixelTool; PixelTool.Tool = Pixel.ToolMode.Pencil; }
	public void SetEraser() { ActiveTool = PixelTool; PixelTool.Tool = Pixel.ToolMode.Eraser; }
	public void SetFiller() { ActiveTool = PixelTool; PixelTool.Tool = Pixel.ToolMode.Fill;   }
    public void SetTile(Tileset.Tile tile) { ActiveTool = TileTool; TileTool.PaintTile = tile; }

	protected Vector2 LastCursor;
	protected bool dragging;

	public Color highlight;
	public float hue;

    public Pixel PixelTool;
    public Tile TileTool;

	public void Awake()
	{
		PixelTool = new Pixel(Tilemap);
        TileTool = new Tile(Tilemap);

        ActiveTool = PixelTool;

		ColorButton.GetComponent<Button>().onClick.AddListener(Randomise);
		
		StartCoroutine(CycleHue());
	}

	public IEnumerator CycleHue()
	{
		while (true)
		{
			hue = (hue + Time.deltaTime) % 1f;

			highlight = new Color(hue, hue, hue, hue);
		
			yield return new WaitForEndOfFrame();
		}
	}

    public void Randomise() 
    { 
        var color = new Color(Random.value, Random.value, Random.value);

        ColorButton.color = color;
		PixelTool.Color = color;
    }

#if UNITY_EDITOR
    [MenuItem("Edit/Reset Playerprefs")] public static void DeletePlayerPrefs() { PlayerPrefs.DeleteAll(); }
#endif
	
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Randomise();
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt)
         || Input.GetKeyDown(KeyCode.LeftShift))
        {
            PixelTool.Tool = Pixel.ToolMode.Picker;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt)
         || Input.GetKeyUp(KeyCode.LeftShift))
        {
            PixelTool.Tool = Pixel.ToolMode.Pencil;
        }

		if (Input.GetKey(KeyCode.Alpha1)) SetSize(1);
		if (Input.GetKey(KeyCode.Alpha2)) SetSize(2);
		if (Input.GetKey(KeyCode.Alpha3)) SetSize(3);
		if (Input.GetKey(KeyCode.Alpha4)) SetSize(4);
		if (Input.GetKey(KeyCode.Alpha5)) SetSize(5);
		if (Input.GetKey(KeyCode.Alpha6)) SetSize(6);
		if (Input.GetKey(KeyCode.Alpha7)) SetSize(7);
		if (Input.GetKey(KeyCode.Alpha8)) SetSize(8);
		if (Input.GetKey(KeyCode.Alpha9)) SetSize(9);

		TileCursor.GetComponent<Image>().color = highlight;

		Vector2 cursor;

		RectTransformUtility.ScreenPointToLocalPointInRectangle(Tilemap.transform as RectTransform, 
		                                                        Input.mousePosition,
		                                                        null,
		                                                        out cursor);

		var grid = new Point(Mathf.FloorToInt(cursor.x / 32f),
		                     Mathf.FloorToInt(cursor.y / 32f));

		TileCursor.anchoredPosition = new Vector2(grid.x * 32 + 16, grid.y * 32 + 16);

		if (dragging)
		{
			ActiveTool.ContinueStroke(LastCursor, cursor);
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

		ActiveTool.BeginStroke(start);

		dragging = true;
	}

	public void OnPointerUp(PointerEventData data)
	{
		Vector2 end;
		
		RectTransformUtility.ScreenPointToLocalPointInRectangle(Tilemap.transform as RectTransform, 
		                                                        data.position,
		                                                        data.pressEventCamera,
		                                                        out end);

		dragging = false;

		ActiveTool.EndStroke(end);
	}
}
