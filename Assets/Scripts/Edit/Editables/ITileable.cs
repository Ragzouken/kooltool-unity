using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{   
    public interface ITileable
    {
        Tilemap Tilemap { get; }

        void Demote(Point cell);
        void Promote(Point cell);
    }
}
