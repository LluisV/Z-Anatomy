using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IncreaseHeigth : MonoBehaviour
{

    const float AnimationTime = 1;
    private float normalHeigth;
    public float expandedHeightMultplier;
    private RectTransform rt;
    public TextMeshProUGUI tmpro;

    private IEnumerator coroutine;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    private void Start()
    {
        normalHeigth = rt.GetSize().y;
    }

    public void SetNormal()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = IncreaseHeight(normalHeigth);
        StartCoroutine(coroutine);
    }

    public void SetExpanded()
    {
        if (tmpro.isTextTruncated)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutine = IncreaseHeight(normalHeigth * expandedHeightMultplier);
            StartCoroutine(coroutine);
        }
    }

    private IEnumerator IncreaseHeight(float targetHeight)
    {
        float startValue = rt.GetHeight();
        float time = 0;
        while (time < AnimationTime)
        {
            float t = time / AnimationTime;
            //Smooth step
            rt.SetHeight(Mathf.Lerp(startValue, targetHeight, t));
            time += Time.deltaTime;
            yield return new WaitForSeconds(.15f);
        }
        rt.SetHeight(targetHeight);
    }
}
