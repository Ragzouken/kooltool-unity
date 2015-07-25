using UnityEngine;
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

        public Tileset tileset;
        public World world;
    }

    public static class ProjectTools
    {
        public static Project Blank()
        {
            var index = new Index();

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

            return project;
        }
    }
}
