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

        [SerializeField] protected ColorIndicator EraserColor;

        [Header("Size")]
        [Range(1, 32)]
        [SerializeField] protected int MaxSize;
        [SerializeField] protected ToggleGroup SizeToggleGroup;
        [SerializeField] protected RectTransform SizeContainer;
        [SerializeField] protected SizeIndicator SizePrefab;

        [Header("Colour")]
        [SerializeField] protected ToggleGroup ColorToggleGroup;
        [SerializeField] protected RectTransform ColorContainer;
        [SerializeField] protected ColorIndicator ColorPrefab;

        protected PixelTool Tool;

        private void Awake()
        {
            PencilToggle.onValueChanged.AddListener(OnToggledPencil);
            FillToggle.onValueChanged.AddListener(OnToggledFill);
            LineToggle.onValueChanged.AddListener(OnToggledLine);

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

            EraserColor.Toggle.onValueChanged.AddListener(delegate(bool active)
            {
                if (active) Tool.Color = Color.clear;
            });

            for (int i = 0; i < 10; ++i)
            {
                var color = new Color(Random.value, Random.value, Random.value);

                var indicator = Instantiate<ColorIndicator>(ColorPrefab);
                indicator.transform.SetParent(ColorContainer, false);
                indicator.Toggle.group = ColorToggleGroup;
                indicator.SetColor(color);

                indicator.Toggle.onValueChanged.AddListener(delegate(bool active)
                {
                    Tool.Color = color;
                });
            }
        }

        private void Update()
        {
            SizeContainer.gameObject.SetActive(Tool.Tool != PixelTool.ToolMode.Fill);
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
    }
}
