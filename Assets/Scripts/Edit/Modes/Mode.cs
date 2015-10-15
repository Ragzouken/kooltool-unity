using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor.Modes
{
    public abstract class Mode
    {
        public readonly Editor editor;
        public readonly List<RectTransform> highlights
            = new List<RectTransform>();

        public Mode(Editor editor)
        {
            this.editor = editor;
        }

        public virtual void Update()
        {
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void CursorInteractStart()
        {
        }

        public virtual void CursorInteractFinish()
        {
        }
    }
}
