using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class PixelTab : MonoBehaviour
    {
        [Header("Tools")]
        [SerializeField] protected Toggle PencilToggle;
        [SerializeField] protected Toggle FillToggle;
        [SerializeField] protected Toggle LineToggle;

        [SerializeField] protected Toggle EraserToggle;

        [Header("Size")]
        [Range(1, 32)]
        [SerializeField] protected int MaxSize;
        [SerializeField] protected ToggleGroup SizeToggleGroup;
        [SerializeField] protected RectTransform SizeContainer;
        [SerializeField] protected SizeIndicator SizePrefab;

        protected PixelTool Tool;

        protected void Awake()
        {
            PencilToggle.onValueChanged.AddListener(OnToggledPencil);
            FillToggle.onValueChanged.AddListener(OnToggledFill);
            LineToggle.onValueChanged.AddListener(OnToggledLine);

            EraserToggle.onValueChanged.AddListener(OnToggledEraser);

            for (int size = 1; size < MaxSize; ++size)
            {
                var indicator = Instantiate<SizeIndicator>(SizePrefab);
                indicator.transform.SetParent(SizeContainer, false);
                indicator.Toggle.group = SizeToggleGroup;
                indicator.SetSize(size);

                int thickness = size;

                UnityEngine.Events.UnityAction<bool> toggled;

                toggled = delegate (bool active)
                {
                    if (active) Tool.Thickness = thickness;
                };

                indicator.Toggle.onValueChanged.AddListener(toggled);
            }
        }

        public void SetPixelTool(PixelTool tool)
        {
            Tool = tool;
        }

        public void OnToggledPencil(bool active)
        {
            if (active) Tool.Tool = PixelTool.ToolMode.Pencil;
        }

        public void OnToggledFill(bool active)
        {
            if (active) Tool.Tool = PixelTool.ToolMode.Fill;
        }

        public void OnToggledLine(bool active)
        {
            if (active) Tool.Tool = PixelTool.ToolMode.Line;
        }

        public void OnToggledEraser(bool active)
        {
            if (active) Tool.SetErase();
        }
    }
}
