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

        public Project Project { get; protected set; }
        public Character Player_ { get; protected set; }

        public void Setup(Project project)
        {
            Project = project;



            if (Project.Characters.Count > 0) Player_ = Project.Characters[0];
        }

        protected void Update()
        {
            CheckInput();

            if (Player_ != null)
            {
                //var center = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight) * 0.5f;

                Camera.SetScale(2);
                Camera.LookAt(Player_.Position);

                World.localPosition = Vector3.zero;
            }
        }

        protected void CheckInput()
        {
            if (Player_ != null)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))  StartCoroutine(MoveCharacter(Player_, Player_.Position + Point.Left  * 32, .5f));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(MoveCharacter(Player_, Player_.Position + Point.Right * 32, .5f));
            }
        }

        IEnumerator MoveCharacter(Character character, 
                                  Vector2 destination, 
                                  float duration)
        {
            float t = 0;
            Vector2 start = character.Position;
            Vector2 velocity = (destination - start) / duration;

            yield return null;

            while (t < duration)
            {
                t += Time.deltaTime;

                character.SetPosition(start + velocity * t);

                yield return null;
            }

            character.SetPosition(destination);
        }
    }
}
