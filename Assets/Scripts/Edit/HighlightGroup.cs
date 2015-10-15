using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class HighlightGroup : MonoBehaviour
    {
        [SerializeField] protected Image HighlightPrefab;

        public MonoBehaviourPooler<RectTransform, Image> Highlights;

        protected void Awake()
        {
            Highlights = new MonoBehaviourPooler<RectTransform, Image>(HighlightPrefab, 
                                                                       initialize: InitHighlight);
        }

        protected void InitHighlight(RectTransform target, Image highlight)
        {
            highlight.transform.SetParent(target.transform, false);
            highlight.gameObject.SetActive(true);
        }
    }
}
