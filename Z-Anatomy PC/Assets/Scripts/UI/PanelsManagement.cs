using Nobi.UiRoundedCorners;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PanelsManagement : MonoBehaviour
{
    public static PanelsManagement instance;

    public GridLayoutGroup grid;
    private RectTransform gridRT;

    public ExpandCollapseUI desc;
    public ExpandCollapseUI lex;
    public ExpandCollapseUI settings;
    public ExpandCollapseUI help;
    public ExpandCollapseUI pencilOptions;
    public ExpandCollapseUI crossSectionsOptions;
    public GameObject colorPicker;

    public Tab descTab;
    public Tab lexTab;
    public Tab helpTab;
    public Tab settingsTab;

    private RectTransform descRT;
    private RectTransform lexRT;
    private RectTransform helpRT;
    private RectTransform settingsRT;

    public RectTransform undoRedoPanel;
    public RectTransform toolsPanel;
    public RectTransform searchPanel;
    public RectTransform tabsPanel;
    public RectTransform horizontalHierarchy;

    public float animationTime;

    public AnimationCurve animationCurve;

    private float initialDescPosX;
    private float initialDescPosY;
    private float initialOptionsHeight;
    private float initialYPosCorssSections;

    [HideInInspector]
    public bool descOnScreen = false;
    [HideInInspector]
    public bool lexOnScreen = false;
    [HideInInspector]
    public bool settingsOnScreen = false;
    [HideInInspector]
    public bool helpOnScreen = false;
    [HideInInspector]
    public bool pencilOptionsOnScreen = false;
    [HideInInspector]
    public bool colorPickerOnScreen = false;

    private bool somePanelOnScreen = true;

    private bool lexWasOpen;
    private bool descWasOpen;

    private IEnumerator moveToTopCoroutine;
    private IEnumerator scaleUpTopCoroutine;
    private IEnumerator scaleUpBtmCoroutine;
    private IEnumerator scaleDwnTopCoroutine;
    private IEnumerator scaleDwnBtmCoroutine;

    private IEnumerator scaleOptionsCoroutine;

    private IEnumerator waitLMBRelease;

    public RectTransform optionsRT;
    public RectTransform visibilityOptionsRT;

    private RectTransform top;

    public RectTransform[] panels;
    public RectTransform[] overlays;
    public RectTransform optionsPanel;

    private float panelWidth;
    public float minPanelWidth;
    public float maxPanelWidth;
    public float minHierarchyWidth;


    private void Awake()
    {

        instance = this;

        descRT = desc.GetComponent<RectTransform>();
        lexRT = lex.GetComponent<RectTransform>();
        helpRT = help.GetComponent<RectTransform>();
        settingsRT = settings.GetComponent<RectTransform>();
        gridRT = grid.GetComponent<RectTransform>();
        panelWidth = descRT.GetWidth();

    }

    private IEnumerator Start()
    {
        initialYPosCorssSections = crossSectionsOptions.expandedPosition.y;

        CameraController.instance.offset = new Vector3(2*panelWidth / Screen.width, CameraController.instance.offset.y, 0);
        CameraController.instance.ResetCamera();

        initialOptionsHeight = optionsRT.GetHeight();
        scaleOptionsCoroutine = ScaleVerticalCoroutine(optionsRT, 0, initialOptionsHeight - ActionControl.Instance.visibilityOptions.GetHeight());
        StartCoroutine(scaleOptionsCoroutine);

        //Wait 4 frames to UI build 
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        initialDescPosX = descRT.anchoredPosition.x;
        initialDescPosY = descRT.anchoredPosition.y;

        StartCoroutine(Rebuild());

        SomeVerticalPanelOpened();

        yield return null;

        Lexicon.Instance.ReturnToOrigin();

        yield return null;

        UpdateHierarchySize();

    }

    public void ShowPencilOptions()
    {
        pencilOptionsOnScreen = !pencilOptionsOnScreen;
        if (pencilOptionsOnScreen)
            pencilOptions.Expand();
        else
            pencilOptions.Collapse();

    }

    public void ShowColorPicker()
    {
        colorPickerOnScreen = !colorPickerOnScreen;
        colorPicker.SetActive(colorPickerOnScreen);
    }

    public void ShowLexicon()
    {
        //Collapse lex
        if (lex.isExpanded)
        {
            if (descOnScreen)
            {
                //Release grid
                ReleaseLexDesc();
                //Scale up desc
                ScaleUpFromBottom(descRT, animationTime);
                top = descRT;
            }

            lex.Collapse();
            lexOnScreen = false;
            lexTab.Deselect();
        }
        //Show lex
        else
        {
            ResetPanel(lexRT);

            lexOnScreen = true;
            lexTab.Select();

            if (descOnScreen)
            {
                ScaleDownFromTop(lexRT, 0);
                StartCoroutine(WaitExpanding());
                IEnumerator WaitExpanding()
                {
                    yield return new WaitWhile(() => desc.expanding);
                    ScaleDownFromBottom(descRT, animationTime);

                    yield return new WaitForSeconds(animationTime);
                    //In grid
                    lex.transform.SetParent(grid.transform, true);
                    desc.transform.SetParent(grid.transform, true);
                    lex.transform.SetSiblingIndex(0);
                    desc.transform.SetSiblingIndex(1);
                }
            }

            top = lexRT;
            lex.Expand();
        }

        if (settings.isExpanded)
        {
            settings.Collapse();
            settingsTab.Deselect();
            settingsOnScreen = false;
        }
        if (help.isExpanded)
        {
            help.Collapse();
            helpTab.Deselect();
            helpOnScreen = false;
        }

        SomeVerticalPanelOpened();
    }

    public void ShowDescription()
    {
        //Collapse desc
        if (desc.isExpanded)
        {

            if (lexOnScreen)
            {
                //Release grid
                ReleaseLexDesc();
                //Scale up lex
                ScaleUpFromTop(lexRT, animationTime);
                top = lexRT;
            }

            desc.Collapse();
            descOnScreen = false;
            descTab.Deselect();
        }
        //Show desc
        else
        {
            ResetPanel(descRT);

            descOnScreen = true;
            descTab.Select();

            if(lexOnScreen)
            {
                top = lexRT;
                ScaleDownFromBottom(descRT, 0);
                StartCoroutine(WaitExpanding());
                IEnumerator WaitExpanding(){
                    yield return new WaitWhile(() => lex.expanding);
                    ScaleDownFromTop(lexRT, animationTime);

                    yield return new WaitForSeconds(animationTime);

                    //In grid
                    lex.transform.SetParent(grid.transform, true);
                    desc.transform.SetParent(grid.transform, true);
                    lex.transform.SetSiblingIndex(0);
                    desc.transform.SetSiblingIndex(1);
                }
            }

            desc.Expand();
        }

        if (settings.isExpanded)
        {
            settings.Collapse();
            settingsTab.Deselect();
            settingsOnScreen = false;
        }
        if (help.isExpanded)
        {
            help.Collapse();
            helpTab.Deselect();
            helpOnScreen = false;
        }

        SomeVerticalPanelOpened();
    }

    private void ReleaseLexDesc()
    {

        lex.transform.SetParent(grid.transform.parent, true);
        desc.transform.SetParent(grid.transform.parent, true);
        lexRT.anchorMin = new Vector2(.5f, 0);
        lexRT.anchorMax = new Vector2(.5f, 1);
        descRT.anchorMin = new Vector2(.5f, 0);
        descRT.anchorMax = new Vector2(.5f, 1);
        lexRT.anchoredPosition = new Vector2(initialDescPosX, lexRT.anchoredPosition.y);
        descRT.anchoredPosition = new Vector2(initialDescPosX, descRT.anchoredPosition.y);
        lexRT.SetTop(-3.5f);
        lexRT.SetBottom(gridRT.rect.height / 2 + grid.spacing.y / 2);
        descRT.SetTop(gridRT.rect.height / 2 + grid.spacing.y / 2);
        descRT.SetBottom(-3.5f);
    }

    public void ShowSettings()
    {
        settingsOnScreen = !settingsOnScreen;
        settings.ExpandButtonClick();

        if (settingsOnScreen)
            settingsTab.Select();
        else
            settingsTab.Deselect();

        if(settingsOnScreen)
        {
            if (descOnScreen && lexOnScreen)
                ReleaseLexDesc();
            if (help.isExpanded)
            {
                help.Collapse();
                helpOnScreen = false;
                helpTab.Deselect();

            }
            if (desc.isExpanded)
            {
                desc.Collapse();
                descWasOpen = true;
                descOnScreen = false;
            }
            else
                descWasOpen = false;

            if (lex.isExpanded)
            {
                lex.Collapse();
                lexWasOpen = true;
                lexOnScreen = false;
            }
            else
                lexWasOpen = false;

            descTab.Deselect();
            lexTab.Deselect();
        }
        else
        {
            if(lexWasOpen && top == lexRT)
            {
                ShowLexicon();
                if (descWasOpen)
                    StartCoroutine(ShowSecond(descRT));
            }
            else if(descWasOpen && top == descRT)
            {
                ShowDescription();
                if (lexWasOpen)
                    StartCoroutine(ShowSecond(lexRT));
            }



            IEnumerator ShowSecond(RectTransform rt)
            {
                yield return new WaitForSeconds(animationTime);
                if (rt == lexRT)
                    ShowLexicon();
                else if (rt == descRT)
                    ShowDescription();
            }

        }

        if (help.isExpanded)
        {
            help.Collapse();
            helpTab.Deselect();
            helpOnScreen = false;
        }

        SomeVerticalPanelOpened();
    }

    public void ShowHelp()
    {
        helpOnScreen = !helpOnScreen;
        help.ExpandButtonClick();

        if (helpOnScreen)
            helpTab.Select();
        else
            helpTab.Deselect();

        if (helpOnScreen)
        {
            if (descOnScreen && lexOnScreen)
                ReleaseLexDesc();
            if(settings.isExpanded)
            {
                settings.Collapse();
                settingsOnScreen = false;
                settingsTab.Deselect();
            }
            if (desc.isExpanded)
            {
                desc.Collapse();
                descWasOpen = true;
                descOnScreen = false;
            }
            else
                descWasOpen = false;

            if (lex.isExpanded)
            {
                lex.Collapse();
                lexWasOpen = true;
                lexOnScreen = false;
            }
            else
                lexWasOpen = false;

            descTab.Deselect();
            lexTab.Deselect();

        }
        else
        {
            if (lexWasOpen && top == lexRT)
            {
                ShowLexicon();
                if (descWasOpen)
                    StartCoroutine(ShowSecond(descRT));

            }
            else if (descWasOpen && top == descRT)
            {
                ShowDescription();
                if (lexWasOpen)
                    StartCoroutine(ShowSecond(lexRT));
            }

            IEnumerator ShowSecond(RectTransform rt)
            {
                yield return new WaitForSeconds(animationTime);
                if (rt == lexRT)
                    ShowLexicon();
                else if (rt == descRT)
                    ShowDescription();
            }
        }

        if (settings.isExpanded)
        {
            settings.Collapse();
            settingsTab.Deselect();
            settingsOnScreen = false;
        }

        SomeVerticalPanelOpened();
    }

    private void ResetPanel(RectTransform rt)
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, initialDescPosY);
        rt.SetTop(0);
        rt.SetBottom(0);
    }

    private void MoveToTop(RectTransform rt, float time)
    {
        //not used
        top = rt;
        if (moveToTopCoroutine != null)
            StopCoroutine(moveToTopCoroutine);
        moveToTopCoroutine = TranslateVertical(rt, time, -1);
        StartCoroutine(moveToTopCoroutine);
    }

    private void ScaleUpFromTop(RectTransform rt, float time)
    {

        if (scaleUpTopCoroutine != null)
            StopCoroutine(scaleUpTopCoroutine);
        scaleUpTopCoroutine = ScaleVerticalCoroutine(rt, time, -3.5f, false);
        StartCoroutine(scaleUpTopCoroutine);
    }

    private void ScaleDownFromTop(RectTransform rt, float time)
    {

        if (scaleDwnTopCoroutine != null)
            StopCoroutine(scaleDwnTopCoroutine);
        scaleDwnTopCoroutine = ScaleVerticalCoroutine(rt, time, gridRT.rect.height / 2, false);
        StartCoroutine(scaleDwnTopCoroutine);
    }

    private void ScaleUpFromBottom(RectTransform rt, float time)
    {

        if (scaleUpBtmCoroutine != null)
            StopCoroutine(scaleUpBtmCoroutine);
        scaleUpBtmCoroutine = ScaleVerticalCoroutine(rt, time, -3.5f, true);
        StartCoroutine(scaleUpBtmCoroutine);
    }

    private void ScaleDownFromBottom(RectTransform rt, float time)
    {

        if (scaleDwnBtmCoroutine != null)
            StopCoroutine(scaleDwnBtmCoroutine);
        scaleDwnBtmCoroutine = ScaleVerticalCoroutine(rt, time, gridRT.rect.height / 2, true);
        StartCoroutine(scaleDwnBtmCoroutine);
    }


    IEnumerator ScaleVerticalCoroutine(RectTransform rt, float durationOfAnimation, float targetValue, bool top)
    {
        float startValue;
        if (top)
            startValue = rt.GetTop();
        else
            startValue = rt.GetBottom();
        float time = 0;
        var roundCorners = rt.GetComponent<ImageWithRoundedCorners>();

        while (time < durationOfAnimation)
        {
            float t = time / durationOfAnimation;

            if(top)
                rt.SetTop(Mathf.Lerp(startValue, targetValue, animationCurve.Evaluate(t)));
            else
                rt.SetBottom(Mathf.Lerp(startValue, targetValue, animationCurve.Evaluate(t)));

            time += Time.deltaTime;
            yield return null;

        }

        if (top)
            rt.SetTop(targetValue);
        else
            rt.SetBottom(targetValue);

        if (roundCorners != null)
        {
            roundCorners.Refresh();
            roundCorners.enabled = false;
            roundCorners.enabled = true;
        }
    }

    IEnumerator ScaleVerticalCoroutine(RectTransform rt, float durationOfAnimation, float targetValue)
    {
        float startValue = rt.GetHeight();
        float time = 0;
        var roundCorners = rt.GetComponent<ImageWithRoundedCorners>();

        while (time < durationOfAnimation)
        {
            float t = time / durationOfAnimation;

            rt.SetHeight(Mathf.Lerp(startValue, targetValue, animationCurve.Evaluate(t)));

            time += Time.deltaTime;
            yield return null;

        }

        rt.SetHeight(targetValue);
        if (roundCorners != null)
        {
            roundCorners.Refresh();
            roundCorners.enabled = false;
            roundCorners.enabled = true;
        }
    }

    IEnumerator TranslateVertical(RectTransform rt, float durationOfAnimation, float targetValue)
    {
        yield return null;

        float time = 0;
        Vector3 startValue = rt.anchoredPosition;
        Vector3 targetPosition = new Vector3(rt.anchoredPosition.x, targetValue);
        while (time < durationOfAnimation)
        {
            float t = time / durationOfAnimation;

            rt.anchoredPosition = Vector3.Lerp(startValue, targetPosition, animationCurve.Evaluate(t));
            time += Time.deltaTime;
            yield return null;
        }
        rt.anchoredPosition = targetPosition;
    }

    public bool SomeVerticalPanelOpened()
    {
        somePanelOnScreen = lexOnScreen || descOnScreen || settingsOnScreen || helpOnScreen;

        if (somePanelOnScreen)
            CameraController.instance.offset = new Vector3(panelWidth / Screen.width, CameraController.instance.offset.y, 0);
        else
            CameraController.instance.offset = new Vector3(0, CameraController.instance.offset.y, 0);

        CameraController.instance.LerpHorizontal(animationTime);

        return somePanelOnScreen;
    }

    public void ExpandOptionsPanel()
    {
        if (scaleOptionsCoroutine != null)
            StopCoroutine(scaleOptionsCoroutine);

        scaleOptionsCoroutine = ScaleVerticalCoroutine(optionsRT, animationTime, initialOptionsHeight);
        StartCoroutine(scaleOptionsCoroutine);

        crossSectionsOptions.expandedPosition = new Vector2(crossSectionsOptions.expandedPosition.x, initialYPosCorssSections);
        if(CrossPlanesGizmo.Instance.opened)
            crossSectionsOptions.Expand();

        visibilityOptionsRT.gameObject.SetActive(true);
    }

    public void CollapseOptionsPanel()
    {
        if (scaleOptionsCoroutine != null)
            StopCoroutine(scaleOptionsCoroutine);

        scaleOptionsCoroutine = ScaleVerticalCoroutine(optionsRT, animationTime, initialOptionsHeight - ActionControl.Instance.visibilityOptions.GetHeight());
        StartCoroutine(scaleOptionsCoroutine);

        crossSectionsOptions.expandedPosition = new Vector2(crossSectionsOptions.expandedPosition.x, initialYPosCorssSections + 40);
        if (CrossPlanesGizmo.Instance.opened)
            crossSectionsOptions.Expand();

        visibilityOptionsRT.gameObject.SetActive(false);

    }

    public void Reset()
    {
        StartCoroutine(Rebuild());
    }

    IEnumerator Rebuild()
    {
        float prev = animationTime;
        animationTime = 0;
        lex.durationOfAnimation = 0;
        desc.durationOfAnimation = 0;

        if(!lexOnScreen)
            ShowLexicon();

        yield return new WaitForEndOfFrame();

        if (!descOnScreen)
            ShowDescription();

        yield return new WaitForEndOfFrame();

        Lexicon.Instance.ReturnToOrigin();

        lex.durationOfAnimation = prev;
        desc.durationOfAnimation = prev;
        animationTime = prev;

        top = lexRT;

    }

    public void ResizePanels(float deltaX)
    {
        if(panelWidth + deltaX >= minPanelWidth && panelWidth + deltaX <= maxPanelWidth && horizontalHierarchy.GetWidth() - deltaX >= minHierarchyWidth)
        {
            UpdatePanelsWidth(deltaX);
            UpdateHierarchySize();

            if (waitLMBRelease != null)
                StopCoroutine(waitLMBRelease);
            waitLMBRelease = AwaitUpdateHierarchy();
            StartCoroutine(waitLMBRelease);
        }
    }

    IEnumerator AwaitUpdateHierarchy()
    {
        yield return new WaitWhile(() => Mouse.current.leftButton.isPressed);
        HierarchyBar.Instance.Set();
    }

    public void CollpasePanels(float deltaX)
    {
        //If the horizontal hierarchy bar is at its minimum
        float dif = Mathf.Abs(horizontalHierarchy.GetWidth() - minHierarchyWidth);
        if (dif < 1 && deltaX < 0)
        {
            //If the deltaX exceeds the limit, reduce it to fit the minimum width
            while (panelWidth + deltaX < minPanelWidth && deltaX < 0)
                deltaX++;

            UpdatePanelsWidth(deltaX);
        }
    }

    private void UpdatePanelsWidth(float deltaX)
    {
        panelWidth += deltaX;

        toolsPanel.Translate(Vector3.left * deltaX);
        gridRT.SetWidth(panelWidth);
        descRT.SetWidth(panelWidth);
        lexRT.SetWidth(panelWidth);
        settingsRT.SetWidth(panelWidth);
        helpRT.SetWidth(panelWidth);
        searchPanel.SetWidth(panelWidth);
        tabsPanel.SetWidth(panelWidth);
        if (somePanelOnScreen)
            CameraController.instance.offset = new Vector3(2 * panelWidth / Screen.width, CameraController.instance.offset.y, 0);
    }

    //It updates its size but not its content!
    //To update its content call HierarchyBar.Instance.Set();
    public void UpdateHierarchySize()
    {
        StartCoroutine(waitForChange());
        IEnumerator waitForChange()
        {
            yield return null;
            if (gridRT != null)
                grid.cellSize = new Vector2(panelWidth, gridRT.GetHeight() / 2);

            horizontalHierarchy.SetWidth(toolsPanel.position.x - horizontalHierarchy.position.x - 10);
            if (horizontalHierarchy.GetWidth() < minHierarchyWidth)
                horizontalHierarchy.SetWidth(minHierarchyWidth);
        }

    }
}
