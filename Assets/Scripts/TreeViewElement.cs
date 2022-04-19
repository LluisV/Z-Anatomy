using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Diagnostics;

public class TreeViewElement : MonoBehaviour
{
    public Sprite openSprite;
    public Sprite closeSprite;
    public Button expandBtn;
    public Image buttonImage;
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
    private bool selected = false;
    [HideInInspector]
    public List<GameObject> childs = new List<GameObject>();
    [HideInInspector]
    public TreeViewElement parent;
    [HideInInspector]
    public ButtonSpriteSwap checkBox;
    [HideInInspector]
    public bool isParent = true;

    private void Awake()
    {
        checkBox = GetComponentInChildren<ButtonSpriteSwap>();
        cam = Camera.main.GetComponent<CameraController>();
    }

    private void Start()
    {
        nameScript = element.GetComponent<NameAndDescription>();
    }

    public void OpenCloseClick()
    {
        opened = !opened;
        if (opened)
            Open();
        else
            Close();
    }

    private void Open()
    {
        buttonImage.sprite = closeSprite;
        TreeViewCanvas.instance.InstatiateChilds(element, GetComponent<RectTransform>());
    }

    private void Close()
    {
        buttonImage.sprite = openSprite;
        buttonBackground.color = new Color(1, 1, 1, 0);
        TreeViewCanvas.instance.CollapseChild(GetComponent<RectTransform>());
    //    RecursivelyCloseChild(this);
    }

    private void RecursivelyCloseChild(TreeViewElement parent)
    {
        foreach (GameObject child in parent.childs)
        {
            TreeViewElement script = child.GetComponent<TreeViewElement>();
            if (script.childs.Count > 0)
                RecursivelyCloseChild(script);
            buttonImage.sprite = openSprite;
            buttonBackground.color = new Color(1, 1, 1, 0);
            TreeViewCanvas.instance.CollapseChild(script.GetComponent<RectTransform>());
        }
    }

    public void DisableElement()
    {
        expandBtn.enabled = false;
        checkBox.GetComponent<Image>().color = Color.gray;
        checkBox.isEnabled = false;
        checkBox.Uncheck();
        HideElement();
    }

    public void EnableElement()
    {
        expandBtn.enabled = true;
        checkBox.GetComponent<Image>().color = Color.white;
        checkBox.isEnabled = true;
    }

    public void ShowElement()
    {
        Transform found = RecursiveFindChild(MeshManagement.instance.globalParent.transform, realName);
        if (found.CompareTag("Fascia"))
            TreeViewCanvas.instance.EnableDisableFascias(true);
        RecursivelySetActiveParents(found);
        RecursiveEnableChild(found);
        RecursivelyCheckChildren(childs);
        RecursivelyCheckParent(parent);
        DisableNotVisible();
        if (found.CompareTag("Skeleton"))
        {
            Transform insertions = transform.parent.Find(MeshManagement.instance.bodySections[1].name);
            if(insertions != null)
                insertions.GetComponent<TreeViewElement>().EnableElement();
        }
        MeshManagement.instance.RefreshReflections();
        SelectedObjectsManagement.instance.EnableDisableButtons();
    }

    private void DisableNotVisible()
    {
        foreach (var item in MeshManagement.instance.bodyPartsRenderers)
        {
            IsVisible script = item.GetComponent<IsVisible>();
            if (script != null && !script.isVisible)
                script.gameObject.SetActive(false);
        }
    }

    private void RecursivelySetActiveParents(Transform parent)
    {
        if(parent != null && !parent.CompareTag("GlobalParent"))
        {
            parent.gameObject.SetActive(true);
            IsVisible script = parent.GetComponent<IsVisible>();
            if (script != null)
                script.isVisible = true;
            RecursivelySetActiveParents(parent.parent);
        }
    }

    public void HideElement()
    {
        Transform found = RecursiveFindChild(MeshManagement.instance.globalParent.transform, realName);
        found.gameObject.SetActive(false);
        if (found.CompareTag("Fascia"))
            TreeViewCanvas.instance.EnableDisableFascias(false);
        IsVisible script = found.GetComponent<IsVisible>();
        if (script != null)
            script.isVisible = false;
        SelectedObjectsManagement.instance.DeselectAllChildren(found);
        SelectedObjectsManagement.instance.EnableDisableButtons();
        RecursiveDisableChild(found);
        RecursivelyUncheckChildren(childs);
        if(nameScript != null && nameScript.allNames.Length > 0 && nameScript.allNames[0].Equals("skeletal system"))
        {
            TreeViewElement insertions = transform.parent.Find(MeshManagement.instance.bodySections[1].name).GetComponent<TreeViewElement>();
            if (insertions != null)
                insertions.DisableElement();
        }
        MeshManagement.instance.RefreshReflections();
    }

    private void RecursivelyCheckParent(TreeViewElement parent)
    {
        if(parent != null)
        {
            parent.checkBox.Check();
            IsVisible isVisibleScript = parent.GetComponent<IsVisible>();
            if (isVisibleScript != null)
                isVisibleScript.isVisible = true;
            RecursivelyCheckParent(parent.parent);
        }
    }

    private void RecursivelyCheckChildren(List<GameObject> elements)
    {
        foreach (GameObject child in elements)
        {
            if(!child.name.Contains(".t"))
            {
                TreeViewElement script = child.GetComponent<TreeViewElement>();
                IsVisible isVisibleScript = child.GetComponent<IsVisible>();
                if (script.checkBox.isEnabled)
                    script.checkBox.Check();
                if (isVisibleScript != null)
                    isVisibleScript.isVisible = true;
                if (script.childs.Count > 0)
                    RecursivelyCheckChildren(script.childs);
            }
        }
    }

    private void RecursivelyUncheckChildren(List<GameObject> elements)
    {
        foreach (GameObject child in elements)
        {
            TreeViewElement script = child.GetComponent<TreeViewElement>();
            if (script.checkBox.isEnabled)
                script.checkBox.Uncheck();
            IsVisible isVisibleScript = child.GetComponent<IsVisible>();
            if (isVisibleScript != null)
                isVisibleScript.isVisible = false;
            if (script.childs.Count > 0)
                RecursivelyUncheckChildren(script.childs);
        }
    }

    public void ElementSelected()
    {
        selected = !selected;
        if (selected && !checkBox.isPressed)
        {
            ShowElement();
            checkBox.Check();
        }
        Transform found = RecursiveFindChild(MeshManagement.instance.globalParent.transform, realName);
        if (found == null)
            return;
        GameObject goFound = found.gameObject;
        BodyPart bodyPartScript = goFound.GetComponent<BodyPart>();
        NameAndDescription nameScript = goFound.GetComponent<NameAndDescription>();
        //SELECT IT
        if (selected)
        {
            //Reset visited objects stack
            NamesManagement.instance.ResetStack();

            buttonBackground.color = new Color(1, 1, 1, 0.025f);
            NamesManagement.instance.SetTitle(goFound.name);
            NamesManagement.instance.SetDescription(nameScript.Description(), goFound);
            ActionControl.someObjectSelected = true;
            SelectedObjectsManagement.instance.SelectObject(goFound);
            SelectedObjectsManagement.instance.EnableDisableButtons();
            //IF IT IS A BODYPART
            if (bodyPartScript != null)
            {
                if (SelectedObjectsManagement.instance.selectedObjects.Count == 1)
                {
                    cam.target = found.gameObject;
                    cam.cameraCenter = bodyPartScript.center;
                    cam.UpdateDistance(bodyPartScript.distanceToCamera);
                }

            }
            //IF IT IS A GROUP
            else
            {
                //  selectionManagement.SelectObject(goFound);
                SelectedObjectsManagement.instance.SelectAllChildren(goFound.transform);
                SelectedObjectsManagement.instance.EnableDisableButtons();
            }
        }
        //DESELECT IT
        else
        {
            SelectedObjectsManagement.instance.DeselectObjet(goFound);
            SelectedObjectsManagement.instance.EnableDisableButtons();
            SelectedObjectsManagement.instance.DeselectAllChildren(goFound.transform);
            buttonBackground.color = new Color(1, 1, 1, 0);
        }
    }

    void RecursiveEnableChild(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if(!child.name.Contains("-lin") && child.GetComponent<Label>() == null)
            {
                child.gameObject.SetActive(true);
                IsVisible script = child.GetComponent<IsVisible>();
                if (script != null)
                    script.isVisible = true;

                if (child.childCount > 0)
                    RecursiveEnableChild(child);
            }
        }
    }

    void RecursiveDisableChild(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (!child.name.Contains("-lin"))
            {
                child.gameObject.SetActive(false);
                IsVisible script = child.GetComponent<IsVisible>();
                if (script != null)
                    script.isVisible = false;
                
                if (child.childCount > 0)
                    RecursiveDisableChild(child);
            }
        }
    }

    Transform RecursiveFindChild(Transform parent, string childName)
    {
        childName = childName.ToLower();
        foreach (Transform child in parent)
        {
            string childN = child.name.ToLower();

            if (childN == childName && !childN.Contains("-line"))
            {
                return child;
            }
            else
            {
                Transform found = RecursiveFindChild(child, childName);
                if (found != null)
                {
                    return found;
                }
            }

        }
        return null;
    }
}
