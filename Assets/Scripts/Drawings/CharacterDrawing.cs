using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool.Editor
{
    public class CharacterDrawing : MonoDrawing
    {
        [SerializeField] protected Image Image;

        public Character Character { get; protected set; }

        public void SetCharacter(Character character)
        {
            Character = character;

            Drawing = new SpriteDrawing(character.Costume.Sprite);

            Image.sprite = character.Costume.Sprite;
        }
    }
}
