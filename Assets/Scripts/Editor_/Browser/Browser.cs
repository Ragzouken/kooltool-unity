using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using kooltool.Serialization;

namespace kooltool.Editor
{
    public class Browser : MonoBehaviour
    {
        public event System.Action<Summary> OnConfirmed = delegate { };

        private MonoBehaviourPooler<Summary, BrowserIcon> summaries;

        [Header("Listing")]
        [SerializeField] private RectTransform iconContainer;
        [SerializeField] private BrowserIcon iconPrefab;

        [Header("Details")]
        [SerializeField] private InputField titleInput;
        [SerializeField] private InputField descriptionInput;
        [SerializeField] private Image test;

        private Summary summary;

        private void Awake()
        {
            summaries = new MonoBehaviourPooler<Summary, BrowserIcon>(iconPrefab,
                                                                      iconContainer,
                                                                      InitialiseIcon);

            titleInput.onEndEdit.AddListener((_) => SaveSummary());
            descriptionInput.onEndEdit.AddListener((_) => SaveSummary());
        }

        private void InitialiseIcon(Summary summary, BrowserIcon icon)
        {
            icon.summary = summary;
            icon.image.sprite = summary.iconSprite;
            icon.single = s => SetSummary(s);
            icon.@double = s => OnConfirmed(s);
        }

        private void SetSummary(Summary summary)
        {
            this.summary = summary;
            titleInput.text = summary.title;
            descriptionInput.text = summary.description;
            test.sprite = summary.iconSprite;
        }

        public void Refresh()
        {
            summaries.SetActive(GetSummaries());
        }

        private void SaveSummary()
        {
            summary.title = titleInput.text;
            summary.description = descriptionInput.text;
           
            ProjectTools.SaveSummary(summary);
        }

        private IEnumerable<Summary> GetSummaries()
        {
            var directory = new DirectoryInfo(Application.persistentDataPath);

            foreach (var project in directory.GetDirectories())
            {
                yield return ProjectTools.LoadSummary(project.Name);
            }
        }
    }
}
