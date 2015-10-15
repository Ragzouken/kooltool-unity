using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{   
    public interface IDrawable
    {
        PixelDraw.IDrawing Drawing { get; }
    }
}
