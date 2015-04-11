using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool
{
    public class Costume
    {
        public string Name { get; protected set; }
        public Sprite Sprite { get; protected set; }

        public Costume(string name, Sprite sprite)
        {
            Sprite = sprite;
            Name = name;
        }
    }
}
