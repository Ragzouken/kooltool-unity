using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{   
    public interface IObject
    {
        RectTransform HighlightParent { get; }

        Vector2 DragPivot(Vector2 world);
        
        void Drag(Vector2 pivot, Vector2 world);

        //void Remove();
    }
}
