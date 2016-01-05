using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(RawImage))]
public class RaycastMaskRaw : MonoBehaviour, ICanvasRaycastFilter
{
    [SerializeField] private RawImage _image;

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        var texture = _image.texture as RenderTexture;

        var rectTransform = (RectTransform)transform;
        Vector2 localPositionPivotRelative;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, sp, eventCamera, out localPositionPivotRelative);

        // convert to bottom-left origin coordinates
        var localPosition = new Vector2(localPositionPivotRelative.x + rectTransform.pivot.x * rectTransform.rect.width,
            localPositionPivotRelative.y + rectTransform.pivot.y * rectTransform.rect.height);

        var maskRect = rectTransform.rect;

        var x = 0;
        var y = 0;

        {
            // conversion to uniform UV space
            x = Mathf.FloorToInt(_image.uvRect.width  * texture.width  * localPosition.x / maskRect.width);
            y = Mathf.FloorToInt(_image.uvRect.height * texture.height * localPosition.y / maskRect.height);
        }

        // destroy component if texture import settings are wrong
        try
        {
            return false;
            //return texture.GetPixel(x, y).a > 0;
        }
        catch (UnityException e)
        {
            Debug.LogError("Mask texture not readable, set your sprite to Texture Type 'Advanced' and check 'Read/Write Enabled'");
            Destroy(this);
            return false;
        }
    }
}
