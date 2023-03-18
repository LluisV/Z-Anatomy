using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class ExpandCollapseUI : MonoBehaviour
{

    [Header("")]
    public Vector2 collapasedPosition;
    public Vector2 expandedPosition;
    public float durationOfAnimation;


    [Header("Movement curves")]
    public AnimationCurve positionCurve;
    public AnimationCurve rotationCurve;

    [Header("Add rotation animation")]
    public RectTransform buttonToAnimate;
    public float angle;

    [Header("Define the actual state")]
    public bool isExpanded;

    [Header("Movement allowed")]
    public bool vertical;
    public bool horizontal;

    [Header("Save expanded position")]
    public bool saveVertical = false;
    public bool saveHorizontal = false;

    [Header("Expenad when the mouse is near")]
    public bool expandOnMouseNear = false;
    public float minXmouseDist = 20f;
    public float minYmouseDist = 20f;
    public float maxXmouseDist = 20f;
    public float maxYmouseDist = 20f;
    public ExpandCollapseUI[] collapseOnExpand;

    public RectTransform mouseObjective;

    [Header("Disable this gameobject when it's collapsed")]
    public bool disbleGameobjectOnCollapse;

    [Header("Disable gameObject when it's expanded")]
    public GameObject go;

    [Header("Disable image when it's collapsed")]
    public bool disbleOnCollapse;
    public Image imageToDisable;
    private RectTransform trans;
    private IEnumerator expandCoroutine;
    private IEnumerator collapseCoroutine;
    private float initialAngle;

    [HideInInspector]
    public bool expanding;
    [HideInInspector]
    public bool collapsing;

    private void Awake()
    {
        trans = GetComponent<RectTransform>();
        if(disbleOnCollapse)
            imageToDisable.enabled = disbleOnCollapse && isExpanded;
        if(buttonToAnimate != null)
            initialAngle = buttonToAnimate.eulerAngles.z;
        expandCoroutine = ExpandCoroutine();
        collapseCoroutine = CollapseCoroutine();
        if (mouseObjective == null)
            mouseObjective = trans;

        //Distances are proportional to screen size
    }


    private void Update()
    {
        if (!expandOnMouseNear)
            return;    

        if(!isExpanded && Mathf.Abs(mouseObjective.position.x - Mouse.current.position.ReadValue().x) < minXmouseDist * Screen.width / 1920 
            && Mathf.Abs(mouseObjective.position.y - Mouse.current.position.ReadValue().y) < minYmouseDist * Screen.width / 1920)
        {
            ExpandButtonClick();
            foreach (var item in collapseOnExpand)
                item.Collapse();
        }
        else if(isExpanded && (Mathf.Abs(mouseObjective.position.x - Mouse.current.position.ReadValue().x) > maxXmouseDist * Screen.width / 1920 
            || Mathf.Abs(mouseObjective.position.y - Mouse.current.position.ReadValue().y) > maxYmouseDist * Screen.width / 1920))
            Collapse();
    }

    public void ExpandButtonClick()
    {
        gameObject.SetActive(true);
        if (isExpanded)
        {
            if (expandCoroutine != null)
                StopCoroutine(expandCoroutine);
            collapseCoroutine = CollapseCoroutine();
            StartCoroutine(collapseCoroutine);
            if (buttonToAnimate != null)
                StartCoroutine(RotateIconCoroutine(initialAngle + angle));
        }
        else
        {
            if (collapseCoroutine != null)
                StopCoroutine(collapseCoroutine);
            expandCoroutine = ExpandCoroutine();
            StartCoroutine(expandCoroutine);
            if (buttonToAnimate != null)
                StartCoroutine(RotateIconCoroutine(initialAngle));
        }
    }

    IEnumerator CollapseCoroutine()
    {
        collapsing = true;
        if (saveHorizontal)
            expandedPosition.x = trans.anchoredPosition.x;
        if (saveVertical)
            expandedPosition.y = trans.anchoredPosition.y;


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
            trans.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, positionCurve.Evaluate(t));
            time += Time.deltaTime;
            yield return null;
        }
        trans.anchoredPosition = targetPosition;
        if (disbleOnCollapse)
            imageToDisable.enabled = false;
        if (disbleGameobjectOnCollapse)
            base.gameObject.SetActive(false);
        if (go != null)
            go.SetActive(false);
        isExpanded = false;
        collapsing = false;
    }

    IEnumerator ExpandCoroutine()
    {
        expanding = true;
        isExpanded = true;
        if (disbleOnCollapse)
            imageToDisable.enabled = true;
        if (go != null)
            go.SetActive(true);
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
          //  t = t * t * t * (t * (6f * t - 15f) + 10f);
            trans.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, positionCurve.Evaluate(t));
            time += Time.deltaTime;
            yield return null;
        }
        trans.anchoredPosition = targetPosition;
        expanding = false;
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

    public void SetCollapsedPositionX(int newX)
    {
        collapasedPosition.x = newX;
    }

    public void Collapse()
    {
        if (isActiveAndEnabled)
        {
            if (expandCoroutine != null)
                StopCoroutine(expandCoroutine);
            collapseCoroutine = CollapseCoroutine();
            StartCoroutine(collapseCoroutine);
        }
        if (this.isActiveAndEnabled && buttonToAnimate != null)
            StartCoroutine(RotateIconCoroutine(initialAngle));
    }

    public void Expand()
    {
        if(isActiveAndEnabled)
        {
            if (collapseCoroutine != null)
                StopCoroutine(collapseCoroutine);
            expandCoroutine = ExpandCoroutine();
            StartCoroutine(expandCoroutine);
        }
        if (this.isActiveAndEnabled && buttonToAnimate != null)
            StartCoroutine(RotateIconCoroutine(initialAngle + angle));
    }
}
