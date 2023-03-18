using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RectangleOutline : MonoBehaviour
{
    public Color color;
    public float width;

    private Image up;
    private Image down;
    private Image left;
    private Image right;

    private RectTransform rt;

    private IEnumerator buildCoroutine;
    public bool refresh;

#if UNITY_EDITOR

    [MenuItem("GameObject/UI/Rectangle", false, -10)]
    static void Rectangle()
    {
        var newGo = new GameObject();
        RectangleOutline script = newGo.AddComponent <RectangleOutline>();
        script.width = 2;
        script.color = Color.white;
        newGo.AddComponent<RectTransform>();
        newGo.name = "Rectangle";
        newGo.transform.parent = Selection.activeTransform;
        newGo.transform.position = newGo.transform.parent.position;
        script.Build();
    }

    private void OnValidate()
    {
        if(refresh)
        {
            refresh = false;
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
        InstantiateLines();
    }

    private IEnumerator ClearChilds()
    {
        foreach (Transform child in transform)
        {
            EditorApplication.delayCall += () =>
            {
                if(child != null)
                    DestroyImmediate(child.gameObject);
            };
        }

        yield return new WaitUntil(() => transform.childCount == 0);

    }


    private void InstantiateLines()
    {
        StartCoroutine(instantiateLines());
        IEnumerator instantiateLines()
        {
            yield return null;
            var go1 = new GameObject();
            var go2 = new GameObject();
            var go3 = new GameObject();
            var go4 = new GameObject();

            go1.transform.SetParent(transform);
            go2.transform.SetParent(transform);
            go3.transform.SetParent(transform);
            go4.transform.SetParent(transform);

            up = go1.AddComponent<Image>();
            down = go2.AddComponent<Image>();
            left = go3.AddComponent<Image>();
            right = go4.AddComponent<Image>();

            up.color = color;
            down.color = color;
            left.color = color;
            right.color = color;


            up.rectTransform.SetHeight(width);
            up.rectTransform.SetWidth(Mathf.Abs(rt.GetWidth()) + width);

            down.rectTransform.SetHeight(width);
            down.rectTransform.SetWidth(Mathf.Abs(rt.GetWidth()) + width);

            left.rectTransform.SetWidth(width);
            left.rectTransform.SetHeight(Mathf.Abs(rt.GetHeight()) + width);

            right.rectTransform.SetWidth(width);
            right.rectTransform.SetHeight(Mathf.Abs(rt.GetHeight()) + width);

            up.rectTransform.localPosition = new Vector2((rt.rect.xMin + rt.rect.xMax) * .5f, rt.rect.yMax);
            down.rectTransform.localPosition = new Vector2((rt.rect.xMin + rt.rect.xMax) * .5f, rt.rect.yMin);
            left.rectTransform.localPosition = new Vector2(rt.rect.xMin, (rt.rect.yMin + rt.rect.yMax) * .5f);
            right.rectTransform.localPosition = new Vector2(rt.rect.xMax, (rt.rect.yMin + rt.rect.yMax) * .5f);


            up.transform.localScale = Vector3.one;
            down.transform.localScale = Vector3.one;
            left.transform.localScale = Vector3.one;
            right.transform.localScale = Vector3.one;
        }

    }
#endif
}
