using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

using kooltool.Data;

namespace kooltool.Editor
{
    public class Layer : Editable, IDrawable, ITileable, IAnnotatable
    {
        public class Hack : IDrawable
        {
            public Layer layer;

            IDrawing IDrawable.Drawing
            {
                get
                {
                    return layer.Annotations;
                }
            }
        }

        Hack IAnnotatable.Hack
        {
            get
            {
                return hack;
            }
        }

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

        [Header("Noteboxes")]
        [SerializeField] private NoteboxView noteboxPrefab;
        [SerializeField] private RectTransform noteboxContainer;

        [Header("Objects")]
        public InfiniteDrawing Drawing;
        public Tilemap Tilemap;
        public InfiniteDrawing Annotations;
        public RectTransform CharacterContainer;

        public MonoBehaviourPooler<Character, CharacterDrawing> Characters;
        public MonoBehaviourPooler<Notebox, NoteboxView> noteboxes;

        private kooltool.Data.Layer layer;
        private Hack hack;

        protected void Awake()
        {
            Characters = new MonoBehaviourPooler<Character, CharacterDrawing>(CharacterPrefab, 
                                                                              CharacterContainer, 
                                                                              InitialiseCharacter);

            noteboxes = new MonoBehaviourPooler<Notebox, NoteboxView>(noteboxPrefab,
                                                                      noteboxContainer,
                                                                      InitialiseNotebox);

            hack = new Hack { layer = this };
        }

        private void InitialiseCharacter(Character character, CharacterDrawing drawing)
        {
            drawing.gameObject.layer = LayerMask.NameToLayer("World");
            drawing.SetCharacter(character, Editor);
            drawing.GetComponent<RectTransform>().anchoredPosition = character.position;
        }

        private void InitialiseNotebox(Notebox notebox, NoteboxView view)
        {
            view.editor = Editor;
            view.SetNotebox(notebox);
            view.GetComponent<RectTransform>().anchoredPosition = notebox.position;
        }

        public void SetLayer(Data.Layer layer)
        {
            this.layer = layer;

            Tilemap.SetLayer(layer);
            Characters.SetActive(layer.characters);
            Drawing.SetLayer(layer, layer.drawing);
            Annotations.SetLayer(layer, layer.annotations);
        }

        void ITileable.Demote(Point cell)
        {
            Data.TileInstance instance;

            bool exists = Tilemap.Get(cell, out instance);

            Assert.IsTrue(exists, string.Format("No tile to demote at {0}!", cell));

            Tilemap.Unset(cell);

            Sprite brush = instance.tile.sprites[0];

            Drawing.Brush(cell * 32, brush, Blend.Alpha);
            Drawing.Apply();
        }
    }
}
