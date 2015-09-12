using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class NotesTab : MonoBehaviour
    {
        [Header("Tools")]
        [SerializeField] private Toggle pencilToggle;
        [SerializeField] private Toggle lineToggle;
        [SerializeField] private Toggle eraserToggle;

        [Header("Size")]
        [Range(1, 32)]
        [SerializeField] private int maxSize;
        [SerializeField] private ToggleGroup sizeToggleGroup;
        [SerializeField] private RectTransform sizeContainer;
        [SerializeField] private SizeIndicator sizePrefab;

        private IList<SizeIndicator> sizeIndicators = new List<SizeIndicator>();

        private Modes.Notes mode;

        private void Awake()
        {
            pencilToggle.onValueChanged.AddListener(OnToggledPencil);
            lineToggle.onValueChanged.AddListener(OnToggledLine);
            eraserToggle.onValueChanged.AddListener(OnToggledEraser);

            for (int size = 1; size < maxSize; ++size)
            {
                var indicator = Instantiate<SizeIndicator>(sizePrefab);
                indicator.transform.SetParent(sizeContainer, false);
                indicator.Toggle.group = sizeToggleGroup;
                indicator.SetSize(size);

                int thickness = size;

                UnityEngine.Events.UnityAction<bool> toggled;

                toggled = delegate (bool active)
                {
                    if (active) mode.thickness = thickness;
                };

                indicator.Toggle.onValueChanged.AddListener(toggled);

                sizeIndicators.Add(indicator);
            }
        }

        public void SetSize(int size)
        {
            if (size - 1 < sizeIndicators.Count)
            {
                sizeIndicators[size - 1].Toggle.isOn = true;
            }
        }

        public void SetProject(Project project)
        {

        }

        public void SetNotesTool(Modes.Notes mode)
        {
            this.mode = mode;
        }

        public void OnToggledPencil(bool active)
        {
            if (active) mode.tool = Modes.Notes.Tool.Pencil;
        }

        public void OnToggledLine(bool active)
        {
            if (active) mode.tool = Modes.Notes.Tool.Line;
        }

        public void OnToggledEraser(bool active)
        {
            mode.erase = active;
        }
    }
}
