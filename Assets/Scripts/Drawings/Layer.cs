using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using PixelDraw;

namespace kooltool.Editor
{
    public class Layer : MonoBehaviour
    {
        [SerializeField] protected Editor Editor;
        [SerializeField] protected CharacterDrawing CharacterPrefab;

        [Header("Objects")]
        public InfiniteDrawing Drawing;
        public Tilemap Tilemap;
        public RectTransform CharacterContainer;

        public MonoBehaviourPooler<Character, CharacterDrawing> Characters;

        protected void Awake()
        {
            Characters = new MonoBehaviourPooler<Character, CharacterDrawing>(CharacterPrefab, CharacterContainer, InitCharacter);
        }

        protected void InitCharacter(Character character, CharacterDrawing drawing)
        {
            drawing.gameObject.layer = LayerMask.NameToLayer("World");
            drawing.SetCharacter(character);
            drawing.GetComponent<RectTransform>().anchoredPosition = character.Position.Vector2();
        }

        public IDrawing DrawingUnderPoint(Point point)
        {
            Point cell, offset;

            Editor.Project.Grid.Coords(point, out cell, out offset);

            Tileset.Tile tile;
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

                if (rtrans.rect.Contains(point.Vector2() - rtrans.anchoredPosition))
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
