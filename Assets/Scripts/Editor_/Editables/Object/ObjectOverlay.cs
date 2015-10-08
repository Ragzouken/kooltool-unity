using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class ObjectOverlay : MonoBehaviour 
    {
        [SerializeField] private UIRectFollowRect follow;

        [Header("Actions")]
        [SerializeField] private RectTransform actionContainer;
        [SerializeField] private ObjectActionButton actionPrefab;

        private IObject subject;

        private MonoBehaviourPooler<ObjectAction, ObjectActionButton> actions;

        private void Awake()
        {
            actions = new MonoBehaviourPooler<ObjectAction, ObjectActionButton>(actionPrefab,
                                                                                actionContainer,
                                                                                InitialiseAction);
        }

        private void InitialiseAction(ObjectAction action, 
                                      ObjectActionButton button)
        {
            button.Setup(IconSettings.Instance[action.icon], action.action);
        }

        public void SetSubject(IObject subject)
        {
            this.subject = subject;

            gameObject.SetActive(subject != null);

            follow.target = subject != null ? subject.OverlayParent : null;

            if (subject != null) actions.SetActive(subject.Actions);
        }
    }
}
