using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpandCollapseUI : MonoBehaviour
{
    public Vector2 collapasedPosition;
    public Vector2 expandedPosition;
    public RectTransform buttonToAnimate;
    public float durationOfAnimation;
    public float rotateButtonVelocity;

    [Header("Define the actual state")]
    public bool isExpanded;

    [Header("Movement allowed")]
    public bool vertical;
    public bool horizontal;

    [Header("Disable this gameobject when it's collapsed")]
    public bool disbleGameobjectOnCollapse;

    [Header("Disable all body when it's expanded")]
    public bool disbleBodyOnCollapse;
    public GameObject allBody;

    [Header("Disable image when it's collapsed")]
    public bool disbleOnCollapse;
    public Image imageToDisable;

    private bool onRoutine;
    private RectTransform trans;

    private void Awake()
    {
        trans = GetComponent<RectTransform>();
        if(disbleOnCollapse)
            imageToDisable.enabled = disbleOnCollapse && isExpanded;
    }

    public void ExpandButtonClick()
    {
        if (!onRoutine)
        {
            onRoutine = true;
            gameObject.SetActive(true);
            if (isExpanded)
                StartCoroutine(CollapseCoroutine());
            else
                StartCoroutine(ExpandCoroutine());

            if(buttonToAnimate != null)
                StartCoroutine(RotateIconCoroutine());

            isExpanded = !isExpanded;

        }
    }

    IEnumerator CollapseCoroutine()
    {
        if (disbleBodyOnCollapse)
            allBody.SetActive(true);
        Vector3 startPosition = trans.anchoredPosition;
        Vector3 targetPosition = collapasedPosition;
        if (!horizontal)
            targetPosition.x = startPosition.x;
        if (!vertical)
            targetPosition.y = startPosition.y;
        float time = 0;
        while (time < durationOfAnimation)
        {
            float t = time / durationOfAnimation;
            //Smooth step
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            trans.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, t);
            time += Time.deltaTime;
            yield return null;
        }
        onRoutine = false;
        trans.anchoredPosition = targetPosition;
        if (disbleOnCollapse)
            imageToDisable.enabled = false;
        if (disbleGameobjectOnCollapse)
            gameObject.SetActive(false);
    }

    IEnumerator ExpandCoroutine()
    {
        if (disbleOnCollapse)
            imageToDisable.enabled = true;
        Vector3 startPosition = trans.anchoredPosition;
        Vector3 targetPosition = expandedPosition;
        if (!horizontal)
            targetPosition.x = startPosition.x;
        if (!vertical)
            targetPosition.y = startPosition.y;
        float time = 0;
        while (time < durationOfAnimation)
        {
            float t = time / durationOfAnimation;
            //Smooth step
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            trans.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, t);
            time += Time.deltaTime;
            yield return null;
        }
        onRoutine = false;
        trans.anchoredPosition = targetPosition;
        if (disbleBodyOnCollapse)
            allBody.SetActive(false);
    }

    IEnumerator RotateIconCoroutine()
    {
        float startAngle = buttonToAnimate.eulerAngles.z;
        float desiredAngle = buttonToAnimate.eulerAngles.z + 180;
        for (float t = 0; t < 1; t += Time.deltaTime * rotateButtonVelocity)
        {
            Vector3 rotate = Vector3.forward * Mathf.LerpAngle(startAngle, desiredAngle, t);
            buttonToAnimate.transform.rotation = Quaternion.Euler(rotate);
            yield return null;
        }
        buttonToAnimate.eulerAngles = Vector3.forward * desiredAngle;
        onRoutine = false;
    }

    public void SetCollapsedPositionX(int newX)
    {
        collapasedPosition.x = newX;
    }

    public void Collapse()
    {
        onRoutine = true;
        StartCoroutine(CollapseCoroutine());
        isExpanded = false;
    }
}
