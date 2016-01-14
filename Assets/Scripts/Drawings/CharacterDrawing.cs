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

        [Header("Onion Skinning")]
        [SerializeField] private Image prevFrame;
        [SerializeField] private Image nextFrame;

        public Character Character { get; protected set; }

        public int frame = 0;
        public Data.Flipbook flipbook;
        public bool preview;

        private float previewTimer;

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

            onionSkin = false;
        }

        public void SetFlipbook(Data.Flipbook flipbook)
        {
            this.flipbook = flipbook;

            SetFrame(0);
        }

        public void SetFrame(int frame)
        {
            this.frame = frame % flipbook.frames.Count;
            sprite = flipbook.frames[this.frame];

            int prev = (frame + flipbook.frames.Count - 1) % flipbook.frames.Count;
            int next = (frame + flipbook.frames.Count + 1) % flipbook.frames.Count;

            prevFrame.sprite = flipbook.frames[prev];
            nextFrame.sprite = flipbook.frames[next];

            image.SetNativeSize();
            prevFrame.SetNativeSize();
            nextFrame.SetNativeSize();
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

        public bool onionSkin
        {
            set
            {
                prevFrame.gameObject.SetActive(value);
                nextFrame.gameObject.SetActive(value);
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
            if (preview)
            {
                previewTimer += Time.deltaTime;

                float freq = flipbook.period / flipbook.frames.Count;

                while (previewTimer > freq)
                {
                    SetFrame(frame + 1);
                    previewTimer -= freq;
                }
            }
            else
            {
                previewTimer = 0;
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

        public override bool Sample(Point pixel, out Color color)
        {
            pixel -= Character.position;
            pixel -= image.sprite.pivot;

            return Drawing.Sample(pixel, out color);
        }
    }
}
