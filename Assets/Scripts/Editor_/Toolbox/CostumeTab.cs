using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

        protected TileTool Tool;

        protected ChildElements<CostumeIndicator> Costumes;

        private void Awake()
        {
            NewButton.onClick.AddListener(OnClickedNew);

            Costumes = new ChildElements<CostumeIndicator>(CostumeContainer, CostumePrefab);

            Refresh();
        }

        public void SetTileTool(TileTool tool)
        {
            Tool = tool;
        }

        public void Refresh()
        {
            Costumes.Clear();

            foreach (Costume costume in Editor.Project.Costumes)
            {
                CostumeIndicator element = Costumes.Add();

                element.SetCostume(costume);

                element.Button.onClick.RemoveAllListeners();
                element.Button.onClick.AddListener(delegate
                {
                    Editor.MakeCharacter(element.Costume);
                });
            }
        }

        public void OnClickedNew()
        {
            //Tool.PaintTile = Editor.Project.Tileset.AddTile();
            Editor.Project.Costumes.Add(Generators.Costume.Smiley(Editor.Project.Grid.CellWidth, Editor.Project.Grid.CellHeight));

            Refresh();
        }
    }
}
