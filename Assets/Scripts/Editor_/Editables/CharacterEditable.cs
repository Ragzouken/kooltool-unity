using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class CharacterEditable : Editable, IObject, IDrawable
    {
        [SerializeField] private CharacterDrawing drawing;
        [SerializeField] private RectTransform highlightParent;

        RectTransform IObject.OverlayParent
        {
            get
            {
                return highlightParent;
            }
        }

        Vector2 IObject.DragPivot(Vector2 world)
        {
            return world - (Vector2) drawing.transform.localPosition;
        }

        void IObject.Drag(Vector2 pivot, Vector2 world)
        {
            Point grid, offset;

            drawing.editor.Project.Grid.Coords(new Point(world - pivot), out grid, out offset);

            drawing.Character.SetPosition((Vector2) grid * 32f + Vector2.one * 16f);
        }

        void IObject.Remove()
        {
            drawing.editor.RemoveCharacter(drawing.Character);
        }

        PixelDraw.IDrawing IDrawable.Drawing
        {
            get
            {
                return drawing;
            }
        }
    }
}