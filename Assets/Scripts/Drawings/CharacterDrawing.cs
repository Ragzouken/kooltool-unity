﻿using UnityEngine;
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
        [SerializeField] protected CanvasGroup dialogueGroup;

        public Character Character { get; protected set; }

        public void SetCharacter(Character character)
        {
            if (character != Character && Character != null)
            {
                Character.PositionUpdated -= UpdatePosition;
            }

            Character = character;
            Character.PositionUpdated += UpdatePosition;

            character.costume.TestInit();
            Drawing = new SpriteDrawing(character.costume.sprite);
            //dialogueInput.text = character.dialogue;

            image.sprite = character.costume.sprite;
            image.SetNativeSize();
        }

        public void SetEditor()
        {
            dialogueGroup.interactable = true;
            dialogueGroup.alpha = 1;
        }

        public void SetPlayer()
        {
            dialogueGroup.interactable = false;
        }

        private void UpdatePosition(Vector2 position)
        {
            transform.localPosition = position;
        }

        private void UpdateDialogue(string text)
        {
            Character.dialogue = text;
        }
    }
}
