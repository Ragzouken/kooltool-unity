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

        public Tileset tileset;
        public Grid tiles = new Grid();
    }

    public class World
    {
        public Tileset tileset;
        public List<Layer> layers = new List<Layer>();
    }
}
