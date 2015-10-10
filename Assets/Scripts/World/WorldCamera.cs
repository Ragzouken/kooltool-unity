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
        [SerializeField] private Transform scaleTransform;
        [SerializeField] private Transform pivotTransform;

        [SerializeField] protected RectTransform World;

        public Vector2 focusTarget;
        private Vector2 focusVelocity;

        public float scaleTarget;
        private float scaleVelocity;

        public float rotationTarget;
        private float rotationVelocity;

        public float pivotTarget;
        private float pivotVelocity;

        public Vector2 up
        {
            get
            {
                return focusTransform.up;
            }
        }

        public Vector2 right
        {
            get
            {
                return focusTransform.right;
            }
        }

        public Vector2 focus
        {
            set
            {
                focusTransform.localPosition = new Vector3(value.x, value.y, 0);
            }

            get
            {
                Vector3 pos = focusTransform.localPosition;

                return new Vector2(pos.x, pos.y);
            }
        }

        private float _scale;

        public float scale
        {
            set
            {
                Camera.orthographicSize = Camera.pixelHeight / (2 * value);

                float h = Camera.pixelHeight * 0.5f;
                float s = value;
                float t = Mathf.Tan(Mathf.Deg2Rad * Camera.fieldOfView * 0.5f);

                scaleTransform.localPosition = Vector3.back * h / (s * t);
            }

            get
            {
                return Camera.pixelHeight / (2 * Camera.orthographicSize);
            }
        }

        public float rotation
        {
            set
            {
                focusTransform.localEulerAngles = Vector3.forward * value;
            }

            get
            {
                return focusTransform.localEulerAngles.z;
            }
        }

        public float pivot
        {
            set
            {
                pivotTransform.localEulerAngles = Vector3.left * value;
            }

            get
            {
                return (360 - pivotTransform.localEulerAngles.x) % 360;
            }
        }

        public Matrix4x4 orthographic
        {
            get
            {
                float size = Camera.pixelHeight / (2 * scale);

                float aspect = Camera.aspect;

                return Matrix4x4.Ortho(-size * aspect,
                                        size * aspect,
                                       -size,
                                        size,
                                       Camera.nearClipPlane,
                                       Camera.farClipPlane);
            }
        }

        public Matrix4x4 perspective
        {
            get
            {
                return Matrix4x4.Perspective(Camera.fieldOfView,
                                             Camera.aspect,
                                             Camera.nearClipPlane,
                                             Camera.farClipPlane);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;

            float h = Camera.pixelHeight * 0.5f;
            float t = Mathf.Tan(Mathf.Deg2Rad * Camera.fieldOfView * 0.5f);

            Gizmos.DrawLine(transform.position, transform.position + Vector3.back * h / (scale * t));
        }

        private void Awake()
        {
            Halt();
        }

        private Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float t)
        {
            t = Mathf.Clamp(t, 0.0f, 1.0f);
            var newMatrix = new Matrix4x4();
            newMatrix.SetRow(0, Vector4.Lerp(from.GetRow(0), to.GetRow(0), t));
            newMatrix.SetRow(1, Vector4.Lerp(from.GetRow(1), to.GetRow(1), t));
            newMatrix.SetRow(2, Vector4.Lerp(from.GetRow(2), to.GetRow(2), t));
            newMatrix.SetRow(3, Vector4.Lerp(from.GetRow(3), to.GetRow(3), t));
            return newMatrix;
        }

        private void Update()
        {
            focus = Vector2.SmoothDamp(focus, focusTarget, ref focusVelocity, .1f);
            scale = Mathf.SmoothDamp(scale, scaleTarget, ref scaleVelocity, .1f);
            rotation = Mathf.SmoothDampAngle(rotation, rotationTarget, ref rotationVelocity, .1f);
            pivot = Mathf.SmoothDampAngle(pivot, pivotTarget, ref pivotVelocity, .1f);
        }

        public void Halt()
        {
            focusTarget = focus;
            scaleTarget = scale;
            rotationTarget = rotation;
            pivotTarget = pivot;

            focusVelocity = Vector2.zero;
            scaleVelocity = 0f;
            rotationVelocity = 0f;
            pivotVelocity = 0f;
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
