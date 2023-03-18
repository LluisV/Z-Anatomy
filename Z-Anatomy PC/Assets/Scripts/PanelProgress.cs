using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class PanelProgress : MonoBehaviour
{

    public int initialPosition;
    public float spacing;
    public float size;
    public Sprite sprite;
    public Color enabledColor;
    public Color disabledColor;
    public float animationTime;
    public AnimationCurve positionCurve;
    public AnimationCurve heightCurve;
    public AnimationCurve widthCurve;
    public AnimationCurve yPosCurve;
    public bool animatePos;
    public bool animateSize;
    public bool animateJump;

    private RectTransform rt;
    private IEnumerator buildCoroutine;
    private HorizontalLayoutGroup layout;
    private Image[] states;
    private Image currentState;
    private int currentIndex;
    private IEnumerator lerpPosCoroutine;

    public bool refresh;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if(refresh)
        {
            refresh = false;
            layout = GetComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = false;
            layout.spacing = spacing;
            currentIndex = initialPosition;
            Build();
        }
    }

    private void Build()
    {
        if (Application.isPlaying || !isActiveAndEnabled)
            return;
        rt = GetComponent<RectTransform>();

        if (rt == null)
            return;

        if (buildCoroutine != null)
            StopCoroutine(buildCoroutine);
        buildCoroutine = build();
        StartCoroutine(buildCoroutine);
    }
    private IEnumerator build()
    {
        yield return StartCoroutine(ClearChilds());
        InstantiateStates();
    }


    private IEnumerator ClearChilds()
    {
        foreach (Transform child in transform)
        {
            EditorApplication.delayCall += () =>
            {
                if (child != null)
                    DestroyImmediate(child.gameObject);
            };
        }

        yield return new WaitUntil(() => transform.childCount == 0);

    }

    private void InstantiateStates()
    {
        StartCoroutine(instantiateStates());
        IEnumerator instantiateStates()
        {
            yield return null;

            states = new Image[transform.parent.childCount - 1];

            for (int i = 0; i < transform.parent.childCount - 1; i++)
                states[i] = CreateState("State " + i);
            yield return null;

            currentState = CreateState("Current");
            currentState.color = enabledColor;
            currentState.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            currentState.transform.position = states[initialPosition].transform.position;
        }
    }

    private Image CreateState(string name)
    {
        Image state = new GameObject().AddComponent<Image>();
        state.name = name;
        state.transform.parent = transform;
        state.sprite = sprite;
        state.color = disabledColor;
        state.rectTransform.SetWidth(size);
        state.rectTransform.SetHeight(size);

        return state;
    }

#endif

    private void Awake()
    {
        currentIndex = initialPosition;
        layout = GetComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childForceExpandWidth = false;
        layout.spacing = spacing;
        currentIndex = initialPosition;
        states = GetComponentsInChildren<Image>();
        currentState = states.Where(it => it.name == "Current").First();
    }

    public void Next()
    {
        if (lerpPosCoroutine != null)
        {
            StopCoroutine(lerpPosCoroutine);
            currentState.transform.position = states[currentIndex].transform.position;
        }

        currentIndex++;

        lerpPosCoroutine = LerpCurrentPos(states[currentIndex].transform.position, animationTime);
        StartCoroutine(lerpPosCoroutine);
    }

    public void Previous()
    {
        if (lerpPosCoroutine != null)
        {
            StopCoroutine(lerpPosCoroutine);
            currentState.transform.position = states[currentIndex].transform.position;
        }

        currentIndex--;

        lerpPosCoroutine = LerpCurrentPos(states[currentIndex].transform.position, animationTime);
        StartCoroutine(lerpPosCoroutine);
    }

    IEnumerator LerpCurrentPos(Vector3 newPos, float animTime)
    {
        if (!animatePos)
            animTime = 0;
        Vector3 prevPosition = currentState.transform.position;
        float time = 0;
        while (time < animTime)
        {
            float t = time / animTime;
            if (animateJump)
                currentState.transform.position = Vector3.Lerp(prevPosition, newPos + Vector3.up * prevPosition.y * yPosCurve.Evaluate(t), positionCurve.Evaluate(t));
            else
                currentState.transform.position = Vector3.Lerp(prevPosition, newPos, positionCurve.Evaluate(t));
            if(animateSize)
            {
                currentState.rectTransform.SetWidth(Mathf.Lerp(size, size * widthCurve.Evaluate(t), t));
                currentState.rectTransform.SetHeight(Mathf.Lerp(size, size * heightCurve.Evaluate(t), t));
            }

            time += Time.deltaTime;
            yield return null;
        }
        currentState.transform.position = newPos;
        currentState.rectTransform.SetWidth(size);
        currentState.rectTransform.SetHeight(size);
    }

    public void ResetPosition()
    {
        currentIndex = initialPosition;
        currentState.transform.position = states[initialPosition].transform.position;
    }
}
