using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool
{
    public class Character
    {
        public event System.Action<Vector2> PositionUpdated;

        public Vector2 Position { get; protected set; }
        public Costume Costume { get; protected set; }

        public string dialogue = "";

        public Character(Point position,
                         Costume costume)
        {
            Position = position;
            Costume = costume;
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;

            PositionUpdated(position);
        }
    }
}
