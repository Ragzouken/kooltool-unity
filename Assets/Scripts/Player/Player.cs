using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] protected WorldCamera Camera;
        [SerializeField] protected RectTransform World;
        [SerializeField] protected kooltool.Editor.Editor editor;

        [Header("Speech Test")]
        [SerializeField] protected RectTransform speechContainer;
        [SerializeField] protected SpeechTest speechPrefab;

        public Project Project { get; protected set; }
        public Character Player_ { get; protected set; }

        protected Coroutine movementCO;
        protected Dictionary<Point, Character> collision
            = new Dictionary<Point, Character>();

        public void Setup(Project project)
        {
            Project = project;

            if (Project.Characters.Count > 0) Player_ = Project.Characters[0];

            collision.Clear();

            foreach (var character in project.Characters)
            {
                Point start, dummy;
                project.Grid.Coords(character.Position, out start, out dummy);

                collision[start] = character;
            }
        }

        protected void Update()
        {
            CheckInput();

            if (Player_ != null)
            {
                Camera.SetScale(2);
                Camera.LookAt(Player_.Position);
            }
        }

        protected void CheckInput()
        {
            if (Player_ != null)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))  MoveCharacter(Player_, Player_.Position + Vector2.left  * 32, .5f);
                if (Input.GetKeyDown(KeyCode.RightArrow)) MoveCharacter(Player_, Player_.Position + Vector2.right * 32, .5f);
                if (Input.GetKeyDown(KeyCode.UpArrow))    MoveCharacter(Player_, Player_.Position + Vector2.up    * 32, .5f);
                if (Input.GetKeyDown(KeyCode.DownArrow))  MoveCharacter(Player_, Player_.Position + Vector2.down  * 32, .5f);
            }
        }

        public void Say(Character character, string text)
        {
            var speech = Instantiate<SpeechTest>(speechPrefab);
            speech.transform.SetParent(speechContainer);
            speech.Setup(text, 1f);

            var drawing = editor.Layer.Characters.Get(character);

            float angle = Random.value * Mathf.PI * 2;

            Vector2 offset = new Vector2(Mathf.Cos(angle),
                                         Mathf.Sin(angle));

            speech.transform.position = drawing.transform.position + (Vector3) offset * 32f;
        }

        void MoveCharacter(Character character, 
                           Vector2 destination, 
                           float duration)
        {
            if (movementCO == null)
            {
                movementCO = StartCoroutine(MoveCharacterCO(character, destination, duration));
            }
        }

        IEnumerator MoveCharacterCO(Character character, 
                                    Vector2 destination, 
                                    float duration)
        {
            Point cstart = Project.Grid.WorldToCell(character.Position);
            Point cend   = Project.Grid.WorldToCell(destination);

            if (collision.ContainsKey(cend))
            {
                var other = collision[cend];

                Say(other, other.dialogue);

                yield break;
            }

            if (collision.ContainsKey(cstart) && collision[cstart] == character) collision.Remove(cstart);

            collision[cend] = character;

            float t = 0;
            Vector2 start = character.Position;
            Vector2 velocity = (destination - start) / duration;

            yield return null;

            while (t < duration)
            {
                t = Mathf.Min(duration, t + Time.deltaTime);

                character.SetPosition(start + velocity * t);

                yield return null;
            }

            character.SetPosition(destination);

            movementCO = null;
        }
    }
}
