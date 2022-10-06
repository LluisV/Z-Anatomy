using Nobi.UiRoundedCorners;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelsManagement : MonoBehaviour
{
    public static PanelsManagement instance;

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

    public float animationTime;

    public AnimationCurve animationCurve;

    private float initialDescHeight;
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

    public RectTransform optionsRT;
    public RectTransform visibilityOptionsRT;

    private RectTransform top;

    private void Awake()
    {
        instance = this;
    }

    private IEnumerator Start()
    {
        initialYPosCorssSections = crossSectionsOptions.expandedPosition.y;

        CameraController.instance.offset = new Vector3(0.35f, CameraController.instance.offset.y, 0);
        CameraController.instance.ResetCamera();

        initialOptionsHeight = optionsRT.GetHeight();
        scaleOptionsCoroutine = ScaleVerticalCoroutine(optionsRT, 0, initialOptionsHeight - ActionControl.Instance.visibilityOptions.GetHeight());
        StartCoroutine(scaleOptionsCoroutine);

        //Wait 4 frames to UI build
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        descRT = desc.GetComponent<RectTransform>();
        lexRT = lex.GetComponent<RectTransform>();

        initialDescHeight = descRT.GetSize().y;
        initialDescPosY = descRT.anchoredPosition.y;

        StartCoroutine(Rebuild());

        SomeVerticalPanelOpened();

        yield return null;


        Lexicon.Instance.ReturnToOrigin();
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
            lex.Collapse();
            lexOnScreen = false;
            lexTab.Deselect();

            if (descOnScreen)
            {
                ScaleUpFromBottom(descRT, animationTime);
                top = descRT;
            }
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
            desc.Collapse();
            descOnScreen = false;
            descTab.Deselect();

            if(lexOnScreen)
            {
                ScaleUpFromTop(lexRT, animationTime);
                top = lexRT;
            }
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
        rt.SetPivot(new Vector2(0.5f, 0.5f));
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, initialDescPosY);
        rt.SetHeight(initialDescHeight);

    }

    private void MoveToTop(RectTransform rt, float time)
    {
        rt.SetPivot(new Vector2(0.5f, 1f));

        top = rt;
        if (moveToTopCoroutine != null)
            StopCoroutine(moveToTopCoroutine);
        moveToTopCoroutine = TranslateVertical(rt, time, 384);
        StartCoroutine(moveToTopCoroutine);
    }

    private void ScaleUpFromTop(RectTransform rt, float time)
    {
        rt.SetPivot(new Vector2(0.5f, 1));


        if (scaleUpTopCoroutine != null)
            StopCoroutine(scaleUpTopCoroutine);
        scaleUpTopCoroutine = ScaleVerticalCoroutine(rt, time, initialDescHeight);
        StartCoroutine(scaleUpTopCoroutine);
    }

    private void ScaleDownFromTop(RectTransform rt, float time)
    {
        rt.SetPivot(new Vector2(0.5f, 1));

        if (scaleDwnTopCoroutine != null)
            StopCoroutine(scaleDwnTopCoroutine);
        scaleDwnTopCoroutine = ScaleVerticalCoroutine(rt, time, initialDescHeight * 0.5f - 2.5f);
        StartCoroutine(scaleDwnTopCoroutine);
    }

    private void ScaleUpFromBottom(RectTransform rt, float time)
    {
        rt.SetPivot(new Vector2(0.5f, 0));

        if (scaleUpBtmCoroutine != null)
            StopCoroutine(scaleUpBtmCoroutine);
        scaleUpBtmCoroutine = ScaleVerticalCoroutine(rt, time, initialDescHeight);
        StartCoroutine(scaleUpBtmCoroutine);
    }

    private void ScaleDownFromBottom(RectTransform rt, float time)
    {
        rt.SetPivot(new Vector2(0.5f, 0));

        if (scaleDwnBtmCoroutine != null)
            StopCoroutine(scaleDwnBtmCoroutine);
        scaleDwnBtmCoroutine = ScaleVerticalCoroutine(rt, time, initialDescHeight * 0.5f - 2.5f);
        StartCoroutine(scaleDwnBtmCoroutine);
    }


    IEnumerator ScaleVerticalCoroutine(RectTransform rt, float durationOfAnimation, float targetValue)
    {
        float startValue = rt.GetHeight();
        float time = 0;
        var roundCorners = rt.GetComponent<ImageWithRoundedCorners>();

        while (time < durationOfAnimation)
        {
            float t = time / durationOfAnimation;

            rt.SetHeight(Mathf.Lerp(startValue,targetValue, animationCurve.Evaluate(t)));

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
        somePanelOnScreen = lexOnScreen || descOnScreen || settingsOnScreen ||helpOnScreen;

        if (somePanelOnScreen)
            CameraController.instance.offset = new Vector3(0.35f, CameraController.instance.offset.y, 0);
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

        crossSectionsOptions.expandedPosition = new Vector2(crossSectionsOptions.expandedPosition.x, initialYPosCorssSections + 24);
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
}
