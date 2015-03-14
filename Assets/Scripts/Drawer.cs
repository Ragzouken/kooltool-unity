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

    [Header("Cursors")]
    [SerializeField] protected PixelCursor PixelCursor;
	[SerializeField] protected TileCursor TileCursor;

	public ITool ActiveTool;

	public void SetSize(int value) { PixelTool.Thickness = value; }
    public void SetPencil() { SetPixelTool(); PixelTool.Tool = PixelTool.ToolMode.Pencil; }
    public void SetEraser() { SetPixelTool(); PixelTool.Tool = PixelTool.ToolMode.Eraser; PixelTool.Color = new Color(0, 0, 0, 0); }
    public void SetFiller() { SetPixelTool(); PixelTool.Tool = PixelTool.ToolMode.Fill;   }

    public void SetTile(Tileset.Tile tile) { SetTileTool(); TileTool.PaintTile = tile; TileTool.Tool = TileTool.ToolMode.Pencil; }
    public void NewTile() { Tilemap.Tileset.AddTile(); }
    public void SetTileErase() { SetTileTool(); TileTool.Tool = TileTool.ToolMode.Eraser; }

	protected Vector2 LastCursor;
	protected bool dragging;

	public Color highlight;
	public float hue;

    public PixelTool PixelTool;
    public TileTool TileTool;

    public void SwitchTool()
    {
        PixelCursor.gameObject.SetActive(false);
        TileCursor.gameObject.SetActive(false);
    }

    public void SetPixelTool()
    {
        SwitchTool();

        ActiveTool = PixelTool;

        PixelCursor.gameObject.SetActive(true);
    }

    public void SetTileTool()
    {
        SwitchTool();

        ActiveTool = TileTool;

        TileCursor.gameObject.SetActive(true);
    }

	public void Awake()
	{
		PixelTool = new PixelTool(Tilemap, Drawing);
        TileTool = new TileTool(Tilemap);

        PixelCursor.Tool = PixelTool;
        TileCursor.Tool = TileTool;

        PixelTool.CoroutineTarget = this;

        ActiveTool = PixelTool;

		ColorButton.GetComponent<Button>().onClick.AddListener(Randomise);
		
		StartCoroutine(CycleHue());
	}

	public IEnumerator CycleHue()
	{
		while (true)
		{
			hue = (hue + Time.deltaTime) % 1f;

            IList<double> RGB = HUSL.HUSLPToRGB(new double[] { hue * 360, 100, 75 });

			highlight = new Color((float) RGB[0], (float) RGB[1], (float) RGB[2], 1f);
		
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
            PixelTool.Tool = PixelTool.ToolMode.Picker;
            TileTool.Tool = TileTool.ToolMode.Picker;
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

		Vector2 cursor;

		RectTransformUtility.ScreenPointToLocalPointInRectangle(Tilemap.transform as RectTransform, 
		                                                        Input.mousePosition,
		                                                        null,
		                                                        out cursor);

		var grid = new Point(Mathf.FloorToInt(cursor.x / 32f),
		                     Mathf.FloorToInt(cursor.y / 32f));

        float offset = (PixelTool.Thickness % 2 == 1) ? 0.5f : 0;

        PixelCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.FloorToInt(cursor.x) + offset,
                                                                                 Mathf.FloorToInt(cursor.y) + offset);
        TileCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(grid.x * 32 + 16, grid.y * 32 + 16);

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
