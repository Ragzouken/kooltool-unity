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
        [SerializeField] protected CanvasGroup dialogueGroup;

        public Editor editor;

        public Character Character { get; protected set; }

        protected void Awake()
        {
            dialogueInput.onEndEdit.AddListener(UpdateDialogue);
        }

        public void SetCharacter(Character character, Editor editor)
        {
            this.editor = editor;

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
            dialogueInput.gameObject.SetActive(true);
            dialogueInput.text = Character.dialogue;
            dialogueGroup.interactable = true;
            dialogueGroup.alpha = 1;
        }

        public void SetPlayer()
        {
            dialogueInput.gameObject.SetActive(false);
            dialogueGroup.interactable = false;
        }

        public void ShowDialogue(string text)
        {
            dialogueInput.text = text;
            dialogueInput.gameObject.SetActive(true);
            dialogueGroup.alpha = 0;

            StartCoroutine(ShowDialogue(1f));
        }

        private IEnumerator ShowDialogue(float duration)
        {
            float t = duration;

            while (t > 0)
            {
                float u = t / duration;

                dialogueGroup.alpha = Mathf.Min(1, Mathf.Sin(u * Mathf.PI) * 2);

                yield return null;

                t -= Time.deltaTime;
            }

            dialogueInput.gameObject.SetActive(false);
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
