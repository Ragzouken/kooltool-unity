using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class SummaryPanel : MonoBehaviour
    {
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button openButton;

        private void Awake()
        {
            cancelButton.onClick.AddListener(OnClickedCancel);
            cancelButton.onClick.AddListener(OnClickedConfirm);
            cancelButton.onClick.AddListener(OnClickedOpen);
        }

        private void OnClickedCancel()
        {

        }

        private void OnClickedConfirm()
        {

        }

        private void OnClickedOpen()
        {

        }
    }
}
