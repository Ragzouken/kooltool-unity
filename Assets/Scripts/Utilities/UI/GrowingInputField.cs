using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GrowingInputField : MonoBehaviour
{
    [SerializeField] protected InputField Input;
    [SerializeField] protected Text Text;
    [SerializeField] protected LayoutElement Layout;
    [SerializeField] protected RectTransform Caret;

    string saved;

    protected void Start()
    {
        Input.onValueChange.AddListener(Grow);
    }

    protected void Grow(string text)
    {
        Caret = Input.transform.GetChild(0) as RectTransform;

        Vector2 extents = Input.textComponent.rectTransform.rect.size;
        var settings = Input.textComponent.GetGenerationSettings(extents);
        settings.generateOutOfBounds = false;

        float width = new TextGenerator().GetPreferredWidth(text.Replace(' ', '_'), settings);
        float height = new TextGenerator().GetPreferredHeight(text.Replace(' ', '_'), settings);

        Layout.preferredWidth = width;
        Layout.preferredHeight = height;

        Caret.anchoredPosition = new Vector2(width / 2f + 5, -(height / 2f + 5));
        Caret.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        Caret.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        Input.ActivateInputField();

        Text.color = Color.white;
    }
}
