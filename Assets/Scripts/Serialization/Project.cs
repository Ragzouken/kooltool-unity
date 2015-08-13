﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace kooltool.Serialization
{
    public class Project
    {
        public Index index;

        public Texture icon;

        public HashSet<Costume> costumes = new HashSet<Costume>();
        public Tileset tileset;
        public World world;
    }

    [JsonObject(IsReference=false)]
    public class Summary
    {
        public string title;
        public string description;
        public string icon;

        [JsonIgnore]
        public Sprite iconSprite;
        [JsonIgnore]
        public string folder;
        [JsonIgnore]
        public string path 
        { 
            get 
            { 
                return Application.persistentDataPath + "/" + folder; 
            } 
        }
    }

    public static class ProjectTools
    {
        public static Summary LoadSummary(string folder)
        {
            string path = string.Format("{0}/{1}",
                                        Application.persistentDataPath,
                                        folder);

            var summary = JsonWrapper.Deserialise<Summary>(System.IO.File.ReadAllText(path + "/summary.json"));
            var texture = new Texture2D(2, 2);

            texture.LoadImage(System.IO.File.ReadAllBytes(path + "/" + summary.icon));
            texture.filterMode = FilterMode.Point;
            summary.iconSprite = Sprite.Create(texture, new UnityEngine.Rect(0, 0, 128, 128), Vector2.zero);
            summary.folder = folder;

            return summary;
        }

        public static void SaveSummary(Summary summary)
        {
            Debug.Log(summary.path);

            System.IO.File.WriteAllBytes(summary.path + "/icon.png", summary.iconSprite.texture.EncodeToPNG());
            System.IO.File.WriteAllText(summary.path + "/summary.json", JsonWrapper.Serialise(summary));
        }

        public static Project LoadProject(Summary summary)
        {
            string path = string.Format("{0}/{1}",
                                        Application.persistentDataPath,
                                        summary.folder);

            var project = JsonWrapper.Deserialise<Project>(System.IO.File.ReadAllText(path + "/project.json"));

            project.index.folder = summary.folder;
            project.index.Load();

            return project;
        }

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

            project.world.tileset = project.tileset;
            project.world.layers.Add(new Layer
            {
                tileset = project.tileset,
            });

            return project;
        }
    }
}
