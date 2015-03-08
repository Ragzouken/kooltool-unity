using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Drawer : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    [System.Serializable]
    public enum ToolMode { Pencil, Eraser, Fill }

    [SerializeField] protected Tilemap Tilemap;
    [SerializeField] protected InfiniteDrawing Drawing;
    [SerializeField] protected Image ColorButton;

    public ToolMode Tool;
    public Color Color = Color.red;
    public int Thickness = 1;

    public void SetSize(int value) { Thickness = value; }

    public void SetPencil() { Tool = ToolMode.Pencil; }
    public void SetEraser() { Tool = ToolMode.Eraser; }
    public void SetFiller() { Tool = ToolMode.Fill;   }

    public void Randomise() 
    { 
        Color = new Color(Random.value, Random.value, Random.value);

        ColorButton.color = Color;
    }

#if UNITY_EDITOR
    [MenuItem("Edit/Reset Playerprefs")] public static void DeletePlayerPrefs() { PlayerPrefs.DeleteAll(); }
#endif

    public void Awake()
    {
        Color = new Color(Random.value, Random.value, Random.value);

        Thickness = 1;

        ColorButton.GetComponent<Button>().onClick.AddListener(Randomise);
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Randomise();
        }

        if (Input.GetKey(KeyCode.Alpha1)) Thickness = 1;
        if (Input.GetKey(KeyCode.Alpha2)) Thickness = 2;
        if (Input.GetKey(KeyCode.Alpha3)) Thickness = 3;
        if (Input.GetKey(KeyCode.Alpha4)) Thickness = 4;
        if (Input.GetKey(KeyCode.Alpha5)) Thickness = 5;
        if (Input.GetKey(KeyCode.Alpha6)) Thickness = 6;
        if (Input.GetKey(KeyCode.Alpha7)) Thickness = 7;
        if (Input.GetKey(KeyCode.Alpha8)) Thickness = 8;
        if (Input.GetKey(KeyCode.Alpha9)) Thickness = 9;
    }

    public void OnPointerClick(PointerEventData data)
    {


        Vector2 start;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(Tilemap.transform as RectTransform, 
                                                                data.position,
                                                                data.pressEventCamera,
                                                                out start);

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftAlt))
        {
            Color sampled;

            if (Tilemap.Sample(new Point(start), out sampled))
            {
                Color = sampled;

                if (sampled.a == 0)
                {
                    Tool = ToolMode.Eraser;
                }
            }
        }
        else if (Tool == ToolMode.Fill)
        {
            Tilemap.Fill(new Point(start), Color);
            Tilemap.Apply();
        }
    }

    public Sprite LineBrush(Point start, Point end, Color color, int thickness)
    {
        int left = Mathf.FloorToInt(thickness / 2f);
        int right = thickness - 1 - left;

        Point size = (end - start).Size + new Point(thickness, thickness);

        Texture2D brush = BlankTexture.New(size.x, size.y, 
                                           new Color32(0, 0, 0, 0));

        Sprite sprite = Sprite.Create(brush, 
                                      new Rect(0, 0, brush.width, brush.height), 
                                      Vector2.zero);

        Bresenham.PlotFunction plot = delegate (int x, int y)
        {
            for (int cy = -left; cy <= right; ++cy)
            {
                for (int cx = -left; cx <= right; ++cx)
                {
                    brush.SetPixel(x + cx, y + cy, color);
                }
            }

            return true;
        };

        Bresenham.Line(start.x + left, start.y + left, end.x + left, end.y + left, plot);

        return sprite;
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 start, end;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(Tilemap.transform as RectTransform, 
                                                                data.position - data.delta,
                                                                data.pressEventCamera,
                                                                out start);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(Tilemap.transform as RectTransform, 
                                                                data.position,
                                                                data.pressEventCamera,
                                                                out end);
        /*
        Vector2 size = end - start;

        int test = 16;

        var brush = BlankTexture.New(//test+1,
                                     (int) Mathf.Abs(size.x) + 1,
                                     (int) Mathf.Abs(size.y) + 1,
                                     new Color32(0, 0, 0, 0));

        var sprite = Sprite.Create(brush, new Rect(0, 0, brush.width, brush.height), Vector2.zero);
        var drawing = new SpriteDrawing(sprite);

        //drawing.Line(new Point(Vector2.zero), new Point(Vector2.right * test), Color);
        drawing.Line(new Point(start - tl), new Point(end - tl), Color);
        drawing.Apply();
        */

        if (Tool != ToolMode.Fill)
        {
            int left = Mathf.FloorToInt(Thickness / 2f);

            var tl = new Vector2(Mathf.Min(start.x, end.x),
                                 Mathf.Min(start.y, end.y));

            var sprite = LineBrush(new Point(start - tl), 
                                   new Point(end - tl), 
                                   Color, Thickness);

            //Tilemap.Line(new Point(start), new Point(end), Color);
            Tilemap.Blit(new Point(tl) - new Point(left, left), sprite, Tool == ToolMode.Eraser);
            Tilemap.Apply();
        }
    }
}
