using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class CharacterDetailsPanel : MonoBehaviour
    {
        [SerializeField] private Editor editor;
        [SerializeField] private InputField input;

        public Character character { get; private set; }

        private void Awake()
        {
            input.onEndEdit.AddListener(OnEndEdit);
        }

        public void SetCharacter(Character character)
        {
            this.character = character;

            input.text = character.dialogue;
        }

        private void OnEndEdit(string text)
        {
            character.dialogue = text;
        }
    }
}
