using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class CharacterEditable : Editable, IObject, IDrawable
    {
        [SerializeField] public CharacterDrawing drawing;
        [SerializeField] private RectTransform highlightParent;

        RectTransform IObject.OverlayParent
        {
            get
            {
                return highlightParent;
            }
        }

        IEnumerable<ObjectAction> IObject.Actions
        {
            get
            {
                yield return new ObjectAction
                {
                    icon = IconSettings.Icon.OpenScript,
                    action = () => Editor.Instance.EditScript(drawing.Character),
                    tooltip = "toggle script panel",
                };

                yield return new ObjectAction
                {
                    icon = IconSettings.Icon.RemoveObject,
                    action = () => Editor.Instance.RemoveCharacter(drawing.Character),
                    tooltip = "remove character",
                };
            }
        }

        Vector2 IObject.DragPivot(Vector2 world)
        {
            return world - (Vector2) drawing.transform.localPosition;
        }

        void IObject.Drag(Vector2 pivot, Vector2 world)
        {
            Point grid, offset;

            Editor.Instance.Project.Grid.Coords(new Point(world - pivot), out grid, out offset);

            drawing.Character.SetPosition((Vector2) grid * 32f + Vector2.one * 16f);
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