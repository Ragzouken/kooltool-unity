using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using kooltool.Data;

namespace kooltool.Editor
{
    public class NoteboxView : Editable, IObject
    {
        public Editor editor;

        [SerializeField] private Text text;

        private Notebox notebox;

        RectTransform IObject.OverlayParent
        {
            get
            {
                return transform as RectTransform;
            }
        }

        public void SetNotebox(Notebox notebox)
        {
            this.notebox = notebox;
            text.text = notebox.text;
        }

        Vector2 IObject.DragPivot(Vector2 world)
        {
            return world - (Vector2) transform.localPosition;
        }

        void IObject.Drag(Vector2 pivot, Vector2 world)
        {
            transform.localPosition = world - pivot;
            notebox.position = world - pivot;
        }

        void IObject.Remove()
        {
            editor.RemoveNotebox(notebox);
        }
    }
}
