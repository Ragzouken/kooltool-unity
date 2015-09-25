using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using kooltool.Data;

namespace kooltool.Editor
{
    public class CostumeTab : MonoBehaviour
    {
        [SerializeField] protected Editor Editor;

        [Header("Tools")]
        [SerializeField] protected Button NewButton;

        [Header("Tiles")]
        [SerializeField] protected ToggleGroup CostumeToggleGroup;
        [SerializeField] protected RectTransform CostumeContainer;
        [SerializeField] protected CostumeIndicator CostumePrefab;

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
            indicator.SetCostume(costume, () => Editor.MakeCharacter(costume));
        }

        public void Refresh()
        {
            costumes.SetActive(Editor.project_.costumes);
        }

        public void OnClickedNew()
        {
            Editor.project_.costumes.Add(Generators.Costume.Smiley(Editor.project_, 32, 32));

            Refresh();
        }
    }
}
