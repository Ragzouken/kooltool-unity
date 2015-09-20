using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class ObjectOverlay : MonoBehaviour 
    {
        [SerializeField] private Button deleteButton;
        [SerializeField] private UIRectFollowRect follow;

        private IObject subject;

        private void Awake()
        {
            deleteButton.onClick.AddListener(OnClickedDelete);
        }

        public void SetSubject(IObject subject)
        {
            this.subject = subject;

            gameObject.SetActive(subject != null);

            follow.target = subject != null ? subject.OverlayParent : null;
        }

        private void OnClickedDelete()
        {
            subject.Remove();
        }
    }
}
