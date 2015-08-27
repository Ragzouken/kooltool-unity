using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor.Modes
{
    public class Draw : Mode
    {
        public enum Tool
        {
            Pencil,
            Fill,
            Line,
        }

        private readonly PixelCursor cursor;
        private readonly PixelTool tool_;

        public Tool tool { get; private set; }

        public Draw(Editor editor, 
                    PixelCursor cursor,
                    PixelTool tool) : base(editor)
        {
            this.cursor = cursor;
            this.tool_ = tool;

            cursor.Tool = tool;
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
            var rtrans = cursor.transform as RectTransform;
            var offset = Vector2.one * ((tool_.Thickness % 2 == 1) ? 0.5f : 0);

            cursor.end = editor.currCursorWorld;
            rtrans.anchoredPosition = cursor.end.Round() + offset;

            cursor.Refresh();
        }

        public void SetTool(Tool tool)
        {
            this.tool = tool;
        }
    }
}
