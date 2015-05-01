using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class Editor : MonoBehaviour
    {
        public RectTransform World;
        public Toolbox Toolbox;

        public Project Project { get; protected set; }

        public MapGenerator generator;

        public Layer Layer;

        protected void Awake()
        {
            Project = new Project(new Point(32, 32));

            Toolbox.PixelTool = new PixelTool(this);
            Toolbox.TileTool = new TileTool(this);

            Toolbox.PixelTab.SetPixelTool(Toolbox.PixelTool);
            Toolbox.TileTab.SetTileTool(Toolbox.TileTool);
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
                Zoom(scroll);
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

        public void Zoom(float delta)
        {
            Vector2 screen = Input.mousePosition;

            Vector2 worlda = ScreenToWorld(screen);
            World.localScale += (Vector3) (delta * Vector2.one);
            Vector2 worldb = ScreenToWorld(screen);
            
            Pan((worldb - worlda) * World.localScale.x);
        }

        public void PanTo(Vector2 position)
        {
            World.anchoredPosition = position;
        }

        public void Pan(Vector2 delta)
        {
            World.anchoredPosition += delta;
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
