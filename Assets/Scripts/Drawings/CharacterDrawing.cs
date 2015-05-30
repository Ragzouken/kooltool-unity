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
        [SerializeField] protected Image image;
        [SerializeField] protected InputField dialogueInput;

        public Character Character { get; protected set; }

        protected void Awake()
        {
            dialogueInput.onEndEdit.AddListener(UpdateDialogue);
        }

        public void SetCharacter(Character character)
        {
            if (character != Character && Character != null)
            {
                Character.PositionUpdated -= UpdatePosition;
            }

            Character = character;
            Character.PositionUpdated += UpdatePosition;

            Drawing = new SpriteDrawing(character.Costume.Sprite);

            image.sprite = character.Costume.Sprite;
            image.SetNativeSize();
        }

        private void UpdatePosition(Point position)
        {
            transform.localPosition = position;
        }

        private void UpdateDialogue(string text)
        {
            Character.dialogue = text;
        }
    }
}
