using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class Tooltip : MonoBehaviour 
{
    [SerializeField] private GameObject container;
    [SerializeField] private Text text;
    [SerializeField] private float delay = 1f;

    private TooltipTrigger trigger;
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (trigger != null 
         && timer >= delay
         && !container.activeSelf)
        {
            Open();
            Reposition(trigger);
            SetText(trigger.text);
        }
    }

    public void Enter(TooltipTrigger trigger)
    {
        this.trigger = trigger;
        timer = 0;
    }

    public void Exit(TooltipTrigger trigger)
    {
        if (this.trigger == trigger)
        {
            Close();

            this.trigger = null;
        }
    }

    public void Open()
    {
        container.SetActive(true);
    }

    public void Close()
    {
        container.SetActive(false);
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void Reposition(TooltipTrigger trigger)
    {
        var rtrans = transform as RectTransform;
        var ptrans = transform.parent.transform as RectTransform;
        var ttrans = trigger.transform as RectTransform;

        var corners = new Vector3[4];

        ttrans.GetWorldCorners(corners);

        rtrans.position = Vector2.Lerp(corners[0], corners[3], 0.5f);
    }

    public void Reposition()
    {
        var rtrans = transform as RectTransform;
        var ptrans = transform.parent.transform as RectTransform;

        Vector3 pos;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(ptrans, Input.mousePosition, null, out pos);

        rtrans.position = pos;

        Vector3 minPosition = ptrans.rect.min - rtrans.rect.min;
        Vector3 maxPosition = ptrans.rect.max - rtrans.rect.max;

        pos.x = Mathf.Clamp(rtrans.localPosition.x, minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(rtrans.localPosition.y, minPosition.y, maxPosition.y);

        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f;

        rtrans.localPosition = pos;
    }
}
