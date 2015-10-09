using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool
{
    public class WorldCamera : MonoBehaviour
    {
        [SerializeField] protected Camera Camera;
        [SerializeField] private Transform focusTransform;

        [SerializeField] protected RectTransform World;

        public Vector2 focusTarget;
        private Vector2 focusVelocity;

        public float scaleTarget;
        private float scaleVelocity;

        public Vector2 focus
        {
            set
            {
                focusTransform.position = new Vector3(value.x, value.y, -256);
            }

            get
            {
                Vector3 pos = focusTransform.position;

                return new Vector2(pos.x, pos.y);
            }
        }

        public float scale
        {
            set
            {
                Camera.orthographicSize = Camera.pixelHeight / (2 * value);
            }

            get
            {
                return Camera.pixelHeight / (2 * Camera.orthographicSize);
            }
        }

        private void Awake()
        {
            Halt();
        }

        private void Update()
        {
            focus = Vector2.SmoothDamp(focus, focusTarget, ref focusVelocity, .1f);
            scale = Mathf.SmoothDamp(scale, scaleTarget, ref scaleVelocity, .1f);
        }

        public void Halt()
        {
            focusTarget = focus;
            scaleTarget = scale;

            focusVelocity = Vector2.zero;
            scaleVelocity = 0f;
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

            focus = inverse.GetPoint(distance);
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
