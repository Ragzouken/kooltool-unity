using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using kooltool.Data;
using Newtonsoft.Json;

namespace kooltool
{
    public class Character
    {
        public event System.Action<Vector2> PositionUpdated;

        public Vector2 position;
        public Costume costume;

        public string name = "unnamed character";
        public string dialogue = "";

        public Character() { }

        public Character(Point position,
                         Costume costume)
        {
            this.position = position;
            this.costume = costume;
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;

            PositionUpdated(position);
        }
    }
}
