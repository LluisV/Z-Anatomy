using Nobi.UiRoundedCorners;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [HideInInspector]
    public TabGroup tabGroup;
    private RectTransform rt;
    private Image background;
    public Image icon;
    private TextMeshProUGUI text;
    private DropShadow shadow;
    private ImageWithIndependentRoundedCorners corners;

    public bool selected;
    public bool pointerClickEnabled = false;

    public UnityEvent onMouseOver;
    public UnityEvent onMouseExit;

    private IEnumerator showCoroutine;
    private IEnumerator setLeftCornerCoroutine;
    private IEnumerator setRightCornerCoroutine;


    private void Awake()
    {
        tabGroup = GetComponentInParent<TabGroup>();
        rt = GetComponent<RectTransform>();
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        shadow = GetComponent<DropShadow>();
        corners = GetComponent<ImageWithIndependentRoundedCorners>();
    }

    private void Start()
    {
        CheckSeletion();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Show();
        tabGroup.SetBarPosition(rt.position, rt.GetHeight() * 0.6f);
        onMouseOver.Invoke();
        tabGroup.TabSelected(this, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
        {
            Hide();
            onMouseExit.Invoke();
            tabGroup.TabSelected(this, false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(pointerClickEnabled)
        {
            selected = !selected;
            CheckSeletion();
        }
    }

    public void CheckSeletion()
    {
        if (selected)
            Select();
        else
            Deselect();
       
    }

    public void Select()
    {
        selected = true;
        background.color = tabGroup.backgroundSelectedColor;
        if(text != null)
        {
            text.color = tabGroup.textSelectedColor;
            text.font = tabGroup.selectedFont;
        }
        if (shadow != null)
            shadow.effectColor = tabGroup.shadowSelectedColor;
        if (icon != null)
            icon.color = tabGroup.textSelectedColor;
        Show();
        tabGroup.TabSelected(this, true);

    }

    public void Deselect()
    {
        selected = false;
        background.color = tabGroup.backgroundDeselectedColor;
        if (text != null)
        {
            text.color = tabGroup.textDeselectedColor;
            text.font = tabGroup.deselectedFont;
        }
        if (shadow != null)
            shadow.effectColor = tabGroup.shadowDeselectedColor;
        if (icon != null)
            icon.color = tabGroup.textDeselectedColor;
        Hide();
        tabGroup.TabSelected(this, false);

    }

    private void Show()
    {
        if (showCoroutine != null)
            StopCoroutine(showCoroutine);
        showCoroutine = SetHeight(tabGroup.unfoldedSize);
        StartCoroutine(showCoroutine);
    }

    private void Hide()
    {
        if (showCoroutine != null)
            StopCoroutine(showCoroutine);
        showCoroutine = SetHeight(tabGroup.foldedSize);
        StartCoroutine(showCoroutine);
    }

    IEnumerator SetHeight(float targetHeight)
    {
        float startHeight = rt.GetHeight();
        float time = 0;
        while (time < tabGroup.mouseOverAnimationTime)
        {
            float t = time / tabGroup.mouseOverAnimationTime;
            rt.SetHeight(Mathf.Lerp(startHeight, targetHeight, tabGroup.positionCurve.Evaluate(t)));
            time += Time.deltaTime;
            yield return null;
        }
        rt.SetHeight(targetHeight);
    }

    public void SetRightCorner()
    {
        if (setRightCornerCoroutine != null)
            StopCoroutine(setRightCornerCoroutine);
        setRightCornerCoroutine = SetCorner(new Vector4(0, 1, 0, 0), tabGroup.cornersRadius);
        StartCoroutine(setRightCornerCoroutine);
    }

    public void RemoveRightCorner()
    {
        if (setRightCornerCoroutine != null)
            StopCoroutine(setRightCornerCoroutine);
        setRightCornerCoroutine = SetCorner(new Vector4(0, 1, 0, 0), 0);
        StartCoroutine(setRightCornerCoroutine);
    }
    public void SetLeftCorner()
    {
        if (setLeftCornerCoroutine != null)
            StopCoroutine(setLeftCornerCoroutine);
        setLeftCornerCoroutine = SetCorner(new Vector4(1, 0, 0, 0), tabGroup.cornersRadius);
        StartCoroutine(setLeftCornerCoroutine);
    }

    public void RemoveLeftCorner()
    {
        if (setLeftCornerCoroutine != null)
            StopCoroutine(setLeftCornerCoroutine);
        setLeftCornerCoroutine = SetCorner(new Vector4(1, 0, 0, 0), 0);
        StartCoroutine(setLeftCornerCoroutine);
    }

    private IEnumerator SetCorner(Vector4 corner, float targetValue)
    {
        bool topLeft = corner == new Vector4(1, 0, 0, 0);
        bool topRight = corner == new Vector4(0, 1, 0, 0);
        bool bottomLeft = corner == new Vector4(0, 0, 1, 0);
        bool bottomRight = corner == new Vector4(0, 0, 0, 1);

        float startValue = 0;

        if (topLeft)
            startValue = corners.r.x;
        else if (topRight)
            startValue = corners.r.y;
        else if (bottomLeft)
            startValue = corners.r.z;
        else if (bottomRight)
            startValue = corners.r.w;

        float time = 0;
        while (time < tabGroup.mouseOverAnimationTime)
        {
            float t = time / tabGroup.mouseOverAnimationTime;

            float newValue = Mathf.Lerp(startValue, targetValue, tabGroup.positionCurve.Evaluate(t));

            if (topLeft)
                corners.r.x = newValue;
            else if (topRight)
                corners.r.y = newValue;
            else if (bottomLeft)
                corners.r.z = newValue;
            else if (bottomRight)
                corners.r.w = newValue;

            corners.Refresh();

            time += Time.deltaTime;
            yield return null;
        }

        if (topLeft)
            corners.r.x = targetValue;
        else if (topRight)
            corners.r.y = targetValue;
        else if (bottomLeft)
            corners.r.z = targetValue;
        else if (bottomRight)
            corners.r.w = targetValue;

        corners.Refresh();

    }
}
