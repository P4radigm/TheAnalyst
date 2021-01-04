using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PingPongAlpha : MonoBehaviour
{
    public float speed = 1f;
    private CanvasGroup target;

    private float _t = 0f;

    private void Start()
    {
        if (GetComponent<CanvasGroup>() != null)
            target = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        _t += Time.deltaTime * speed;
        target.alpha = Mathf.PingPong(_t, 1f);
    }
}
