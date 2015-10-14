using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GrowingInputField : MonoBehaviour
{
    [SerializeField] private InputField input;
    [SerializeField] private Text text;
    [SerializeField] private LayoutElement layout;
    [SerializeField] private RectTransform rtrans;

    private RectTransform Caret;
    private string saved;

    protected void Start()
    {
       // input.onValueChange.AddListener(Grow);
    }

    private void Update()
    {
        Caret = input.transform.GetChild(0) as RectTransform;

        Vector2 extents = input.textComponent.rectTransform.rect.size;
        var settings = input.textComponent.GetGenerationSettings(extents);
        settings.generateOutOfBounds = true;

        var fake = input.text.Replace(' ', '_');

        float width = new TextGenerator().GetPreferredWidth(fake, settings);
        float height = new TextGenerator().GetPreferredHeight(fake, settings);

        layout.preferredWidth = width;
        layout.preferredHeight = height;

        rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        input.ActivateInputField();

        this.text.color = Color.white;
    }
}
