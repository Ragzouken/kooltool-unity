﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
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

        void ITileable.Demote(Point cell)
        {
            Serialization.TileInstance instance;

            bool exists = Tilemap.Get(cell, out instance);

            Assert.IsTrue(exists, string.Format("No tile to demote at {0}!", cell));

            Tilemap.Unset(cell);

            Sprite brush = instance.tile.sprites[0];

            Drawing.Brush(cell * 32, brush, Blend.Alpha);
            Drawing.Apply();
        }
    }
}
