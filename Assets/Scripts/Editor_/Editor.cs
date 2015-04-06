using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class Editor : MonoBehaviour
    {
        [SerializeField] protected Toolbox Toolbox;

        public Project Project { get; protected set; }

        public MapGenerator generator;

        protected void Awake()
        {
            Project = new Project(new Point(32, 32));
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
        }

        public void SetProject(Project project)
        {
            Project = project;

            Toolbox.SetProject(project);
        }
    }
}
