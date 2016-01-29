using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using kooltool.Data;

namespace kooltool
{
    public class Main : MonoBehaviour
    {
        [SerializeField] private Editor.Editor editor;
        [SerializeField] private Player.Player player;
        [SceneOnly][SerializeField] private MainMenu menu;

        [Header("World")]
        [SerializeField] private Transform worldContainer;
        [SerializeField] private WorldView worldPrefab;

        private WorldView world; 

        private void Start()
        {
            player.gameObject.SetActive(false);
            editor.gameObject.SetActive(false);
            menu.gameObject.SetActive(false);

            Project auto;

            if (FindEmbedded(out auto))
            {
                player.gameObject.SetActive(true);
            }
            else
            {
                OpenMenu();
            }
        }

        private bool FindEmbedded(out Project project)
        {
            if (System.IO.File.Exists(Application.dataPath + "/autoplay/summary.json"))
            {
                var summary = ProjectTools.LoadSummary("autoplay", Application.dataPath);

                project = ProjectTools.LoadProject(summary);
                return true;
            }

            project = null;
            return false;
        }

        public void SetPlayer(Project project)
        {
            editor.gameObject.SetActive(false);
            player.gameObject.SetActive(true);

            player.Setup(project);
        }

        public void SetEditor(Project project)
        {
            editor.SetProject(project);
            editor.gameObject.SetActive(true);
            player.gameObject.SetActive(false);
        }

        public WorldView CreateWorld(World world_)
        {
            if (world != null)
            {
                Destroy(world.gameObject);
            }

            world = Instantiate(worldPrefab);
            world.transform.SetParent(worldContainer, false);
            world.SetWorld(world_);

            return world;
        }

        public void OpenMenu()
        {
            menu.gameObject.SetActive(true);
        }

        public void CloseMenu()
        {
            menu.gameObject.SetActive(false);
        }

        public void Blank()
        {
            CloseMenu();

            editor.gameObject.SetActive(true);
            editor.SetProject(ProjectTools.Blank());
            editor.Do();
        }

        public void Generate()
        {
            CloseMenu();

            editor.gameObject.SetActive(true);
            editor.SetProject(ProjectTools.Blank());
            editor.Do();

            FindObjectOfType<MapGenerator>().Go(editor.project_, FindObjectOfType<Tilemap>());
        }

        public void Load(string name)
        {
            CloseMenu();

            editor.gameObject.SetActive(true);
            editor.SetProject(ProjectTools.LoadProject2(name).project);
            editor.Do();
        }

        public void Browse()
        {
        }

        public void Exit()
        {
            // TODO: check for saves etc

            CloseMenu();

            Application.Quit();
        }
    }
}
