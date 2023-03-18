using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SplittedSlider : MonoBehaviour
{
    public GameObject btnPrefab;
    public Color btnColor;
    public Color btnDisabledColor;
    public Color btnEmptyColor;

    private List<SplittedSliderButton> buttons = new List<SplittedSliderButton>();

    public bool growToRight = true;
    public bool growToLeft;

    public int maxValue;
    public int value;

    public float separatorWidth;
    public float buttonWidth;
    public float buttonHeight;

    public float radius;

    private RectTransform rt;
   // private Image background;

    public bool build;

    RectTransform parent;

    [HideInInspector]
    public bool dragging;

    public UnityEvent onValueChanged;

    private List<int> empties = new List<int>();

    private void OnValidate()
    {
        if (!growToRight && !growToLeft)
            growToRight = true;

        if(build)
        {
            build = false;
            Fetch();
            Build();
        }

    }

    private void Awake()
    {
        Fetch();
    }

    private IEnumerator Start()
    {
        //Wait UI build
        yield return null;
        yield return null;
        Build();
    }

    private void Fetch()
    {
        Transform go = transform.Find("Buttons");
        if (go != null)
            parent = go.GetComponent<RectTransform>();
        if (parent == null)
        {
            parent = new GameObject().AddComponent<RectTransform>();
            parent.parent = transform;
            parent.name = "Buttons";

        }

        rt = GetComponent<RectTransform>();
        //background = GetComponent<Image>();
        //background.color = GlobalVariables.SurfaceColor;
    }

    public void Build()
    {
        if (growToLeft)
        {
            rt.pivot = new Vector2(1, 0.5f);
            parent.pivot = new Vector2(1, 0.5f);
        }
        else
        {
            rt.pivot = new Vector2(0, 0.5f);
            parent.pivot = new Vector2(0, 0.5f);
        }

        rt.ForceUpdateRectTransforms();
        parent.ForceUpdateRectTransforms();


        //Clear previous buttons
        if (buttons != null)
        {
            foreach (Transform item in parent)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (item != null)
                        DestroyImmediate(item.gameObject);
                };
#endif
#if !UNITY_EDITOR
                    if (item != null)
                        Destroy(item.gameObject);
#endif
            }
        }

        buttons = new List<SplittedSliderButton>();

        float width = buttonWidth * maxValue + separatorWidth * maxValue;

        rt.SetWidth(width);
        rt.SetHeight(buttonHeight);

        rt.ForceUpdateRectTransforms();

        parent.SetWidth(width);
        parent.SetHeight(buttonHeight);

        parent.ForceUpdateRectTransforms();

        parent.SetPositionAndRotation(rt.position, Quaternion.identity);
        parent.localScale = Vector3.one;

        parent.ForceUpdateRectTransforms();

        //Build new ones
        for (int i = 0; i < maxValue; i++)
        {
            SplittedSliderButton newBtn = Instantiate(btnPrefab, parent).AddComponent<SplittedSliderButton>();
            RectTransform newBtnRT = newBtn.GetComponent<RectTransform>();

            if (growToLeft)
                newBtnRT.localPosition = new Vector2(-width + buttonWidth + width / maxValue * i, 0);
            else
                newBtnRT.localPosition = new Vector2(buttonWidth / 2 + width / maxValue * i, 0);


            newBtnRT.SetHeight(buttonHeight);
            newBtnRT.SetWidth(width / maxValue - separatorWidth / 2);
            newBtn.Init(btnColor, btnDisabledColor, btnEmptyColor, i, radius, this);
            buttons.Add(newBtn);
        }

        UpdateButtons(value);
       
    }

    public void UpdateButtons(int value)
    {
        if (buttons.Count == 0)
            return;

        //Update buttons
        for (int i = 0; i < value; i++)
        {
            if (empties.Contains(i))
                buttons[i].SetEmpty();
            else
                buttons[i].SetEnabled();
            buttons[i].SetRadius();
        }

        for (int i = value; i < maxValue; i++)
        {
            buttons[i].SetDisabled();
            buttons[i].SetRadius();
        }
    }

    public void OnClick(int index)
    {
        value = index;
        UpdateButtons(index);
        onValueChanged.Invoke();
    }

    public void Add()
    {
        value++;
        if (value > maxValue)
            value = maxValue;
        OnClick(value);
    }

    public void Sub()
    {
        value--;
        if (value < 0)
            value = 0;
        OnClick(value);
    }

    public void SetEmpties(List<int> emptyList)
    {
        empties = new List<int>(emptyList);
    }

}
