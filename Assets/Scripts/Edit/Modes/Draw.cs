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
        public static Color eraseColour = Color.clear;

        public enum Tool
        {
            Pencil,
            Pick,
            Fill,
            Line,
            Stamp,
        }

        private readonly PixelCursor cursor;

        private IDrawable hovering;
        public IDrawable drawing;
        public Vector2 start;

        public Color paintColour = Color.magenta;
        public Tool tool;
        public int thickness = 1;

        public Sprite brush;

        public override IconSettings.Icon CursorIcon
        {
            get
            {
                if (tool == Tool.Pick || Pick) return IconSettings.Icon.PickCursor;
                if (tool == Tool.Pencil) return IconSettings.Icon.PencilCursor;
                if (tool == Tool.Fill)   return IconSettings.Icon.FillCursor;
                if (tool == Tool.Line)   return IconSettings.Icon.LineCursor;

                return base.CursorIcon;
            }
        }

        public bool Pick
        {
            get
            {
                return Input.GetKey(KeyCode.LeftAlt) 
                    || Input.GetKey(KeyCode.RightAlt);
            }
        }

        public bool Erase
        {
            get
            {
                return paintColour == eraseColour;
            }
        }

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

            brush = Brush.Circle(thickness, Erase ? Editor.GetFlashColour() : paintColour);
            brush.texture.Apply();

            hovering = editor.hovered.OfType<IDrawable>().FirstOrDefault();

            var @object = (drawing ?? hovering) as IObject;

            if (@object != null) highlights.Add(@object.OverlayParent);

            var rtrans = cursor.transform as RectTransform;
            var offset = Vector2.one * ((thickness % 2 == 1) ? 0.5f : 0);

            rtrans.anchoredPosition = (editor.currCursorWorld - offset).Round();

            cursor.correct = tool == Tool.Line;
            cursor.colour = Erase ? Editor.GetFlashColour()
                                  : paintColour; 
            
            Color color = Erase ? Color.white : paintColour;
            var blend = Erase ? Blend.Subtract
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
                brush = Brush.Line(start.Round(),
                                   editor.currCursorWorld.Round(),
                                   color,
                                   thickness);
                brush.texture.Apply();
            }

            cursor.preview = brush;
            cursor.Refresh();
        }

        public override void CursorInteractStart()
        {
            base.CursorInteractStart();

            if (tool == Tool.Pick || Pick)
            {
                if (!hovering.Drawing.Sample(editor.currCursorWorld, out paintColour))
                {
                    paintColour = Color.clear;
                }
            }
            else if (tool == Tool.Pencil || tool == Tool.Line)
            {
                if (hovering != null)
                {
                    start = editor.currCursorWorld;
                    drawing = hovering;
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

            Color color = Erase ? Color.white : paintColour;
            var blend = Erase ? Blend.Subtract
                              : Blend.Alpha;

            if (tool == Tool.Line && drawing != null)
            {
                drawing.Drawing.DrawLine(start.Round(), 
                                         editor.currCursorWorld.Round(), 
                                         thickness, color, blend);
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
