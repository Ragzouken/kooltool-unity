using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using System.Xml.Serialization;
using Newtonsoft.Json;

namespace kooltool.Serialization
{
    [JsonObject(IsReference=false)]
    public struct TileInstance
    {
        public Tile tile;
        public string variant;
    }

    public class Layer
    {
        [JsonArray]
        public class Grid : Dictionary<Point, TileInstance> { }
        [JsonArray]
        public class Drawing : Dictionary<Point, Texture> { }

        public World world;

        public Tileset tileset;
        public Grid tiles = new Grid();
        public Drawing drawing = new Drawing();
        public Drawing annotations = new Drawing();

        public HashSet<Character> characters = new HashSet<Character>();
    }

    public class World
    {
        public Project project;
        public Tileset tileset;
        public List<Layer> layers = new List<Layer>();

        public Layer AddLayer()
        {
            var layer = new Layer
            {
                world = this,
                tileset = tileset,
            };

            layers.Add(layer);

            return layer;
        }
    }
}
