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
        [SerializeField] private PixelBorder.BorderRenderer border;

        public Character Character { get; protected set; }

        public int frame = 0;
        public Data.Flipbook flipbook;

        public void SetCharacter(Character character)
        {
            if (character != Character && Character != null)
            {
                Character.PositionUpdated -= UpdatePosition;
            }

            Character = character;
            Character.PositionUpdated += UpdatePosition;

            character.costume.TestInit();

            SetFlipbook(character.costume.GetFlipbook("idle"));

            //dialogueInput.text = character.dialogue;
        }

        public void SetFlipbook(Data.Flipbook flipbook)
        {
            this.flipbook = flipbook;

            Drawing = new SpriteDrawing(flipbook.frames[0]);

            sprite = flipbook.frames[0];
            image.SetNativeSize();
        }

        public void SetFrame(int frame)
        {
            this.frame = frame % flipbook.frames.Count;
            sprite = flipbook.frames[frame];
        }

        private Sprite sprite
        {
            set
            {
                image.sprite = value;
                Drawing = new SpriteDrawing(value);
                border.sourceSprite = value;
            }
        }

        public void SetEditor()
        {
        }

        public void SetPlayer()
        {
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                SetFrame((frame + 1) % flipbook.frames.Count);
            }
        }

        private void UpdatePosition(Vector2 position)
        {
            transform.localPosition = position;
        }

        private void UpdateDialogue(string text)
        {
            Character.dialogue = text;
        }

        private void OnDestroy()
        {
            if (Character != null)
            {
                Character.PositionUpdated -= UpdatePosition;
            }
        }
    }
}
