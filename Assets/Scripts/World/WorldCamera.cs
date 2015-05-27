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
        }

        public void LookAt(Vector3 world, Vector2? screen=null)
        {
            var center = new Vector2(Camera.pixelWidth, 
                                     Camera.pixelHeight) * 0.5f;

            screen = screen ?? center;

            var panning = new Plane(Vector3.forward, Camera.transform.position);

            Ray ray = Camera.ScreenPointToRay((Vector3) screen);
            Ray inverse = new Ray(world, -ray.direction);

            float distance;

            panning.Raycast(inverse, out distance);

            Camera.transform.position = inverse.GetPoint(distance);
        }

        public void Pan(Vector3 worldDelta)
        {
            // TODO: fix this
            Camera.transform.position += worldDelta;
        }

        public Vector2 ScreenToWorld(Vector2 screen)
        {
            Vector2 world;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(World,
                                                                    screen,
                                                                    Camera,
                                                                    out world);

            return world;
        }
    }
}
