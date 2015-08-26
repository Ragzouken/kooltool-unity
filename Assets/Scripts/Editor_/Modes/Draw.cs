using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor.Modes
{
    public class Draw : Mode
    {
        private readonly PixelCursor cursor;
        private readonly PixelTool tool;

        public Draw(Editor editor, 
                    PixelCursor cursor,
                    PixelTool tool) : base(editor)
        {
            this.cursor = cursor;
            this.tool = tool;
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
            var offset = Vector2.one * ((tool.Thickness % 2 == 1) ? 0.5f : 0);

            cursor.end = editor.cursorWorld;
            rtrans.anchoredPosition = cursor.end.Round() + offset;
        }
    }
}
