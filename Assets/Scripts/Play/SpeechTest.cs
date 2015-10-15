using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class SpeechTest : MonoBehaviour 
{
    [SerializeField] protected Text text;

    protected float timer;

    public void Setup(string text, float duration)
    {
        this.text.text = text;

        timer = duration;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0) Destroy(gameObject);
    }
}
