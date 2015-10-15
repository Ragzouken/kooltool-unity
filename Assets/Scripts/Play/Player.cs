using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

namespace kooltool.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Main main;
        [SerializeField] private WorldCamera Camera;
        [SerializeField] private WorldView world;

        [Header("Speech Test")]
        [SerializeField] protected RectTransform speechContainer;
        [SerializeField] protected SpeechTest speechPrefab;

        public Data.Project Project { get; protected set; }
        public Character Player_ { get; protected set; }

        protected Coroutine movementCO;
        protected Dictionary<Point, Character> collision
            = new Dictionary<Point, Character>();

        private HashSet<Character> speaking
            = new HashSet<Character>();

        public void Setup(Data.Project project)
        {
            Project = project;

            if (project.world.layers[0].characters.Count > 0) Player_ = project.world.layers[0].characters.First();

            var grid = new SparseGrid<int>(32);

            collision.Clear();

            foreach (var character in project.world.layers[0].characters)
            {
                Point start, dummy;
                grid.Coords(character.position, out start, out dummy);

                collision[start] = character;
            }
        }

        protected void Update()
        {
            CheckInput();

            if (Player_ != null)
            {
                Camera.scale = 2;
                Camera.focus = Player_.position;
            }
        }

        protected void CheckInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) main.SetEditor(Project);

            if (Player_ != null)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))  MoveCharacter(Player_, Player_.position + Vector2.left  * 32, .5f);
                if (Input.GetKeyDown(KeyCode.RightArrow)) MoveCharacter(Player_, Player_.position + Vector2.right * 32, .5f);
                if (Input.GetKeyDown(KeyCode.UpArrow))    MoveCharacter(Player_, Player_.position + Vector2.up    * 32, .5f);
                if (Input.GetKeyDown(KeyCode.DownArrow))  MoveCharacter(Player_, Player_.position + Vector2.down  * 32, .5f);
            }
        }

        public void Say(Character character, string text)
        {
            if (speaking.Contains(character)) return;

            var speech = Instantiate<SpeechTest>(speechPrefab);
            speech.transform.SetParent(speechContainer);
            speech.Setup(text, 1f);

            var drawing = world.layers.Get(Project.world.layers[0]).Characters.Get(character);

            float angle = Random.value * Mathf.PI * 2;

            Vector2 offset = new Vector2(Mathf.Cos(angle),
                                         Mathf.Sin(angle));

            speech.transform.position = drawing.transform.position + (Vector3) offset * 32f;

            speaking.Add(character);
            StartCoroutine(Delay(0.5f, () => speaking.Remove(character)));
        }

        private IEnumerator Delay(float delay, System.Action callback)
        {
            yield return new WaitForSeconds(delay);

            callback();
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
            var grid = new SparseGrid<int>(32);

            Point cstart = grid.WorldToCell(character.position);
            Point cend   = grid.WorldToCell(destination);

            if (collision.ContainsKey(cend))
            {
                var other = collision[cend];

                Say(other, other.dialogue);

                yield break;
            }

            if (collision.ContainsKey(cstart) && collision[cstart] == character) collision.Remove(cstart);

            collision[cend] = character;

            float t = 0;
            Vector2 start = character.position;
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
