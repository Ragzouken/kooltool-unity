using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool
{
    public class WorldCamera : MonoBehaviour
    {
        [SerializeField] protected Camera Camera;
        [SerializeField] protected RectTransform World;

        public Vector2 Position { get; protected set; }
        public float Scale { get; protected set; }

        public void SetScale(float scale)
        {
            Camera.orthographicSize = Camera.pixelHeight / (2 * scale);

            Debug.Log(scale);

            //transform.position = (Vector3) Position + Vector3.back * scale * 32;
        }
    }
}
