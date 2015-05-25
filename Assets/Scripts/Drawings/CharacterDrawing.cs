using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;

using PixelDraw;

namespace kooltool.Editor
{
    public class CharacterDrawing : MonoDrawing
    {
        [SerializeField] protected Image Image;

        public Character Character { get; protected set; }

        public void SetCharacter(Character character)
        {
            if (character != Character && Character != null)
            {
                Character.PositionUpdated -= UpdatePosition;
            }

            Character = character;
            Character.PositionUpdated += UpdatePosition;

            Drawing = new SpriteDrawing(character.Costume.Sprite);

            Image.sprite = character.Costume.Sprite;
            Image.SetNativeSize();
        }

        private void UpdatePosition(Point position)
        {
            transform.localPosition = position;
        }
    }
}
