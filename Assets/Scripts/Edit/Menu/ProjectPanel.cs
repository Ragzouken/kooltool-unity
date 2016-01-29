using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace kooltool
{
    public class ProjectPanel : MonoBehaviour
    {
        [SceneOnly][SerializeField] private Main main;
        [SceneOnly][SerializeField] private MainMenu menu;
        [SerializeField] private Image iconImage;

        [SerializeField] private InputField titleText;
        [SerializeField] private InputField blurbText;

        [SerializeField] private Button openButton;
        [SerializeField] private Button deleteButton;

        private Data.Summary summary;

        private void Awake()
        {
            titleText.onEndEdit.AddListener(OnTextChange);
            blurbText.onEndEdit.AddListener(OnTextChange);

            openButton.onClick.AddListener(OnClickedOpen);
            deleteButton.onClick.AddListener(OnClickedDelete);
        }

        public void SetSummary(Data.Summary summary)
        {
            this.summary = summary;

            iconImage.sprite = summary.iconSprite;
            titleText.text = summary.title;
            blurbText.text = summary.description;
        }

        private void OnTextChange(string value)
        {
            bool change = summary.title != titleText.text
                       || summary.description != blurbText.text;

            if (change)
            {
                summary.title = titleText.text;
                summary.description = blurbText.text;

                Data.ProjectTools.SaveSummary(summary);
            }
        }

        private void OnClickedOpen()
        {
            main.Load(summary.folder);
        }

        private void OnClickedDelete()
        {
            Data.ProjectTools.DeleteProject(summary);

            menu.Refresh();
        }
    }
}
