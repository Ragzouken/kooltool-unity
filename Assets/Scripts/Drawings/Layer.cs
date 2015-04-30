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

        protected Dictionary<Character, CharacterDrawing> CharacterDrawings
            = new Dictionary<Character, CharacterDrawing>();

        public IDrawing DrawingUnderPoint(Point point)
        {
            Point cell, offset;

            Editor.Project.Grid.Coords(point, out cell, out offset);

            Tileset.Tile tile;

            foreach (CharacterDrawing drawing in CharacterDrawings.Values)
            {
                var rtrans = drawing.transform as RectTransform;

                if (rtrans.rect.Contains(point.Vector2()))
                {
                    return drawing;
                }
            }

            if (Tilemap.Get(cell, out tile))
            {
                return Tilemap;
            }
            else
            {
                return Drawing;
            }
        }

        public CharacterDrawing AddCharacter(Character character)
        {
            var drawing = Instantiate<CharacterDrawing>(CharacterPrefab);
            drawing.transform.SetParent(CharacterContainer, false);
            drawing.SetCharacter(character);
            drawing.GetComponent<RectTransform>().anchoredPosition = character.Position.Vector2();

            CharacterDrawings.Add(character, drawing);

            return drawing;
        }
    }
}
