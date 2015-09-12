using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using PixelDraw;

namespace kooltool.Editor.Modes
{
    public class Notes : Mode
    {
        public enum Tool
        {
            Pencil,
            Line,
        }

        private readonly PixelCursor cursor;

        private IDrawable hovering;
        public IDrawable drawing;
        public Vector2 start;

        public Tool tool;
        public int thickness = 1;

        public Sprite brush;

        public bool erase;

        public Notes(Editor editor,
                     PixelCursor cursor)
            : base(editor)
        {
            this.cursor = cursor;
        }

        public override void Enter()
        {
            cursor.gameObject.SetActive(true);
        }

        public override void Exit()
        {
            cursor.gameObject.SetActive(false);
        }

        public override void Update()
        {
            highlights.Clear();

            hovering = editor.hovered.OfType<IAnnotatable>().FirstOrDefault().Hack;

            var @object = (drawing ?? hovering) as IObject;

            if (@object != null)  highlights.Add(@object.HighlightParent);

            var rtrans = cursor.transform as RectTransform;
            var offset = Vector2.one * ((thickness % 2 == 1) ? 0.5f : 0);

            cursor.end = editor.currCursorWorld;
            rtrans.anchoredPosition = cursor.end.Round() + offset;

            cursor.colour = Editor.GetFlashColour();
            cursor.Refresh();

            var blend = erase ? Blend.Subtract
                              : Blend.Alpha;

            if (tool == Tool.Pencil && drawing != null)
            {
                drawing.Drawing.DrawLine(editor.currCursorWorld.Round(),
                                         editor.prevCursorWorld.Round(),
                                         thickness, Color.white, blend);
                drawing.Drawing.Apply();
            }
            else if (tool == Tool.Line && drawing != null)
            {
                brush = Brush.Line(start.Round(),
                                   editor.currCursorWorld.Round(),
                                   Color.white,
                                   thickness);

                brush.texture.Apply();
            }
        }

        public override void CursorInteractStart()
        {
            base.CursorInteractStart();

            if (tool == Tool.Pencil || tool == Tool.Line)
            {
                if (hovering != null)
                {
                    start = editor.currCursorWorld;
                    drawing = hovering;
                }
            }
        }

        public override void CursorInteractFinish()
        {
            base.CursorInteractFinish();

            var blend = erase ? Blend.Subtract
                              : Blend.Alpha;

            if (tool == Tool.Line && drawing != null)
            {
                drawing.Drawing.DrawLine(start.Round(),
                                         editor.currCursorWorld.Round(),
                                         thickness, Color.white, blend);
                drawing.Drawing.Apply();
            }

            drawing = null;
        }

        public void SetTool(Tool tool)
        {
            this.tool = tool;
        }
    }
}
