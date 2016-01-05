using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;
using kooltool.Data;

namespace kooltool.Data
{
    [JsonObject(IsReference=false)]
    public struct Rect
    {
        public float x, y, w, h;

        public static implicit operator UnityEngine.Rect(Rect subtile)
        {
            return new UnityEngine.Rect(subtile.x, subtile.y, subtile.w, subtile.h);
        }

        public static implicit operator Rect(UnityEngine.Rect rect)
        {
            return new Rect
            {
                x = rect.x,
                y = rect.y,
                w = rect.width,
                h = rect.height,
            };
        }
    }

    public class Tile
    {
        public Tileset tileset;
        public List<Rect> subtiles = new List<Rect>();

        [JsonIgnore]
        public List<Sprite> sprites = new List<Sprite>();

        public void InitTest()
        {
            if (sprites.Count < subtiles.Count)
            {
                for (int i = sprites.Count; i < subtiles.Count; ++i)
                {
                    var sprite = Sprite.Create(tileset.texture, subtiles[i], Vector2.zero, 1f, 0U, SpriteMeshType.FullRect);
                    sprite.name = "(Tile)";

                    sprites.Add(sprite);
                }
            }
        }

        public TileInstance Instance()
        {
            InitTest();

            return new TileInstance { tile = this };
        }
    }

    public class Tileset : IResource
    {
        public Data.Texture texture;
        public List<Tile> tiles = new List<Tile>();

        void IResource.Load(Index index) { }
        void IResource.Save(Index index) { }

        public Tile TestTile()
        {
            int i = tiles.Count;

            var tile = new Tile
            {
                tileset = this,
                subtiles = new List<Rect> { new Rect { x = i * 32, y = 0, h = 32, w = 32 } },
            };

            var blank = PixelDraw.Brush.Rectangle(32, 32, new Color(Random.value, Random.value, Random.value, 1));

            tile.InitTest();

            PixelDraw.Brush.Apply(blank, Point.Zero, tile.sprites[0], Point.Zero, PixelDraw.Blend.Replace);
            tile.sprites[0].texture.Apply();

            tiles.Add(tile);

            return tile;
        }
    }
}
