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

        protected IList<SizeIndicator> SizeIndicators = new List<SizeIndicator>();

        private Modes.Draw mode;

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
                    if (active) mode.thickness = thickness;
                };

                indicator.Toggle.onValueChanged.AddListener(toggled);

                SizeIndicators.Add(indicator);
            }

            EraserColor.Toggle.onValueChanged.AddListener(delegate(bool active)
            {
                if (active) mode.paintColour = Color.clear;
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
                    mode.paintColour = color;
                });
            }
        }

        private void Update()
        {
            SizeContainer.gameObject.SetActive(mode.tool != Modes.Draw.Tool.Fill);
        }

        public void SetSize(int size)
        {
            if (size - 1 < SizeIndicators.Count)
            {
                SizeIndicators[size - 1].Toggle.isOn = true;
            }
        }

        public void SetProject(Project project)
        {

        }

        public void SetPixelTool(Modes.Draw mode)
        {
            this.mode = mode;
        }

        public void OnToggledPencil(bool active)
        {
            if (active) mode.tool = Modes.Draw.Tool.Pencil;
        }

        public void OnToggledFill(bool active)
        {
            if (active) mode.tool = Modes.Draw.Tool.Fill;
        }

        public void OnToggledLine(bool active)
        {
            if (active) mode.tool = Modes.Draw.Tool.Line;
        }
    }
}
