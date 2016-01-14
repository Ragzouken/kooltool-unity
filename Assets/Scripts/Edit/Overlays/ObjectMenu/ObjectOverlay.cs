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

        [SerializeField] private Button removeButton;
        [SerializeField] private InputField characterNameInput;

        [Header("Costume")]
        [SerializeField] private Text flipbookNameText;
        [SerializeField] private Button rotateCWButton;
        [SerializeField] private Button rotateACWButton;
        [SerializeField] private Toggle animateToggle;
        [SerializeField] private Toggle onionSkinToggle;

        [Header("Frames")]
        [SerializeField] private Scrollbar frameScrollbar;
        [SerializeField] private Button deleteFrameButton;
        [SerializeField] private Button insertFrameAfterButton;
        [SerializeField] private Button insertFrameBeforeButton;

        private CharacterEditable subject;

        private MonoBehaviourPooler<ObjectAction, ObjectActionButton> actions;

        private void Awake()
        {
            actions = new MonoBehaviourPooler<ObjectAction, ObjectActionButton>(actionPrefab,
                                                                                actionContainer,
                                                                                InitialiseAction);

            rotateCWButton.onClick.AddListener(OnRotateCWClicked);
            rotateACWButton.onClick.AddListener(OnRotateACWClicked);
            animateToggle.onValueChanged.AddListener(OnAnimateToggled);
            onionSkinToggle.onValueChanged.AddListener(OnOnionSkinToggled);
            characterNameInput.onEndEdit.AddListener(OnRename);

            removeButton.onClick.AddListener(OnClickedRemove);

            frameScrollbar.onValueChanged.AddListener(OnFrameChanged);
            deleteFrameButton.onClick.AddListener(OnClickedDeleteFrame);
            insertFrameAfterButton.onClick.AddListener(OnClickedInsertFrameAfter);
            insertFrameBeforeButton.onClick.AddListener(OnClickedInsertFrameBefore);
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

                var character = subject.GetComponent<CharacterDrawing>().Character;

                characterNameInput.text = character.name;

                flipbookNameText.text = subject.drawing.flipbook.name + " (" + subject.drawing.flipbook.tag + ")";

                frameScrollbar.size = 1f / drawing.flipbook.frames.Count;
                frameScrollbar.value = drawing.frame / (drawing.flipbook.frames.Count - 1);
                frameScrollbar.numberOfSteps = drawing.flipbook.frames.Count;
            }

            if (subject != null) actions.SetActive(editable.Actions);
        }

        private void OnFrameChanged(float value)
        {
            subject.drawing.SetFrame(Mathf.RoundToInt(value * (drawing.flipbook.frames.Count - 1)));
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

        private void OnAnimateToggled(bool animate)
        {
            subject.GetComponent<CharacterDrawing>().preview = animate;
        }

        private void OnOnionSkinToggled(bool show)
        {
            subject.GetComponent<CharacterDrawing>().onionSkin = show;
        }

        private void OnClickedRemove()
        {
            Editor.Instance.RemoveCharacter(subject.GetComponent<CharacterDrawing>().Character);
        }

        private void OnRename(string name)
        {
            name = name.Replace("\n", "");
            name = name.Trim();
            name = name.Substring(0, Mathf.Min(name.Length - 1, 16));

            subject.GetComponent<CharacterDrawing>().Character.name = name;
            characterNameInput.text = name;
        }

        private CharacterDrawing drawing
        {
            get
            {
                return subject.GetComponent<CharacterDrawing>();
            }
        }

        private void CopyFrame(int source, int dest)
        {
            Data.Frame original = drawing.flipbook.frames[source];
            Data.Frame copy     = original.BadClone(Editor.Instance.project_.index.CreateTexture(original.texture.texture.width,
                                                                                                 original.texture.texture.height));
            
            if (dest == drawing.flipbook.frames.Count)
            {
                drawing.flipbook.frames.Add(copy);
            }            
            else
            {
                drawing.flipbook.frames.Insert(dest, copy);
            }
        }

        private void OnClickedDeleteFrame()
        {
            drawing.flipbook.frames.RemoveAt(drawing.frame);
            SetSubject(subject);
        }

        private void OnClickedInsertFrameAfter()
        {
            CopyFrame(drawing.frame, drawing.frame + 1);
            SetSubject(subject);
        }

        private void OnClickedInsertFrameBefore()
        {
            CopyFrame(drawing.frame, drawing.frame);
            SetSubject(subject);
        }
    }
}
