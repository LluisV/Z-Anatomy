using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Linq;
using Assets.Scripts.Commands;

public class LexiconElement : MonoBehaviour
{
    public LexiconItemExpandBtn expandBtn;
    public ExpandCollapseUI expandImg;
    public Image buttonBackground;
    private CameraController cam;
    [HideInInspector]
    public string realName;
    [HideInInspector]
    public int deepLevel;
    [HideInInspector]
    public Transform element;
    [HideInInspector]
    public NameAndDescription nameScript;
    [HideInInspector]
    public int childCount;
    [HideInInspector]
    public bool opened = false;
    [HideInInspector]
    public List<GameObject> childs = new List<GameObject>();
    [HideInInspector]
    public LexiconElement parent;
    [HideInInspector]
    public LexiconItemCheckbox checkBox;
    BodyPart bodyPartScript;
    Visibility isVisibleScript;
    [HideInInspector]
    public Label label;
    private TextMeshProUGUI tmpro;
    private Color defaultColor;
    [HideInInspector]
    public bool isParent;

    private RectTransform rt;

    private GameObject line;

    private void Awake()
    {
        checkBox = GetComponentInChildren<LexiconItemCheckbox>();
        cam = Camera.main.GetComponent<CameraController>();
        tmpro = GetComponentInChildren<TextMeshProUGUI>();
        rt = GetComponent<RectTransform>();
        defaultColor = tmpro.color;
    }

    private void Start()
    {
        nameScript = element.GetComponent<NameAndDescription>();
        bodyPartScript = element.GetComponent<BodyPart>();
        isVisibleScript = element.GetComponent<Visibility>();
        label = element.GetComponent<Label>();
        isParent = IsParent();


        if (element.GetComponentsInChildren<BodyPart>(true).Length == 0 && element.name != GlobalVariables.Instance.bodySections[2].name)
        {
            checkBox.btn.image.sprite = Lexicon.Instance.noCheckboxSprite;
            checkBox.btn.transform.localScale = new Vector2(0.2f, 0.2f);
            checkBox.btn.isEnabled = false;

        }

    }

    private bool IsParent()
    {
        bool hasChilds = false;
        var childs = element.GetComponentsInChildren<Transform>(true);
        foreach (var child in childs)
        {
            if (child == element)
                continue;
            hasChilds = (child.gameObject.IsBodyPart() && !child.CompareTag("Insertions")) || child.gameObject.IsLabel() || child.gameObject.IsGroup();
            if (hasChilds)
                break;
        }

        expandBtn.gameObject.SetActive(hasChilds);
        return hasChilds;
    }

    public void OpenCloseClick()
    {
        opened = !opened;
        if (opened)
            Open();
        else
            Close(false);
    }

    public void Open(bool immediate = false)
    {
        if (!isParent)
            return;

        opened = true;
        if(expandImg.isActiveAndEnabled)
        {
            float tmp = expandImg.durationOfAnimation;
            if (immediate)
                expandImg.durationOfAnimation = 0;
            expandImg.Expand();
            expandImg.durationOfAnimation = tmp;
        }
        Lexicon.Instance.InstatiateChilds(element, GetComponent<RectTransform>());

        SetLine();
    }

    private void SetLine()
    {
        Lexicon.Instance.RecalculateSpaceBtnElements();

        line = new GameObject();
        Image img = line.AddComponent<Image>();
        img.color = new Color(GlobalVariables.SecondaryColor.r, GlobalVariables.SecondaryColor.g, GlobalVariables.SecondaryColor.b, Lexicon.Instance.linesAlpha);

        RectTransform rt = img.GetComponent<RectTransform>();
        rt.SetParent(transform);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(0, 0.5f);

        rt.anchoredPosition = new Vector2(5, -5);
        rt.SetWidth(1);
        rt.SetHeight(childCount * Lexicon.Instance.realSpaceBtwnElements - Lexicon.Instance.realSpaceBtwnElements / 3);

        var lineParent = this.parent;
        var prevParent = this;

        while (lineParent != null)
        {
            if (lineParent.childs.IndexOf(prevParent.gameObject) == lineParent.childs.Count - 1)
            {
                prevParent = lineParent;
                lineParent = lineParent.parent;
                continue;
            }
            var parentRt = lineParent.line.GetComponent<RectTransform>();
            parentRt.SetHeight(parentRt.GetHeight() + childCount * Lexicon.Instance.realSpaceBtwnElements);
            prevParent = lineParent;
            lineParent = lineParent.parent;
        }
    }

    public void Close(bool immediate = false)
    {
        if (!isParent)
            return;

        opened = false;
        float tmp = expandImg.durationOfAnimation;
        if(immediate)
            expandImg.durationOfAnimation = 0;
        expandImg.Collapse();
        expandImg.durationOfAnimation = tmp;
        Lexicon.Instance.CollapseChild(GetComponent<RectTransform>());

        Destroy(line.gameObject);

        var lineParent = this.parent;
        var prevParent = this;

        Lexicon.Instance.RecalculateSpaceBtnElements();

        while (lineParent != null)
        {
            if (lineParent.childs.IndexOf(prevParent.gameObject) == lineParent.childs.Count - 1)
            {
                prevParent = lineParent;
                lineParent = lineParent.parent;
                continue;
            }
            var parentRt = lineParent.line.GetComponent<RectTransform>();
            parentRt.SetHeight(parentRt.GetHeight() - childCount * Lexicon.Instance.realSpaceBtwnElements);
            prevParent = lineParent;
            lineParent = lineParent.parent;
        }
    }

    private void RecursivelyCloseChild(LexiconElement parent)
    {
        foreach (GameObject child in parent.childs)
        {
            LexiconElement script = child.GetComponent<LexiconElement>();
            if (script.childs.Count > 0)
                RecursivelyCloseChild(script);
            expandImg.Collapse();
            buttonBackground.color = new Color(1, 1, 1, 0);
            Lexicon.Instance.CollapseChild(script.GetComponent<RectTransform>());
        }
    }

    public void DisableElement()
    {
        expandBtn.enabled = false;
        checkBox.btn.targetGraphic.color = Color.gray;
        checkBox.btn.isEnabled = false;
        checkBox.btn.Uncheck();
        HideElement();
    }

    public void EnableElement()
    {
        expandBtn.enabled = true;
        checkBox.btn.targetGraphic.color = Color.white;
        checkBox.btn.isEnabled = true;
    }

    public void ShowElement()
    {
        if (element.name == GlobalVariables.Instance.bodySections[2].name)
        {
            MeshManagement.Instance.EnableInsertions();
            return;
        }

        List<GameObject> shown = new List<GameObject>();
        RecursivelySetActiveParents(element, shown);
        RecursiveEnableChild(element, shown);

        RecursivelyCheckChildren(childs);
        RecursivelyCheckParent(parent);
        DisableNotVisible();
        if (label != null && label.line != null)
            label.line.gameObject.SetActive(true);
        ActionControl.Instance.UpdateButtons();
        Lexicon.Instance.UpdateTreeViewCheckboxes();
        if(shown.Count > 0)
            ActionControl.Instance.AddCommand(new ShowCommand(shown), false);
    }

    private void DisableNotVisible()
    {
        foreach (var item in GlobalVariables.Instance.allBodyPartRenderers)
        {
            Visibility script = item.GetComponent<Visibility>();
            if (script != null && !script.isVisible)
                script.gameObject.SetActive(false);
        }
    }

    private void RecursivelySetActiveParents(Transform parent, List<GameObject> shown = null)
    {

        if (parent != null && !parent.CompareTag("GlobalParent"))
        {
            if(!parent.gameObject.activeInHierarchy)
                shown.Add(parent.gameObject);
            parent.gameObject.SetActive(true);
            Visibility script = parent.GetComponent<Visibility>();
            if (script != null)
                script.isVisible = true;
            RecursivelySetActiveParents(parent.parent, shown);
        }
    }

    public void HideElement()
    {
        if (element.name == GlobalVariables.Instance.bodySections[2].name)
        {
            MeshManagement.Instance.DisableInsertions();
            return;
        }
        if (isVisibleScript != null)
            isVisibleScript.isVisible = false;
        if (label != null && label.line != null)
            label.line.gameObject.SetActive(false);
        SelectedObjectsManagement.Instance.DeselectAllChildren(element);
        ActionControl.Instance.UpdateButtons();

        List<GameObject> hidden = new List<GameObject>();
        hidden.AddRange(RecursiveDisableChild(element, new List<GameObject>()));
        RecursivelyUncheckChildren(childs);
        hidden.Add(element.gameObject);
        element.gameObject.SetActive(false);

        ActionControl.Instance.AddCommand(new DeleteCommand(hidden), false);

    }

    public void RecursivelyCheckParent(LexiconElement parent)
    {
        if(parent != null)
        {
            parent.checkBox.btn.Check();
            Visibility isVisibleScript = parent.GetComponent<Visibility>();
            if (isVisibleScript != null)
                isVisibleScript.isVisible = true;
            RecursivelyCheckParent(parent.parent);
        }
    }

    private void RecursivelyCheckChildren(List<GameObject> elements)
    {
        foreach (GameObject child in elements)
        {
            if(child.GetComponent<LexiconElement>().label == null && !child.CompareTag("Insertions"))
            {
                LexiconElement script = child.GetComponent<LexiconElement>();
                Visibility isVisibleScript = child.GetComponent<Visibility>();
                if (script.checkBox.btn.isEnabled)
                    script.checkBox.btn.Check();
                if (isVisibleScript != null)
                    isVisibleScript.isVisible = true;
                if (script.childs.Count > 0)
                    RecursivelyCheckChildren(script.childs);
            }
        }
    }

    public void RecursivelyUncheckChildren(List<GameObject> elements)
    {
        foreach (GameObject child in elements)
        {
            LexiconElement script = child.GetComponent<LexiconElement>();
            if (script.checkBox.btn.isEnabled)
                script.checkBox.btn.Uncheck();
            Visibility isVisibleScript = child.GetComponent<Visibility>();
            if (isVisibleScript != null)
                isVisibleScript.isVisible = false;
            if (script.childs.Count > 0)
                RecursivelyUncheckChildren(script.childs);
        }
    }

    public void ElementClick()
    {
        if (element == null)
            return;

        if (!isVisibleScript.isSelected && !checkBox.btn.isPressed)
        {
            ShowElement();
            checkBox.btn.Check();
        }
        //SELECT IT
        if (!isVisibleScript.isSelected)
        {
            if (!Keyboard.current.leftCtrlKey.isPressed)
            {
                if (label == null)
                    SelectedObjectsManagement.Instance.DeselectAllObjects();
                //Update hierarchy bar
                HierarchyBar.Instance.Set(element.transform);

                if (SearchEngine.onSearch)
                    SearchEngine.Instance.ClearSearch();

                Lexicon.Instance.ExpandRecursively();

            }

            SelectedObjectsManagement.Instance.SelectObject(element.gameObject);

            nameScript.SetDescription();

            //IF IT IS A BODYPART
            if (bodyPartScript != null)
            {

                if (SelectedObjectsManagement.Instance.selectedObjects.Count == 1)
                {
                    cam.SetTarget(element.gameObject);
                    cam.cameraCenter.position = bodyPartScript.center;
                    SelectedObjectsManagement.Instance.lastParentSelected = null;

                }
                ActionControl.Instance.AddCommand(new SelectCommand(SelectedObjectsManagement.Instance.selectedObjects), false);
            }
            //IF IT IS A LABEL
            else if (label != null)
            {
                cam.SetTarget(label.gameObject);
                if (!(SelectedObjectsManagement.Instance.activeObjects.Count == 1 && SelectedObjectsManagement.Instance.activeObjects[0] == label.parent.gameObject))
                {
                    SelectedObjectsManagement.Instance.DeselectAllObjects();
                    SelectedObjectsManagement.Instance.SelectObject(element.gameObject);
                    MeshManagement.Instance.IsolationClick();
                }
                else
                {
                    SelectedObjectsManagement.Instance.DeselectAllChildren(label.parent.transform);
                    SelectedObjectsManagement.Instance.SelectObject(element.gameObject);
                }
                SelectedObjectsManagement.Instance.lastParentSelected = null;
            }
            else
            {
                SelectedObjectsManagement.Instance.SelectAllChildren(element.transform);
                ActionControl.Instance.AddCommand(new SelectCommand(SelectedObjectsManagement.Instance.selectedObjects), false);
                SelectedObjectsManagement.Instance.lastParentSelected = element.transform;
            }

            if (!Shortcuts.Instance.multipleSelectionShortcut.IsPressed() && ActionControl.zoomSelected)
                cam.CenterView(true);
        }
        //DESELECT IT
        else
            Deselect();

        ActionControl.Instance.UpdateButtons();
        TranslateObject.Instance.UpdateSelected();

    }

    public void Deselect()
    {
        SelectedObjectsManagement.Instance.DeselectObject(element.gameObject);
        SelectedObjectsManagement.Instance.DeselectAllChildren(element.transform);
        Lexicon.Instance.SetHighlighted(null);
        if (!Keyboard.current.leftCtrlKey.isPressed && SelectedObjectsManagement.Instance.selectedObjects.Count > 0)
        {
            SelectedObjectsManagement.Instance.DeselectAllObjects();
            HierarchyBar.Instance.Set(element.transform);
            Lexicon.Instance.ExpandRecursively();

            SelectedObjectsManagement.Instance.SelectObject(element.gameObject);
            nameScript.SetDescription();
            Lexicon.Instance.SetHighlighted(this);
            if (bodyPartScript != null)
            {
                cam.SetTarget(element.gameObject);
                cam.cameraCenter.position = bodyPartScript.center;
                cam.UpdateCameraPos(bodyPartScript.distanceToCamera);
            }
        }
        if (label == null)
            ActionControl.Instance.AddCommand(new SelectCommand(SelectedObjectsManagement.Instance.selectedObjects), false);
    }

    public void SetElementAsSelected()
    {
        buttonBackground.color = new Color(.75f, .75f, .75f, 0.02f);
    }

    public void SetElementAsHighlighted()
    {
        tmpro.color = GlobalVariables.HighligthColor;
    }

    public void SetElementAsUnselected()
    {
        buttonBackground.color = new Color(1, 1, 1, 0);
    }

    public void SetDefaultColor()
    {
        tmpro.color = defaultColor;
    }

    public void MouseEnter()
    {
        buttonBackground.color = new Color(.75f, .75f, .75f, 0.01f);
    }

    public void MouseExit()
    {
        if(!isVisibleScript.isSelected)
            buttonBackground.color = new Color(1, 1, 1, 0);
        else
            buttonBackground.color = new Color(.75f, .75f, .75f, 0.02f);
    }

    void RecursiveEnableChild(Transform parent, List<GameObject> shown)
    {
        foreach (Transform child in parent)
        {
            if(!child.name.Contains(".j") && !child.name.Contains(".i") && !child.gameObject.IsLabel() && !child.CompareTag("Insertions"))
            {
                if(!child.gameObject.activeInHierarchy)
                    child.gameObject.SetActive(true);
                shown.Add(child.gameObject);
                Visibility script = child.GetComponent<Visibility>();
                if (script != null)
                    script.isVisible = true;

                if (child.childCount > 0)
                    RecursiveEnableChild(child, shown);
            }
        }
    }

    List<GameObject> RecursiveDisableChild(Transform parent, List<GameObject> hidden)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.activeSelf)
                hidden.Add(child.gameObject);
            child.gameObject.SetActive(false);
            Visibility script = child.GetComponent<Visibility>();
            if (script != null)
                script.isVisible = false;
                
            if (child.childCount > 0)
                RecursiveDisableChild(child, hidden);        
        }
        return hidden;
    }

    public void ShowContextualMenu()
    {
        ContextualMenu.Instance.contextObject = element.gameObject;
        ContextualMenu.Instance.Show();
    }

    public void ExpandRecursively()
    {
        List<LexiconElement> parents = new List<LexiconElement>();
        var parent = this.parent;
        while (parent != null)
        {
            parents.Add(parent);
            parent = parent.parent;
        }
        foreach (var item in parents.OrderBy(it => it.deepLevel))
        {
            if (!item.opened)
                item.Open(true);
        }
        if(!opened)
            Open();
    }
}
