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
        private readonly Transform originalCursor;

        private IDrawable hovering;
        public IDrawable drawing;
        public Vector2 start;

        public Color paintColour = Color.magenta;
        public Tool tool;
        public int thickness = 1;

        public Sprite brush;

        private HashSet<Editable> obstructions = new HashSet<Editable>();

        private Vector2 currCursor;
        private Vector2 prevCursor;

        private CharacterEditable highlighted;

        public override IconSettings.Icon CursorIcon
        {
            get
            {
                if (Pick)                return IconSettings.Icon.PickCursor;
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
                return tool == Tool.Pick
                    || Input.GetKey(KeyCode.LeftAlt) 
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
            if (highlighted != null) highlighted.ShowBorder = false;
        }

        public override void Update()
        {
            foreach (Editable obstruction in obstructions)
            {
                obstruction.targetAlpha = 1f;
            }

            obstructions.Clear();

            if (drawing != null)
            { 
                obstructions.UnionWith(editor.hovered.TakeWhile(e => e != drawing));
            }

            foreach (Editable obstruction in obstructions)
            {
                obstruction.targetAlpha = 0.125f;
            }

            highlights.Clear();

            brush = Brush.Circle(thickness, Erase ? Editor.GetFlashColour() : paintColour);
            brush.texture.Apply();

            hovering = editor.hovered.OfType<IDrawable>().FirstOrDefault();

            var rtrans = cursor.transform as RectTransform;

            var @object = (drawing ?? hovering) as IObject;

            if (@object != null)
            {
                highlights.Add(@object.OverlayParent);

                editor.objectOverlay.SetSubject(@object as CharacterEditable);
            }

            currCursor = editor.currCursorWorld;

            var editable = (drawing ?? hovering) as Editable;

            if (editable != null)
            {
                currCursor = editor.WorldCursor(editable.transform as RectTransform);

                //cursor.transform.SetParent(editable.CursorParent, true);
                rtrans.position = (currCursor - Vector2.one).Round();
                var local = rtrans.localPosition;
                local.z = 0;
                rtrans.localPosition = local;
            }

            var character = (drawing ?? hovering) as CharacterEditable;

            if (character != null
             && character != highlighted)
            {
                if (highlighted != null) highlighted.ShowBorder = false;
                highlighted = character;
                highlighted.ShowBorder = true;
            }
            else if (character == null
                  && highlighted != null)
            {
                highlighted.ShowBorder = false;
                highlighted = null;
            }

            cursor.correct = tool == Tool.Line;
            cursor.colour = Erase ? Editor.GetFlashColour()
                                  : paintColour; 
            
            Color color = Erase ? Color.white : paintColour;
            var blend = Erase ? Blend.Subtract
                              : Blend.Alpha;

            if (tool == Tool.Pencil && drawing != null)
            {
                drawing.Drawing.DrawLine(currCursor.Round(), 
                                         prevCursor.Round(), 
                                         thickness, color, blend);
                drawing.Drawing.Apply();
            }
            else if (tool == Tool.Line && drawing != null)
            {
                brush = Brush.Line(start.Round(),
                                   currCursor.Round(),
                                   color,
                                   thickness);
                brush.texture.Apply();
            }

            cursor.preview = brush;
            cursor.Refresh();

            prevCursor = currCursor;
        }

        public override void CursorInteractStart()
        {
            base.CursorInteractStart();

            if (Pick)
            {
                if (!hovering.Drawing.Sample(currCursor, out paintColour))
                {
                    paintColour = Color.clear;
                }
            }
            else if (tool == Tool.Pencil || tool == Tool.Line)
            {
                if (hovering != null)
                {
                    start = currCursor;
                    drawing = hovering;
                }
            }
            else if (tool == Tool.Fill)
            {
                hovering.Drawing.Fill(currCursor, paintColour);
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
                                         currCursor.Round(), 
                                         thickness, color, blend);
                drawing.Drawing.Apply();
            }

            drawing = null;

            Editor.Instance.Do();
        }

        public void SetTool(Tool tool)
        {
            this.tool = tool;
        }
    }
}
