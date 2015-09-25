using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using kooltool.Data;
using Newtonsoft.Json;

namespace kooltool
{
    public class Character : IResource
    {
        public event System.Action<Vector2> PositionUpdated;

        [JsonIgnore]
        public Vector2 position;

        public Point _position;
        public Costume costume;

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

        void IResource.Load(Index index)
        {
            position = _position;
        }

        void IResource.Save(Index index)
        {
            _position = position;
        }
    }
}
