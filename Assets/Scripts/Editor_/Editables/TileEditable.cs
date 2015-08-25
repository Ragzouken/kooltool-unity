using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class TileEditable : Editable, IDrawable
    {
        public PixelDraw.TiledDrawing drawing;

        PixelDraw.IDrawing IDrawable.Drawing
        {
            get
            {
                return drawing;
            }
        }
    }
}