using UnityEngine;
using UnityEngine.UI;

using System.Linq;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class RaycastMask : MonoBehaviour, ICanvasRaycastFilter
{
    [SerializeField] private Image _image;
    [SerializeField]
    [Range(0, 3)]
    private int border;

    private Sprite _sprite;

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        _sprite = _image.sprite;

        var rectTransform = (RectTransform)transform;
        Vector2 localPositionPivotRelative;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, sp, eventCamera, out localPositionPivotRelative);

        // convert to bottom-left origin coordinates
        var localPosition = new Vector2(localPositionPivotRelative.x + rectTransform.pivot.x * rectTransform.rect.width,
            localPositionPivotRelative.y + rectTransform.pivot.y * rectTransform.rect.height);

        var spriteRect = _sprite.textureRect;
        var maskRect = rectTransform.rect;

        var x = 0;
        var y = 0;
        // convert to texture space
        switch (_image.type)
        {

            case Image.Type.Sliced:
                {
                    var border = _sprite.border;
                    // x slicing
                    if (localPosition.x < border.x)
                    {
                        x = Mathf.FloorToInt(spriteRect.x + localPosition.x);
                    }
                    else if (localPosition.x > maskRect.width - border.z)
                    {
                        x = Mathf.FloorToInt(spriteRect.x + spriteRect.width - (maskRect.width - localPosition.x));
                    }
                    else
                    {
                        x = Mathf.FloorToInt(spriteRect.x + border.x +
                                             ((localPosition.x - border.x) /
                                             (maskRect.width - border.x - border.z)) *
                                             (spriteRect.width - border.x - border.z));
                    }
                    // y slicing
                    if (localPosition.y < border.y)
                    {
                        y = Mathf.FloorToInt(spriteRect.y + localPosition.y);
                    }
                    else if (localPosition.y > maskRect.height - border.w)
                    {
                        y = Mathf.FloorToInt(spriteRect.y + spriteRect.height - (maskRect.height - localPosition.y));
                    }
                    else
                    {
                        y = Mathf.FloorToInt(spriteRect.y + border.y +
                                             ((localPosition.y - border.y) /
                                             (maskRect.height - border.y - border.w)) *
                                             (spriteRect.height - border.y - border.w));
                    }
                }
                break;
            case Image.Type.Simple:
            default:
                {
                    // conversion to uniform UV space
                    x = Mathf.FloorToInt(spriteRect.x + spriteRect.width * localPosition.x / maskRect.width);
                    y = Mathf.FloorToInt(spriteRect.y + spriteRect.height * localPosition.y / maskRect.height);
                }
                break;
        }

        int xMin = Mathf.Clamp(x - this.border, 0, _sprite.texture.width  - 1);
        int yMin = Mathf.Clamp(y - this.border, 0, _sprite.texture.height - 1);
        int xMax = Mathf.Clamp(x + this.border, 0, _sprite.texture.width  - 1);
        int yMax = Mathf.Clamp(y + this.border, 0, _sprite.texture.height - 1);

        int width = xMax - xMin + 1;
        int height = yMax - yMin + 1;

        // destroy component if texture import settings are wrong
        try
        {
            Color[] pixels;

            try
            {
                pixels = _sprite.texture.GetPixels(xMin, yMin, width, height);
            }
            catch (System.OverflowException)
            {
                Debug.LogErrorFormat("{0}, {1}, {2}, {3}", xMin, yMin, xMax, yMax);

                return false;
            }

            return pixels.Any(c => c.a > 0);
        }
        catch (UnityException e)
        {
            Debug.LogError("Mask texture not readable, set your sprite to Texture Type 'Advanced' and check 'Read/Write Enabled'");
            Destroy(this);
            return false;
        }
    }
}
