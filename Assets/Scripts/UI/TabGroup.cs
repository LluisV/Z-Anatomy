using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{

    [Header("Bar type")]
    public bool horizontal;
    public bool vertical;

    [Space]
    [Header("Settings")]
    public float foldedSize;
    public float unfoldedSize;
    public float mouseOverAnimationTime;
    public float cornersRadius;
    public float separatorHeight;
    public Color backgroundSelectedColor;
    public Color backgroundDeselectedColor;
    public Color textSelectedColor;
    public Color textDeselectedColor;
    public Color shadowSelectedColor;
    public Color shadowDeselectedColor;
    public TMP_FontAsset deselectedFont;
    public TMP_FontAsset selectedFont;


    [Space]
    [Header("Movement curves")]
    public AnimationCurve positionCurve;

    [Space]
    [Header("References")]
    public RectTransform positionBar;

    private IEnumerator setBarPosCoroutine;
    private IEnumerator setBarSizeCoroutine;

    public List<Tab> tabs;
    public List<RectTransform> separators;
    private IEnumerator[] separatorsAnimations;

    private void Awake()
    {
        separatorsAnimations = new IEnumerator[separators.Count];
    }


    public void SetBarPosition(Vector3 targetPosition, float targetSize)
    {
        if (setBarPosCoroutine != null)
            StopCoroutine(setBarPosCoroutine);
        if(setBarSizeCoroutine != null)
            StopCoroutine(setBarSizeCoroutine);

        setBarPosCoroutine = setBarPos(positionBar.position, targetPosition);
        if(vertical)
            setBarSizeCoroutine = setBarHeight(targetSize);
        else
            setBarSizeCoroutine = setBarWidth(targetSize);

        StartCoroutine(setBarPosCoroutine);
        StartCoroutine(setBarSizeCoroutine);
    }


    IEnumerator setBarPos(Vector3 startPosition, Vector3 targetPosition)
    {
        if (horizontal)
            targetPosition.y = startPosition.y;
        if (vertical)
            targetPosition.x = startPosition.x;

        float time = 0;
        while (time < mouseOverAnimationTime)
        {
            float t = time / mouseOverAnimationTime;
            positionBar.position = Vector3.Lerp(startPosition, targetPosition, positionCurve.Evaluate(t));
            time += Time.deltaTime;
            yield return null;
        }
        positionBar.position = targetPosition;
    }

    IEnumerator setBarHeight(float targetHeight)
    {
        float startHeight = positionBar.GetHeight();
        float time = 0;
        while (time < mouseOverAnimationTime)
        {
            float t = time / mouseOverAnimationTime;
            positionBar.SetHeight(Mathf.Lerp(startHeight, targetHeight, positionCurve.Evaluate(t)));
            time += Time.deltaTime;
            yield return null;
        }
        positionBar.SetHeight(targetHeight);
    }


    IEnumerator setBarWidth(float targetWidth)
    {
        float startHeight = positionBar.GetWidth();
        float time = 0;
        while (time < mouseOverAnimationTime)
        {
            float t = time / mouseOverAnimationTime;
            positionBar.SetWidth(Mathf.Lerp(startHeight, targetWidth, positionCurve.Evaluate(t)));
            time += Time.deltaTime;
            yield return null;
        }
        positionBar.SetWidth(targetWidth);
    }

    IEnumerator setSeparatorHeight(RectTransform rt, float targetHeight)
    {
        float startHeight = rt.GetHeight();
        float time = 0;
        while (time < (mouseOverAnimationTime + 0.05f))
        {
            float t = time / (mouseOverAnimationTime + 0.05f);
            rt.SetHeight(Mathf.Lerp(startHeight, targetHeight, positionCurve.Evaluate(t)));
            time += Time.deltaTime;
            yield return null;
        }
        rt.SetHeight(targetHeight);
    }

    public void TabSelected(Tab tab, bool selected)
    {
        int index = tabs.IndexOf(tab);
 
        //If it is the first tab
        if(index == 0)
        {
            if (selected)
            {
                if (tabs[index + 1].selected)
                {
                    tab.RemoveRightCorner();
                    tabs[index + 1].RemoveLeftCorner();
                }
                else
                    tab.SetRightCorner();

                AnimateSeparator(index, 0);

            }
            else
            {
                tab.RemoveRightCorner();
                if (tabs[index + 1].selected)
                    tabs[index + 1].SetLeftCorner();
                else
                    AnimateSeparator(index, separatorHeight);
            }
        }
        //If it is the last tab
        else if(index == tabs.Count - 1)
        {
            if (selected)
            {
                if (tabs[index - 1].selected)
                {
                    tab.RemoveLeftCorner();
                    tabs[index - 1].RemoveRightCorner();
                }
                else
                    tab.SetLeftCorner();

                AnimateSeparator(separators.Count - 1, 0);
            }
            else
            {
                tab.RemoveLeftCorner();
                if (tabs[index - 1].selected)
                    tabs[index - 1].SetRightCorner();
                else
                    AnimateSeparator(separators.Count - 1, separatorHeight);
            }
        }
        else
        {
            if (selected)
            {
                if (tabs[index - 1].selected)
                {
                    tab.RemoveLeftCorner();
                    tabs[index - 1].RemoveRightCorner();
                }
                else
                    tab.SetLeftCorner();

                if (tabs[index + 1].selected)
                {
                    tab.RemoveRightCorner();
                    tabs[index + 1].RemoveLeftCorner();
                }
                else
                    tab.SetRightCorner();

                AnimateSeparator(index - 1, 0);
                AnimateSeparator(index, 0);
            }
            else
            {
                tab.RemoveRightCorner();
                tab.RemoveLeftCorner();

                if (tabs[index - 1].selected)
                    tabs[index - 1].SetRightCorner();
                else
                    AnimateSeparator(index - 1, separatorHeight);

                if (tabs[index + 1].selected)
                    tabs[index + 1].SetLeftCorner();
                else
                    AnimateSeparator(index, separatorHeight);

            }
        }

    }

    private void AnimateSeparator(int index, float targetValue)
    {
        if (separatorsAnimations[index] != null)
            StopCoroutine(separatorsAnimations[index]);
        separatorsAnimations[index] = setSeparatorHeight(separators[index], targetValue);
        StartCoroutine(separatorsAnimations[index]);
    }
}
