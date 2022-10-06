using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ExpandTextUI : MonoBehaviour
{
    public float fontSize;
    public string text;
    public float durationOfAnimation;

    private TextMeshProUGUI tmpro;
    private RectMask2D mask;
    private float paddingValue;

    private void Awake()
    {
        tmpro = GetComponent<TextMeshProUGUI>();
        tmpro.fontSize = fontSize;
        tmpro.horizontalAlignment = HorizontalAlignmentOptions.Left;
        tmpro.verticalAlignment = VerticalAlignmentOptions.Middle;
        mask = GetComponentInParent<RectMask2D>();

    }

    private IEnumerator Start()
    {
        yield return null;
        paddingValue = mask.rectTransform.rect.width;
        mask.padding = new Vector4(0, 0, paddingValue, 0);
    }

    private void OnEnable()
    {
        tmpro.text = "";
    }

    public void Expand()
    {
        StopAllCoroutines();
        StartCoroutine(ExpandCoroutine());      
    }

    public void Collapse()
    {
        StopAllCoroutines();
        StartCoroutine(CollapseCoroutine());
    }

    IEnumerator CollapseCoroutine()
    {
        float time = 0;
        float initialValue = mask.padding.z;
        while (time < durationOfAnimation)
        {
            float t = time / durationOfAnimation;
            //Smooth step
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            float value = Mathf.Lerp(initialValue, paddingValue, t);
            mask.padding = new Vector4(0, 0, value, 0);
            time += Time.deltaTime;
            yield return null;
        }
        tmpro.text = "";
    }

    IEnumerator ExpandCoroutine()
    {
        tmpro.text = text;
        float time = 0;
        float initialValue = 0;
        while (time < durationOfAnimation)
        {
            float t = time / durationOfAnimation;
            //Smooth step
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            float value = Mathf.Lerp(initialValue, 0, t);
            mask.padding = new Vector4(0, 0, value, 0);
            time += Time.deltaTime;
            yield return null;
        }
    }

}
