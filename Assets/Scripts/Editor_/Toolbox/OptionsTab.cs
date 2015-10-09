using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class OptionsTab : MonoBehaviour
    {
        [SerializeField] private Toolbox toolbox;

        [SerializeField] private Button saveButton;
        [SerializeField] private Button exportButton;
        [SerializeField] private Button playButton;

        private void Awake()
        {
            saveButton.onClick.AddListener(OnClickedSave);
            exportButton.onClick.AddListener(OnClickedExport);
            playButton.onClick.AddListener(OnClickedPlay);
        }

        public void OnClickedSave()
        {
            toolbox.editor.Save();
        }

        public void OnClickedExport()
        {
            toolbox.editor.Export();
        }

        public void OnClickedPlay()
        {
            toolbox.editor.Play();
        }
    }
}
