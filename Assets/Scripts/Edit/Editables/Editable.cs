using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    [DisallowMultipleComponent]
    public class Editable : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform cursorParent;

        public Transform CursorParent { get { return cursorParent; } }

        public float targetAlpha = 1f;

        private float alphaVelocity;

        protected virtual void Update()
        {
            canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, targetAlpha, ref alphaVelocity, .25f);
        }
    }
}
