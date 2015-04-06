using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool
{
    public class Project
    {
        // meta data
        public string Name = "kooltool project";
        public string Decription = "a new project";
        public SpriteDrawing Icon { get; protected set; }
        public SparseGrid<bool> Grid { get; protected set; }

        // resources
        public Tileset Tileset { get; protected set; }
        public ICollection<Costume> Sprites
            = new HashSet<Costume>();

        public Project(Point gridsize)
        {
            Grid = new SparseGrid<bool>(gridsize.x, gridsize.y);

            Tileset = new Tileset();
        }
    }
}
