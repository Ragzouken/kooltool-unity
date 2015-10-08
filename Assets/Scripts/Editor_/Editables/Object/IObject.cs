using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{   
    public interface IObject
    {
        RectTransform OverlayParent { get; }

        Vector2 DragPivot(Vector2 world);
        
        void Drag(Vector2 pivot, Vector2 world);

        IEnumerable<ObjectAction> Actions
        {
            get;
        }
    }

    public class ObjectAction
    {
        public IconSettings.Icon icon;
        public System.Action action;
    }
}
