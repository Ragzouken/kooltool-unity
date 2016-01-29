using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace kooltool
{
    public class MainMenu : MonoBehaviour
    {
        [SceneOnly][SerializeField] private Main main;
        
        [Header("Buttons")]
        [SerializeField] private Button blankButton;
        [SerializeField] private Button generateButton;
        [SerializeField] private Button quitButton;

        [Header("Projects")]
        [SerializeField] private RectTransform projectContainer;
        [SerializeField] private ProjectPanel projectPrefab;

        private MonoBehaviourPooler<Data.Summary, ProjectPanel> projects;

        private void Awake()
        {
            blankButton.onClick.AddListener(main.Blank);
            generateButton.onClick.AddListener(main.Generate);
            quitButton.onClick.AddListener(main.Exit);

            projects = new MonoBehaviourPooler<Data.Summary, ProjectPanel>(projectPrefab,
                                                                           projectContainer,
                                                                           InitialiseProject);

            Refresh();
        }

        private void InitialiseProject(Data.Summary summary, 
                                       ProjectPanel panel)
        {
            panel.SetSummary(summary);
        }

        public void Refresh()
        {
            projects.SetActive(Data.ProjectTools.GetSummaries());
        }
    }
}
