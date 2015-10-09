using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool
{
    public class ProjectOld
    {
        public SparseGrid<bool> Grid { get; protected set; }

        public ProjectOld(Point gridsize)
        {
            Grid = new SparseGrid<bool>(gridsize.x, gridsize.y);
        }
    }
}
