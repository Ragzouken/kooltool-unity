using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class UIFollowTransform : MonoBehaviour 
{
    public Camera camera;
    public Transform target;

    private void Update()
    {
        Reposition();
    }

    public void Reposition()
    {
        if (target != null) MatchTransform(transform as RectTransform, target);
    }

    public void MatchTransform(RectTransform rtrans,
                               Transform transform)
    {
        Camera camera = this.camera != null ? this.camera : Camera.main;

        Vector3 local;
        Vector2 screen = camera.WorldToScreenPoint(transform.position);

        RectTransformUtility.ScreenPointToWorldPointInRectangle(rtrans.parent as RectTransform,
                                                                screen,
                                                                camera,
                                                                out local);

        rtrans.position = screen;
    }
}
