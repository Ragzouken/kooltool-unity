using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class CreateTile : MonoBehaviour
    {
        [SerializeField] private CanvasGroup cgroup;
        [SerializeField] private Button button;

        public System.Action OnClickedCreate = delegate { };

        private void Awake()
        {
            button.onClick.AddListener(() => OnClickedCreate());
        }

        public float alpha
        {
            set
            {
                cgroup.alpha = value;
            }
        }
    }
}
