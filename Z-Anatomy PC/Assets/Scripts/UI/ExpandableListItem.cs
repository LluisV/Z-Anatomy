using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpandableListItem : MonoBehaviour
{
    public bool isExpanded;

    public float expandedHeight;
    public float collapsedHeight;

    public AnimationCurve positionCurve;

    [Space]
    public float durationOfAnimation;

    [Space]
    public RectTransform buttonToAnimate;
    public AnimationCurve rotationCurve;

    private RectTransform rt;
    private VerticalLayoutGroup verticalLayout;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        verticalLayout = GetComponentInParent<VerticalLayoutGroup>();
        SetUp();
    }

    public void SetUp()
    {
        if (isExpanded)
        {
            rt.SetHeight(expandedHeight);
            buttonToAnimate.localRotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            rt.SetHeight(collapsedHeight);
            buttonToAnimate.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    
    public void ExpandCollapseClick()
    {
        isExpanded = !isExpanded;
        if (isExpanded)
        {
            StartCoroutine(Expand());
            StartCoroutine(RotateIconCoroutine(180));
        }
        else
        {
            StartCoroutine(Collapse());
            StartCoroutine(RotateIconCoroutine(0));
        }
    }

    IEnumerator Expand()
    {
        float time = 0;
        while (time < durationOfAnimation)
        {
            if (verticalLayout != null)
                verticalLayout.enabled = false;
            float t = time / durationOfAnimation;
            float newHeight = Mathf.Lerp(collapsedHeight, expandedHeight, positionCurve.Evaluate(t));
            rt.SetHeight(newHeight);
            time += Time.deltaTime;
            if (verticalLayout != null)
                verticalLayout.enabled = true;
            yield return null;
        }
        rt.SetHeight(expandedHeight);
    }

    IEnumerator Collapse()
    {
        float time = 0;
        while (time < durationOfAnimation)
        {
            if (verticalLayout != null)
                verticalLayout.enabled = false;
            float t = time / durationOfAnimation;
            float newHeight = Mathf.Lerp(expandedHeight, collapsedHeight, positionCurve.Evaluate(t));
            rt.SetHeight(newHeight);
            time += Time.deltaTime;
            yield return null;
            if (verticalLayout != null)
                verticalLayout.enabled = true;
        }
        rt.SetHeight(collapsedHeight);

    }


    IEnumerator RotateIconCoroutine(float desiredAngle)
    {
        float startAngle = buttonToAnimate.eulerAngles.z;

        float time = 0;
        while (time < durationOfAnimation)
        {
            float t = time / durationOfAnimation;
            //Smooth step
            //  t = t * t * t * (t * (6f * t - 15f) + 10f);
            Vector3 rotate = Vector3.forward * Mathf.LerpAngle(startAngle, desiredAngle, rotationCurve.Evaluate(t));
            buttonToAnimate.transform.rotation = Quaternion.Euler(rotate);
            time += Time.deltaTime;
            yield return null;
        }

        buttonToAnimate.eulerAngles = Vector3.forward * desiredAngle;
    }
}
