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

        [Header("Costume")]
        [SerializeField] private Text flipbookNameText;
        [SerializeField] private Slider frameSlider;
        [SerializeField] private Button rotateCWButton;
        [SerializeField] private Button rotateACWButton;

        private CharacterEditable subject;

        private MonoBehaviourPooler<ObjectAction, ObjectActionButton> actions;

        private void Awake()
        {
            actions = new MonoBehaviourPooler<ObjectAction, ObjectActionButton>(actionPrefab,
                                                                                actionContainer,
                                                                                InitialiseAction);

            frameSlider.onValueChanged.AddListener(OnFrameChanged);
            rotateCWButton.onClick.AddListener(OnRotateCWClicked);
            rotateACWButton.onClick.AddListener(OnRotateACWClicked);
        }

        private void InitialiseAction(ObjectAction action, 
                                      ObjectActionButton button)
        {
            button.Setup(action);
        }

        public void SetSubject(CharacterEditable subject)
        {
            this.subject = subject;

            var editable = subject as IObject;

            gameObject.SetActive(subject != null);

            follow.target = subject != null ? editable.OverlayParent : null;

            if (subject != null)
            {
                frameSlider.value = subject.drawing.frame;
                frameSlider.minValue = 0;
                frameSlider.maxValue = subject.drawing.flipbook.frames.Count - 1;
                flipbookNameText.text = subject.drawing.flipbook.name + " (" + subject.drawing.flipbook.tag + ")";
            }

            if (subject != null) actions.SetActive(editable.Actions);
        }

        private void OnFrameChanged(float value)
        {
            subject.drawing.SetFrame(Mathf.FloorToInt(value));
        }

        private static List<string> dirs = new List<string> { "n", "e", "s", "w" };

        private void OnRotateCWClicked()
        {
            var d = subject.drawing;
            int i = (dirs.IndexOf(d.flipbook.tag) + 1) % 4;
            d.SetFlipbook(d.Character.costume.GetFlipbook(d.flipbook.name, dirs[i]));
            SetSubject(subject);
        }

        private void OnRotateACWClicked()
        {
            var d = subject.drawing;
            int i = (dirs.IndexOf(d.flipbook.tag) + 3) % 4;
            d.SetFlipbook(d.Character.costume.GetFlipbook(d.flipbook.name, dirs[i]));
            SetSubject(subject);
        }
    }
}
