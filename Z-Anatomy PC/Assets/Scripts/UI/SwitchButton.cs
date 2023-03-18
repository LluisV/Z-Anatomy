using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SwitchButton : MonoBehaviour
{
    public Image background;
    public Image handler;
    public Image border;
    private RectTransform backgroundRt;
    private RectTransform handlerRt;
    private TextMeshProUGUI tmpro;

    public Transform start;
    public Transform end;

    private IEnumerator posCoroutine;
    private IEnumerator colorCoroutine;

    [Space]
    public bool isOn;

    [Space]
    public float animationTime;
    public AnimationCurve animationCurve;

    [Space]
    public Color onColor;
    public Color offColor;

    public Color onBorderColor;
    public Color offBorderColor;

    public Color onHandlerColor;
    public Color offHandlerColor;

    [Space]
    public TMP_FontAsset onFont;
    public TMP_FontAsset offFont;

    private void Awake()
    {
        backgroundRt = background.GetComponent<RectTransform>();
        handlerRt = handler.GetComponent<RectTransform>();
        tmpro = transform.parent.GetComponentInChildren<TextMeshProUGUI>();

        if (isOn)
            SetOn();
        else
            SetOff();
    }

    public void Click()
    {
        isOn = !isOn;

        if (posCoroutine != null)
            StopCoroutine(posCoroutine);
        if (colorCoroutine != null)
            StopCoroutine(colorCoroutine);

        if (isOn)
        {
            posCoroutine = LerpPosition(end.position);
            colorCoroutine = LerpColor(onColor);

            border.color = onBorderColor;
            handler.color = onHandlerColor;
            tmpro.font = onFont;
        }
        else
        {
            posCoroutine = LerpPosition(start.position);
            colorCoroutine = LerpColor(offColor);
            border.color = offBorderColor;
            handler.color = offHandlerColor;
            tmpro.font = offFont;

        }

        StartCoroutine(posCoroutine);
        StartCoroutine(colorCoroutine);

    }

    IEnumerator LerpColor(Color targetColor)
    {
        Color startColor = background.color;
        float time = 0;
        while (time < animationTime)
        {
            float t = time / animationTime;
            background.color = Color.Lerp(startColor, targetColor, animationCurve.Evaluate(t));
            //tmpro.color = Color.Lerp(startColor, targetColor, animationCurve.Evaluate(t));
            time += Time.deltaTime;
            yield return null;
        }
        background.color = targetColor;
        //tmpro.color = targetColor;
    }

    IEnumerator LerpPosition(Vector3 targetPos)
    {
        Vector3 startPos = handlerRt.position;
        float time = 0;
        while (time < animationTime)
        {
            float t = time / animationTime;
            handlerRt.position = Vector3.Lerp(startPos, targetPos, animationCurve.Evaluate(t));
            time += Time.deltaTime;
            yield return null;
        }
        handlerRt.position = targetPos;
    }

    public void SetOn()
    {
        isOn = true;
        border.color = onBorderColor;
        handler.color = onHandlerColor;
        background.color = onColor;
        handlerRt.position = end.position;
        tmpro.font = onFont;
    }

    public void SetOff()
    {
        isOn = false;
        border.color = offBorderColor;
        handler.color = offHandlerColor;
        background.color = offColor;
        handlerRt.position = start.position;
        tmpro.font = offFont;
    }
}
