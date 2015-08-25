using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

namespace kooltool.Editor.Modes
{
    public class Tile : Mode
    {
        private IObject hovering;
        private IObject dragging;
        private Vector2 dragPivot;

        public Tile(Editor editor) : base(editor)
        {
        }

        public override void Update()
        {
            highlights.Clear();

            hovering = editor.hovered.OfType<IObject>().FirstOrDefault();

            if (hovering != null) highlights.Add(hovering.HighlightParent);

            if (dragging != null)
            {
                dragging.Drag(dragPivot, editor.cursorWorld);
            }
        }

        public void SetDrag(IObject target, Vector2 pivot)
        {
            dragging = target;
            dragPivot = pivot;
        }

        public override void Enter()
        {

        }

        public override void Exit()
        {
            highlights.Clear();

            dragging = null;
        }

        public override void CursorInteractStart()
        {
            dragging = hovering;

            if (dragging != null)
            {
                dragPivot = dragging.DragPivot(editor.cursorWorld);
            }
        }

        public override void CursorInteractFinish()
        {
            dragging = null;
        }
    }
}
