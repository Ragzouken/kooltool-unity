using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool
{
    public class Project
    {
        public SparseGrid<bool> Grid { get; protected set; }

        public Project(Point gridsize)
        {
            Grid = new SparseGrid<bool>(gridsize.x, gridsize.y);
        }
    }
}
