using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

[ExecuteInEditMode]
public class UIRectFollowRect : MonoBehaviour 
{
    public new Camera camera;
    public RectTransform target;

    private void Update()
    {
        Reposition();
    }

    public void Reposition()
    {
        if (target != null) MatchRect(transform as RectTransform, target);
    }

    public void MatchRect(RectTransform rtrans,
                          RectTransform ttrans)
    {
        Camera camera = this.camera != null ? this.camera : Camera.main;

        var world = new Vector3[4];
        var screen = new Vector2[4];

        ttrans.GetWorldCorners(world);

        for (int i = 0; i < world.Length; ++i)
        {
            screen[i] = camera.WorldToScreenPoint(world[i]);
        }

        float x1 = screen.Min(v => v.x);
        float x2 = screen.Max(v => v.x);
        float y1 = screen.Min(v => v.y);
        float y2 = screen.Max(v => v.y);

        float w = x2 - x1;
        float h = y2 - y1;

        rtrans.position = new Vector2(x1, y1);
        rtrans.sizeDelta = new Vector2(w, h);
    }
}
