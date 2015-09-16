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

        private IObject subject;

        private void Awake()
        {
            deleteButton.onClick.AddListener(OnClickedDelete);
        }

        public void SetSubject(IObject subject)
        {
            this.subject = subject;

            gameObject.SetActive(subject != null);
            transform.SetParent(subject != null ? subject.OverlayParent : null, false);
        }

        private void OnClickedDelete()
        {
            subject.Remove();
        }
    }
}
