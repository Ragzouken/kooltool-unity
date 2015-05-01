using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class Editor : MonoBehaviour
    {
        [SerializeField] protected AnimationCurve ZoomCurve;

        public RectTransform World;
        public Toolbox Toolbox;

        public Project Project { get; protected set; }

        public MapGenerator generator;

        public Layer Layer;

        public float Zoom { get; protected set; }

        protected Coroutine ZoomCoroutine;

        protected void Awake()
        {
            Project = new Project(new Point(32, 32));

            Toolbox.PixelTool = new PixelTool(this);
            Toolbox.TileTool = new TileTool(this);

            Toolbox.PixelTab.SetPixelTool(Toolbox.PixelTool);
            Toolbox.TileTab.SetTileTool(Toolbox.TileTool);

            ZoomTo(1f);
        }


        protected void Start()
        {
            SetProject(Project);

            generator.Go(Project);

            Toolbox.Hide();
        }

        protected void Update()
        {
            if (Project == null) return;

            if (Input.GetKeyDown(KeyCode.Space)) Toolbox.Show();
            if (Input.GetKeyUp(KeyCode.Space)) Toolbox.Hide();

            if (Input.GetKey(KeyCode.Alpha1)) Toolbox.PixelTab.SetSize(1);
            if (Input.GetKey(KeyCode.Alpha2)) Toolbox.PixelTab.SetSize(2);
            if (Input.GetKey(KeyCode.Alpha3)) Toolbox.PixelTab.SetSize(3);
            if (Input.GetKey(KeyCode.Alpha4)) Toolbox.PixelTab.SetSize(4);
            if (Input.GetKey(KeyCode.Alpha5)) Toolbox.PixelTab.SetSize(5);
            if (Input.GetKey(KeyCode.Alpha6)) Toolbox.PixelTab.SetSize(6);
            if (Input.GetKey(KeyCode.Alpha7)) Toolbox.PixelTab.SetSize(7);
            if (Input.GetKey(KeyCode.Alpha8)) Toolbox.PixelTab.SetSize(8);
            if (Input.GetKey(KeyCode.Alpha9)) Toolbox.PixelTab.SetSize(9);

            if (Input.GetKeyDown(KeyCode.LeftAlt)
             || Input.GetKeyDown(KeyCode.LeftShift))
            {
                Toolbox.TileTool.Tool = TileTool.ToolMode.Picker;
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scroll) > Mathf.Epsilon)
            {
                if (ZoomCoroutine != null) StopCoroutine(ZoomCoroutine);

                ZoomCoroutine = StartCoroutine(SmoothZoomTo(Zoom - scroll * 2, 0.125f));
            }
        }

        public void SetProject(Project project)
        {
            Project = project;

            Toolbox.SetProject(project);
        }

        public Vector2 ScreenToWorld(Vector2 screen)
        {
            Vector2 world;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(World, 
                                                                    screen,
                                                                    null,
                                                                    out world);
            return world;                                                   
        }

        public IEnumerator SmoothZoomTo(float zoom, float duration)
        {
            float start = Zoom;
            float end = Mathf.Clamp(zoom, 0, 2);
            float u = 0;
            float timer = 0;

            Vector2 focus = new Vector2(Camera.main.pixelWidth  * 0.5f,
                                        Camera.main.pixelHeight * 0.5f);


            if (start > end)
            {
                focus = Input.mousePosition;
            }

            Vector2 worlda = ScreenToWorld(focus);

            while (timer < duration)
            {
                yield return new WaitForEndOfFrame();

                timer += Time.deltaTime;

                ZoomTo(start + timer / duration * (end - start), focus);
            }

            ZoomTo(end, focus);
        }

        public void ZoomTo(float zoom, Vector2? focus = null)
        {
            Zoom = Mathf.Clamp(zoom, 0f, 2f);

            Vector2 screen = focus ?? Input.mousePosition;

            Vector2 worlda = ScreenToWorld(screen);
            World.localScale = (Vector3) (ZoomCurve.Evaluate(Zoom) * Vector2.one);
            Vector2 worldb = ScreenToWorld(screen);
            
            Pan(worldb - worlda);
        }

        public void Pan(Vector2 delta)
        {
            World.anchoredPosition += delta * World.localScale.x;
            /*
            World.localPosition = new Vector3(Mathf.Floor(World.localPosition.x),
                                              Mathf.Floor(World.localPosition.y),
                                              0f);*/
        }

        public void MakeCharacter(Costume costume)
        {
            var character = new Character(Point.Zero, costume);

            Layer.AddCharacter(character);
        }
    }
}
