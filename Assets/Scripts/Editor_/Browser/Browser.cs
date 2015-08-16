using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using kooltool.Serialization;

namespace kooltool.Editor
{
    public class Browser : MonoBehaviour
    {
        public event System.Action<Summary> OnConfirmed = delegate { };

        private MonoBehaviourPooler<Summary, ProjectTile> summaries;

        private Dictionary<Point, Summary> p2s = new Dictionary<Point, Summary>();
        private Dictionary<Summary, Point> s2p = new Dictionary<Summary, Point>();

        [Header("Grid")]
        [SerializeField] private RectTransform panelContainer;
        [SerializeField] private ProjectTile panelPrefab;

        [Header("New Project")]
        [SerializeField] private CreateTile createProjectButton;
        [SerializeField] private WorldCamera wcamera;

        private SparseGrid<bool> grid = new SparseGrid<bool>(256);
        private Summary summary;

        private void Awake()
        {
            summaries = new MonoBehaviourPooler<Summary, ProjectTile>(panelPrefab,
                                                                      panelContainer,
                                                                      InitialisePanel);

            createProjectButton.OnClickedCreate = CreateProject;
        }

        private void InitialisePanel(Summary summary, ProjectTile panel)
        {
            Point cell = Point.Right * p2s.Count;

            p2s[cell] = summary;
            s2p[summary] = cell;

            panel.OnClickedOpen = s => OnConfirmed(s);
            panel.OnClickedDelete = s => DeleteSummary(s);
            panel.SetSummary(summary);
            panel.transform.position = cell * 256;
        }

        private void Update()
        {
            Vector2 cursor = wcamera.ScreenToWorld(Input.mousePosition)
                           + Vector2.one * 128;

            Point g, o;

            grid.Coords(cursor, out g, out o);

            Vector2 distance = cursor - (Vector2) (g * 256 + Point.One * 128);
            float d = Mathf.Max(Mathf.Abs(distance.x), Mathf.Abs(distance.y));
            
            float u = 1 - Mathf.Clamp01((d - 64) / 32f);

            //if (d <= 64) u = 1;

            createProjectButton.transform.position = g * 256;
            createProjectButton.gameObject.SetActive(!p2s.ContainsKey(g));
            createProjectButton.alpha = u;
        }

        private void CreateProject()
        {
            Vector2 cursor = wcamera.ScreenToWorld(Input.mousePosition)
               + Vector2.one * 128;

            Point g, o;

            grid.Coords(cursor, out g, out o);

            Summary summary = new Summary
            {
                folder = System.Guid.NewGuid().ToString(),
                title = "test",
                description = "test",
                iconSprite = PixelDraw.Brush.Rectangle(128, 128, new Color(Random.value, Random.value, Random.value)),
                icon = "icon.png",
            };

            ProjectTools.CreateProject(summary);

            var panel = summaries.Get(summary);
            panel.transform.position = g * 256;
        }

        private void DeleteSummary(Summary summary)
        {
            ProjectTools.DeleteProject(summary);
            summaries.Discard(summary);

            var point = s2p[summary];
            s2p.Remove(summary);
            p2s.Remove(point);
        }

        public void Refresh()
        {
            summaries.SetActive(GetSummaries());
        }

        private IEnumerable<Summary> GetSummaries()
        {
            var directory = new DirectoryInfo(Application.persistentDataPath);

            foreach (var project in directory.GetDirectories())
            {
                yield return ProjectTools.LoadSummary(project.Name);
            }
        }
    }
}
