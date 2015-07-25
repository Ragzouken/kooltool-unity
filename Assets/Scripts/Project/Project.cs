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
        public readonly IList<Costume> Costumes;

        // test
        public readonly IList<Character> Characters;

        public Project(Point gridsize)
        {
            Grid = new SparseGrid<bool>(gridsize.x, gridsize.y);

            Costumes = new List<Costume>();

            Characters = new List<Character>();

            Costumes.Add(Generators.Costume.Smiley(Grid.CellWidth, Grid.CellHeight));
            Costumes.Add(Generators.Costume.Smiley(Grid.CellWidth, Grid.CellHeight));
        }
    }
}
