using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using PixelDraw;

namespace kooltool.Editor.Modes
{
    public class Draw : Mode
    {
        public enum Tool
        {
            Pencil,
            Pick,
            Fill,
            Line,
        }

        private readonly PixelCursor cursor;

        private IDrawable hovering;
        public IDrawable drawing;
        public Vector2 start;

        public Color paintColour = Color.magenta;
        public Tool tool;
        public int thickness = 1;

        public Draw(Editor editor, 
                    PixelCursor cursor) : base(editor)
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

            hovering = editor.hovered.OfType<IDrawable>().FirstOrDefault();

            var @object = (drawing ?? hovering) as IObject;

            if (@object != null) highlights.Add(@object.HighlightParent);

            var rtrans = cursor.transform as RectTransform;
            var offset = Vector2.one * ((thickness % 2 == 1) ? 0.5f : 0);

            cursor.end = editor.currCursorWorld;
            rtrans.anchoredPosition = cursor.end.Round() + offset;

            cursor.Refresh();

            bool erase = paintColour.a == 0;

            Color color = erase ? Color.white : paintColour;
            var blend = erase ? Blend.Subtract
                              : Blend.Alpha;

            if (tool == Tool.Pencil && drawing != null)
            {
                drawing.Drawing.DrawLine(editor.currCursorWorld.Round(), 
                                         editor.prevCursorWorld.Round(), 
                                         thickness, color, blend);
                drawing.Drawing.Apply();
            }
            else if (tool == Tool.Line && drawing != null)
            {
                drawing.Drawing.DrawLine(start, 
                                         editor.currCursorWorld, 
                                         thickness, color, blend);
                drawing.Drawing.Apply();
            }
        }

        public override void CursorInteractStart()
        {
            base.CursorInteractStart();

            if (tool == Tool.Pencil || tool == Tool.Line)
            {
                if (hovering != null)
                {
                    drawing = hovering;
                }
            }
            else if (tool == Tool.Pick)
            {
                if (!hovering.Drawing.Sample(editor.currCursorWorld, out paintColour))
                {
                    paintColour = new Color(0, 0, 0, 0);
                }
            }
            else if (tool == Tool.Fill)
            {
                hovering.Drawing.Fill(editor.currCursorWorld, paintColour);
                hovering.Drawing.Apply();
            }
        }  

        public override void CursorInteractFinish()
        {
            base.CursorInteractFinish();

            drawing = null;
        }

        public void SetTool(Tool tool)
        {
            this.tool = tool;
        }
    }
}
