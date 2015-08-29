using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;
using System.Linq;

namespace kooltool.Editor
{
    public class PixelTool : ITool
    {
        public enum ToolMode
        {
            Pencil,
            Fill,
            Line,
        }

        protected Editor Editor;

        public ToolMode Tool;

        public int Thickness = 1;
        public Color Color = Color.red;

        public IDrawing Target;

        public bool dragging;
        public Vector2 start;

        public PixelTool(Editor editor)
        {
            Editor = editor;
        }

        public void SetErase()
        {
            Color = Color.clear;
        }

        public void BeginStroke(Vector2 start)
        {
            start = start.Round();

            Target = Editor.hovered.OfType<IDrawable>().First().Drawing;

            dragging = false;

            if (Tool == ToolMode.Line)
            {
                this.start = start;
            }

            dragging = true;
        }

        public void ContinueStroke(Vector2 start, Vector2 end)
        {
        }

        public void EndStroke(Vector2 end)
        {
            if (Tool == ToolMode.Line)
            {
                Color color = Color.a > 0 ? Color : Color.white;
                var blend = Color.a == 0 ? Blend.Subtract : Blend.Alpha;
                
                Target.DrawLine(this.start, end, Thickness, color, blend);

                Target.Apply();
            }

            dragging = false;
        }
    }
}