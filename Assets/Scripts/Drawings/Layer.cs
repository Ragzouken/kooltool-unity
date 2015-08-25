using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool.Editor
{
    public class Layer : MonoBehaviour, IDrawable
    {
        IDrawing IDrawable.Drawing
        {
            get
            {
                return Drawing;
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

        public IDrawing DrawingUnderPoint(Point point)
        {
            Point cell, offset;

            Editor.Project.Grid.Coords(point, out cell, out offset);

            kooltool.Serialization.TileInstance tile;
            CharacterDrawing character;

            if (CharacterUnderPoint(point, out character))
            {
                return character;
            }
            else if (Tilemap.Get(cell, out tile))
            {
                return Tilemap;
            }
            else
            {
                return Drawing;
            }
        }

        public bool CharacterUnderPoint(Point point, out CharacterDrawing character)
        {
            Point cell, offset;

            Editor.Project.Grid.Coords(point, out cell, out offset);

            character = null;

            foreach (CharacterDrawing drawing in Characters.Instances)
            {
                var rtrans = drawing.transform as RectTransform;

                if (rtrans.rect.Contains((Vector2) point - rtrans.anchoredPosition))
                {
                    if (character == null
                     || drawing.transform.GetSiblingIndex() > character.transform.GetSiblingIndex())
                    {
                        character = drawing;
                    }
                }
            }

            return character != null;
        }
    }
}
