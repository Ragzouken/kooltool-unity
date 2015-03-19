using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool
{
    public class Project
    {
        public string Name = "kooltool project";
        public string Decription = "a new project";

        public SpriteDrawing Icon { get; protected set; }

        public SparseGrid<bool> Grid { get; protected set; }

        public ICollection<Sprite> Sprites
            = new HashSet<Sprite>();

        public Project(Point gridsize)
        {
            Grid = new SparseGrid<bool>(gridsize.x, gridsize.y);
        }
    }
}
