using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class DrawingEditable : Editable, IDrawable
    {
        public PixelDraw.IDrawing drawing;

        PixelDraw.IDrawing IDrawable.Drawing
        {
            get
            {
                return drawing;
            }
        }
    }
}