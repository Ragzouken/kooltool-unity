using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using kooltool.Data;

namespace kooltool.Editor
{
    public class CostumeTab : MonoBehaviour
    {
        [SerializeField] private Toolbox toolbox;

        [Header("Tools")]
        [SerializeField] private Button NewButton;
        [SerializeField] private GameObject trashIcon;

        [Header("Tiles")]
        [SerializeField] private ToggleGroup CostumeToggleGroup;
        [SerializeField] private RectTransform CostumeContainer;
        [SerializeField] private CostumeIndicator CostumePrefab;

        private MonoBehaviourPooler<Costume, CostumeIndicator> costumes;

        private void Awake()
        {
            NewButton.onClick.AddListener(OnClickedNew);

            costumes = new MonoBehaviourPooler<Costume, CostumeIndicator>(CostumePrefab,
                                                                          CostumeContainer,
                                                                          InitialiseCostume);

            Refresh();
        }

        private void InitialiseCostume(Costume costume, 
                                       CostumeIndicator indicator)
        {
            indicator.SetCostume(toolbox, costume, () => Editor.Instance.MakeCharacter(costume));
        }

        private void Update()
        {
            NewButton.gameObject.SetActive(toolbox.draggedItem == null);
            trashIcon.gameObject.SetActive(toolbox.draggedItem != null);
        }

        public void Refresh()
        {
            costumes.SetActive(Editor.Instance.project_.costumes);
        }

        public void OnClickedNew()
        {
            Editor.Instance.project_.costumes.Add(Generators.Costume.Smiley(Editor.Instance.project_, 32, 32));

            Refresh();
        }

        public void OnDroppedTrash()
        {
            var costume = toolbox.draggedItem as Costume;

            if (costume != null)
            {
                toolbox.CancelDrag();
                toolbox.editor.project_.RemoveCostume(costume);
                Refresh();
            }
        }
    }
}
