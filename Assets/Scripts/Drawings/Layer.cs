using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool.Editor
{
    public class Layer : Editable, IDrawable, ITileable
    {
        IDrawing IDrawable.Drawing
        {
            get
            {
                return Drawing;
            }
        }
        
        Tilemap ITileable.Tilemap
        {
            get
            {
                return Tilemap;
            }
        }

        [SerializeField] protected Editor Editor;
        [SerializeField] protected CharacterDrawing CharacterPrefab;

        [Header("Objects")]
        public InfiniteDrawing Drawing;
        public Tilemap Tilemap;
        public RectTransform CharacterContainer;

        public MonoBehaviourPooler<Character, CharacterDrawing> Characters;

        private kooltool.Serialization.Layer layer;

        protected void Awake()
        {
            Characters = new MonoBehaviourPooler<Character, CharacterDrawing>(CharacterPrefab, 
                                                                              CharacterContainer, 
                                                                              InitialiseCharacter);
        }

        private void InitialiseCharacter(Character character, CharacterDrawing drawing)
        {
            drawing.gameObject.layer = LayerMask.NameToLayer("World");
            drawing.SetCharacter(character, Editor);
            drawing.GetComponent<RectTransform>().anchoredPosition = character.position;
        }

        public void SetLayer(Serialization.Layer layer)
        {
            this.layer = layer;

            Tilemap.SetLayer(layer);
            Characters.SetActive(layer.characters);
            Drawing.SetLayer(layer);
        }
    }
}
