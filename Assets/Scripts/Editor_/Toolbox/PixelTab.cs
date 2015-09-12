using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class PixelTab : MonoBehaviour
    {
        [Header("Tools")]
        [SerializeField] private Toggle PencilToggle;
        [SerializeField] private Toggle FillToggle;
        [SerializeField] private Toggle LineToggle;
        [SerializeField] private Toggle stampToggle;

        [SerializeField] private ColorIndicator EraserColor;

        [Header("Size")]
        [Range(1, 32)]
        [SerializeField] private int MaxSize;
        [SerializeField] private ToggleGroup SizeToggleGroup;
        [SerializeField] private RectTransform SizeContainer;
        [SerializeField] private SizeIndicator SizePrefab;

        [Header("Colour")]
        [SerializeField] private ToggleGroup ColorToggleGroup;
        [SerializeField] private RectTransform ColorContainer;
        [SerializeField] private ColorIndicator ColorPrefab;
        [SerializeField] private Image colourBackgroundImage;

        [Header("Stamps")]
        [SerializeField] private GameObject brushSizeDisableObject;
        [SerializeField] private GameObject stampTabsDisableObject;

        private IList<SizeIndicator> SizeIndicators = new List<SizeIndicator>();

        private Modes.Draw mode;

        private void Awake()
        {
            PencilToggle.onValueChanged.AddListener(OnToggledPencil);
            FillToggle.onValueChanged.AddListener(OnToggledFill);
            LineToggle.onValueChanged.AddListener(OnToggledLine);
            stampToggle.onValueChanged.AddListener(OnToggledStamp);

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
                if (active) mode.paintColour = Modes.Draw.eraseColour;
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
            brushSizeDisableObject.SetActive(mode.tool == Modes.Draw.Tool.Pencil 
                                          || mode.tool == Modes.Draw.Tool.Line);
            stampTabsDisableObject.SetActive(mode.tool == Modes.Draw.Tool.Stamp);
            
            colourBackgroundImage.color = mode.paintColour;
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

        public void OnToggledStamp(bool active)
        {
            if (active) mode.tool = Modes.Draw.Tool.Stamp;
        }
    }
}