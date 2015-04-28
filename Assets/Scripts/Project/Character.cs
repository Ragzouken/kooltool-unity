using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool
{
    public class Character
    {
        public Point Position { get; protected set; }
        public Costume Costume { get; protected set; }

        public Character(Point position,
                         Costume costume)
        {
            Position = position;
            Costume = costume;
        }
    }
}
