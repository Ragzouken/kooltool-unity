using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace kooltool.Data
{
    public class Project
    {
        public Index index;

        public Texture icon;

        public HashSet<Costume> costumes = new HashSet<Costume>();

        public Tileset tileset;
        public Regions regions;

        public World world;
    }

    [JsonObject(IsReference=false)]
    public class Summary
    {
        public string title;
        public string description;
        public string icon;

        public Point position;

        [JsonIgnore]
        public Sprite iconSprite;
        [JsonIgnore]
        public string folder;
        [JsonIgnore]
        public string root;
        [JsonIgnore]
        public string path 
        { 
            get 
            { 
                return (root ?? Application.persistentDataPath) + "/" + folder; 
            } 
        }
    }

    public static class ProjectTools
    {
        public static Summary LoadSummary(string folder, string root = null)
        {
            root = root ?? Application.persistentDataPath;

            string path = string.Format("{0}/{1}",
                                        root,
                                        folder);

            var summary = JsonWrapper.Deserialise<Summary>(System.IO.File.ReadAllText(path + "/summary.json"));
            var texture = new Texture2D(2, 2);

            texture.LoadImage(System.IO.File.ReadAllBytes(path + "/" + summary.icon));
            texture.filterMode = FilterMode.Point;
            summary.iconSprite = texture.FullSprite();
            summary.folder = folder;
            summary.root = root;

            return summary;
        }

        public static void SaveSummary(Summary summary)
        {
            Debug.Log(summary.path);

            System.IO.File.WriteAllBytes(summary.path + "/icon.png", summary.iconSprite.texture.EncodeToPNG());
            System.IO.File.WriteAllText(summary.path + "/summary.json", JsonWrapper.Serialise(summary));
        }

        public static void CreateProject(Summary summary)
        {
            System.IO.Directory.CreateDirectory(summary.path);

            var project = ProjectTools.Blank();
            project.index.folder = summary.folder;
            project.index.root = summary.root;

            SaveSummary(summary);
            project.index.Save(project);
        }

        public static void DeleteProject(Summary summary)
        {
            System.IO.Directory.Delete(summary.path, recursive: true);
        }

        public static Project LoadProject(Summary summary)
        {
            var project = JsonWrapper.Deserialise<Project>(System.IO.File.ReadAllText(summary.path + "/project.json"));

            project.index.folder = summary.folder;
            project.index.root = summary.root;
            project.index.Load();

            return project;
        }

        #region Bad And Wrong

        private static Character BadAndWrongCopy(Character character)
        {
            return new Character
            {
                costume = character.costume,
                dialogue = character.dialogue,
                name = character.name,
                position = character.position,
                _position = character.position,
            };
        }

        private static Layer BadAndWrongCopy(Layer layer)
        {
            return new Layer
            {
                annotations = layer.annotations,
                drawing = layer.drawing,
                noteboxes = layer.noteboxes,
                tiles = layer.tiles,
                tileset = layer.tileset,
                world = layer.world,
                characters = new HashSet<Character>(layer.characters.Select(c => BadAndWrongCopy(c))),
            };
        }

        private static World BadAndWrongCopy(World world, Project project)
        {
            return new World
            {
                tileset = world.tileset,
                project = project,
                layers = world.layers.Select(l => BadAndWrongCopy(l)).ToList(),
            };
        }

        public static Project BadAndWrongCopy(Project project)
        {
            var copy = new Project();

            copy.icon = project.icon;
            copy.costumes = project.costumes;
            copy.regions = project.regions;
            copy.tileset = project.tileset;
            copy.world = BadAndWrongCopy(project.world, copy);

            return copy;
        }

        #endregion

        public static Project Blank()
        {
            var index = new Index();

            index.folder = "test";

            var project = new Project
            {
                index = index,

                icon = index.CreateTexture(32, 32),

                tileset = new Tileset
                {
                    texture = index.CreateTexture(1024, 1024),
                },

                world = new World(),
            };

            project.tileset.TestTile();
            project.tileset.TestTile();

            project.world.project = project;
            project.world.tileset = project.tileset;
            project.world.AddLayer();

            var icon = project.index.CreateTexture(32, 32);
            var blank = PixelDraw.Brush.Rectangle(32, 32, new Color(Random.value, Random.value, Random.value, 1));
            
            PixelDraw.Brush.Apply(blank, Point.Zero, icon.texture.FullSprite(), Point.Zero, PixelDraw.Blend.Replace);
            icon.texture.Apply();

            var wall = new Region
            {
                name = "wall",
                icon = icon,
            };

            project.regions = new Regions();
            project.regions.Add(wall);

            project.regions.AddDefault(project.tileset.tiles[1], wall);

            return project;
        }

        public static void RemoveCostume(this Project project,
                                         Costume costume)
        {
            project.costumes.Remove(costume);
        }

        public static void RemoveTile(this Project project,
                                      Tile tile)
        {
            project.tileset.tiles.Remove(tile);

            if (project.tileset.tiles.Count == 0)
            {
                project.tileset.TestTile();
            }

            var replace = project.tileset.tiles[0];
            var changes = new Dictionary<Point, TileInstance>();

            foreach (var layer in project.world.layers)
            {
                changes.Clear();

                foreach (var pair in layer.tiles)
                {
                    TileInstance instance = pair.Value;

                    if (instance.tile == tile)
                    {
                        instance.tile = replace;

                        changes.Add(pair.Key, instance);
                    }
                }

                foreach (var change in changes)
                {
                    layer.tiles[change.Key] = change.Value;
                }
            }
        }
    }
}
