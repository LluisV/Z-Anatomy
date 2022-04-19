using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class TreeViewCanvas : MonoBehaviour
{
    public static TreeViewCanvas instance;
    public Transform root;
    public RectTransform background;
    public RectTransform firstPosition;
    public RectTransform elementsParent;
    public GameObject treeViewElementPrefab;
    public Canvas canvas;
    public ScrollRect scrollView;
    public Button backToTopBtn;
    public Button collapseAllBtn;
    public ExpandCollapseUI expandScript;
    public float verticalSpacing;
    public float horizontalSpacing;
    private float originContentPositionY;
    public GameObject[] fascias;
    [HideInInspector]
    public List<RectTransform> elements = new List<RectTransform>();
    RectTransform canvasRT;
    private bool firstInstance = true;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        canvasRT = canvas.GetComponent<RectTransform>();

        SetPosToRight();
        SetCollapsedToRight();
        expandScript.expandedPosition = new Vector2(0, 0);

        originContentPositionY = elementsParent.localPosition.y;
        background.gameObject.SetActive(false);
    }


    public void InstantiateRoots(List<GameObject> gameObjects3d, bool isSearch)
    {
        ClearAllElements();
        Vector2 newPos = firstPosition.localPosition;
        foreach (GameObject child in gameObjects3d.OrderByDescending(it => it.name.Contains(".g")))
        {
            IsVisible isVisibleScript = child.GetComponent<IsVisible>();
            if (!child.CompareTag("Debug") && !child.CompareTag("Insertions") && !child.name.Contains("-lin") && !child.name.Contains(".gLabel") && !child.name.Contains("label"))
            {
                GameObject newElement = Instantiate(treeViewElementPrefab, elementsParent);
                RectTransform rt = newElement.GetComponent<RectTransform>();
                TreeViewElement script = newElement.GetComponentInChildren<TreeViewElement>();
                TextMeshProUGUI tmpro = newElement.GetComponentInChildren<TextMeshProUGUI>();
                elements.Add(rt);
                rt.localPosition = newPos;
                script.element = child.transform;
                script.deepLevel = 1;
                //if (!child.name.Contains(".g"))
                if (child.name.Contains("-lin") || child.transform.childCount == 0 || (child.transform.childCount == 1 && child.transform.GetChild(0).name.Contains("-lin")))
                    script.expandBtn.gameObject.SetActive(false);
                script.realName = child.name;
                newElement.name = child.name;
                int indexOfPoint = child.name.IndexOf('.');
                string cleanName = child.name;
                if (indexOfPoint != -1)
                    cleanName = child.name.Substring(0, indexOfPoint);
                tmpro.text = cleanName;
                if (isSearch)
                {
                    //Highlight text

                    int indexOfSearch = tmpro.text.ToLower().IndexOf(TreeViewSearchEngine.instance.mainInputField.text.ToLower());
                    if (indexOfSearch == -1)
                        indexOfSearch = StaticMethods.RemoveAccents(tmpro.text).ToLower().IndexOf(TreeViewSearchEngine.instance.mainInputField.text.ToLower());
                    string newText = tmpro.text;
                    if (indexOfSearch != -1)
                        newText = string.Concat(tmpro.text.Substring(0, indexOfSearch), "<color=#fc7b03>" + tmpro.text.Substring(indexOfSearch, TreeViewSearchEngine.instance.mainInputField.text.Length), "</color>" + tmpro.text.Substring(indexOfSearch + TreeViewSearchEngine.instance.mainInputField.text.Length));
                    tmpro.text = newText;
                }

                newPos.y -= verticalSpacing;
                if (isVisibleScript != null && isVisibleScript.isVisible)
                    script.checkBox.Check();
            }
        }
        CalculateScrollViewHeight();
    }
    public void InstatiateOriginalRootChilds()
    {
        Vector2 newPos = firstPosition.localPosition;
        string a = "";
        try
        {
            foreach (Transform child in root)
            {
                a = child.name;
                IsVisible isVisibleScript = child.GetComponent<IsVisible>();
                if (!child.CompareTag("Debug"))
                {
                    GameObject newElement = Instantiate(treeViewElementPrefab, elementsParent);
                    RectTransform rt = newElement.GetComponent<RectTransform>();
                    elements.Add(rt);
                    rt.localPosition = newPos;
                    TreeViewElement script = newElement.GetComponentInChildren<TreeViewElement>();
                    script.element = child;
                    script.deepLevel = 1;
                    script.realName = child.name;
                    script.nameScript =  script.element.GetComponent<NameAndDescription>();
                    script.checkBox = newElement.GetComponentInChildren<ButtonSpriteSwap>();
                    if (child.CompareTag("Insertions"))
                    {
                        script.expandBtn.gameObject.SetActive(false);    
                        if(!firstInstance)
                        {
                            script.DisableElement();
                            firstInstance = false;
                        }
                    }
                    newElement.name = child.name;
                    int indexOfPoint = child.name.IndexOf('.');
                    string cleanName = child.name;
                    if (indexOfPoint != -1)
                        cleanName = child.name.Substring(0, indexOfPoint);
                    newElement.GetComponentInChildren<TextMeshProUGUI>().text = cleanName;
                    newPos.y -= verticalSpacing;

                    if (isVisibleScript.isVisible && script.checkBox != null)
                        script.checkBox.Check();
                }
            }
        }
        catch
        {
            Debug.Log(a);
        }
      
        CalculateScrollViewHeight();
    }
    public void InstatiateChilds(Transform parentIn3dView, RectTransform treeViewParent)
    {

        int childCount = 0;
        int indexOfParent = elements.IndexOf(treeViewParent) + 1;
        List<RectTransform> newElements = new List<RectTransform>();
        TreeViewElement parentScript = treeViewParent.GetComponent<TreeViewElement>();
        bool firstInstance = parentScript.childs.Count == 0;

        List<Transform> childs = new List<Transform>();

        foreach (Transform child in parentIn3dView)
        {
            if (child.name.Contains(".labels"))
            {
                foreach (Transform grandchild in child)
                {
                    childs.Add(grandchild);
                }
            }
            else
                childs.Add(child);
        }

        //Instance child
        foreach (Transform child in childs)
        {
            //&& !child.name.Contains(".t") && !child.name.Contains("label")
            if (!child.CompareTag("Debug")  && !child.name.Contains("-lin"))
            {
                GameObject newElement;
                TreeViewElement script;
                IsVisible isVisibleScript = child.GetComponent<IsVisible>();
                childCount++;
                if (!firstInstance)
                {
                    newElement = parentScript.childs.Where(it => it.name == child.name).First();
                    newElement.SetActive(true);
                    script = newElement.GetComponentInChildren<TreeViewElement>();
                    if (script.checkBox.isEnabled && (script.checkBox.isPressed || (isVisibleScript != null && isVisibleScript.isVisible)))
                        script.checkBox.Check();
                }
                else
                {
                    newElement = Instantiate(treeViewElementPrefab, treeViewParent.position, Quaternion.identity, elementsParent);
                    script = newElement.GetComponentInChildren<TreeViewElement>();
                    newElement.name = child.name;
                    script.element = child;
                    script.realName = child.name;
                    script.parent = parentScript;
                    script.isParent = true;
                    if (parentScript != null)
                        script.deepLevel = parentScript.deepLevel + 1;
                    else
                        script.deepLevel = 1;

                    //Is it a group? We hide de + icon if it have no childs, or it's only child is a glabel and/or a line
                   // if (child.childCount == 0 || child.CompareTag("Insertions") || (child.name.Contains(".g") && child.childCount == 1 && child.Find(child.name.Replace(".g", ".gLabel")) != null) || (child.name.Contains(".g") && child.childCount == 2 && child.Find(child.name.Replace(".g", "-line")) != null))
                    if(child.name.Contains("-lin") || child.childCount == 0 || child.CompareTag("Insertions") || (child.transform.childCount == 1 && child.transform.GetChild(0).name.Contains("-lin")))
                    {
                        script.expandBtn.gameObject.SetActive(false);
                        script.isParent = false;
                    }

                    if (isVisibleScript != null && isVisibleScript.isVisible)
                        script.checkBox.Check();

                    //Is it a real gameobject or it's just a hierachy object?
                    if ((child.childCount == 1 && child.GetChild(0).name.Contains("-lin") || child.childCount == 0) && script.element.GetComponent<MeshRenderer>() == null)
                    {
                        script.checkBox.isEnabled = false;
                        script.checkBox.Uncheck();
                        script.checkBox.GetComponent<Image>().color = Color.gray;
                    }

                    parentScript.childs.Add(newElement);
                    RectTransform rt = newElement.GetComponent<RectTransform>();
                    rt.offsetMax = new Vector2(-horizontalSpacing * script.deepLevel, rt.offsetMax.y);
                }
                string cleanName = child.name;
                int indexOfPoint = cleanName.IndexOf('.');
                if(indexOfPoint != -1)
                {
                    string suffix = cleanName.Substring(indexOfPoint);
                    if (suffix.Contains(".labels"))
                        cleanName = cleanName.Replace(suffix, " parts");
                    else
                        cleanName = cleanName.Replace(suffix, "");
                }
                newElement.GetComponentInChildren<TextMeshProUGUI>().text = cleanName;
                newElements.Add(newElement.GetComponent<RectTransform>());
            }
        }


        parentScript.childCount = childCount;
        //.OrderByDescending(it => it.name.Contains(".g"))
        newElements = newElements.OrderByDescending(it => it.GetComponentInChildren<TreeViewElement>().isParent).ToList();
        elements.InsertRange(indexOfParent, newElements);
        float xPos = treeViewParent.localPosition.x + horizontalSpacing;
        float yPos = treeViewParent.localPosition.y;

        //Set child position
        for (int i = indexOfParent; i < indexOfParent + childCount; i++)
        {
            yPos -= verticalSpacing;
            elements[i].localPosition = new Vector2(xPos, yPos);
        }

        //Set other parents position
        for (int i = indexOfParent + childCount; i < elements.Count; i++)
        {
            elements[i].localPosition = new Vector2(elements[i].localPosition.x, elements[i - 1].localPosition.y - verticalSpacing);
        }

        //Update scroll view size
        CalculateScrollViewHeight();
    }
    public void UpdateTreeViewCheckboxes()
    {
        foreach (var element in elements)
        {
            TreeViewElement elementScript = element.GetComponent<TreeViewElement>(); 
            if(elementScript != null && elementScript.element.GetComponent<IsVisible>().isVisible && elementScript.checkBox != null)
                elementScript.checkBox.Check();
            else if (elementScript != null && elementScript.checkBox != null)
                elementScript.checkBox.Uncheck();
        }
    }
    public void CollapseChild(RectTransform treeViewParent)
    {
        TreeViewElement script = treeViewParent.GetComponent<TreeViewElement>();
        int indexOfParent = elements.IndexOf(treeViewParent) + 1;

        //Destroy childs
        for (int i = indexOfParent; i < indexOfParent + script.childCount; i++)
        {
            if (elements[i].GetComponent<TreeViewElement>().opened)
                elements[i].GetComponent<TreeViewElement>().OpenCloseClick();
            elements[i].gameObject.SetActive(false);
        }

        elements.RemoveRange(indexOfParent, script.childCount);

        if(indexOfParent + 1 < elements.Count)
            elements[indexOfParent + 1].localPosition = new Vector2(elements[indexOfParent + 1].localPosition.x, elements[indexOfParent + 1].localPosition.y - verticalSpacing);

        //Collapse objects below
        if (indexOfParent != 0)
        {
            for (int i = indexOfParent; i < elements.Count; i++)
            {
                elements[i].localPosition = new Vector2(elements[i].localPosition.x, elements[i - 1].localPosition.y - verticalSpacing);
            }
        }


        //Update scroll view size
        elementsParent.sizeDelta = new Vector2(elementsParent.sizeDelta.x, elements.Count * verticalSpacing);
    }
    
    private void CalculateScrollViewHeight()
    {
        if(elements.Count > 0)
            elementsParent.sizeDelta = new Vector2(elementsParent.sizeDelta.x, Vector2.Distance(elements[0].localPosition, elements[elements.Count - 1].localPosition) + verticalSpacing * 2);
        //ReturnToOrigin();
    }
    public void Reset()
    {
        ClearAllElements();
        InstatiateOriginalRootChilds();
        CalculateScrollViewHeight();
    }
    public void ClearAllElements()
    {
        foreach (Transform item in elementsParent)
        {
            if(item != firstPosition)
                Destroy(item.gameObject);
        }
        elements.Clear();
    }

    public void UpdateElements(List<RectTransform> newList)
    {
        ClearAllElements();
        elements = newList;
    }


    // ----------------------- UI EVENTS ------------------------------//


    public void SetPanelSize()
    {
        RectTransform canvasRT = canvas.GetComponent<RectTransform>();
        background.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvasRT.sizeDelta.x);
        background.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvasRT.sizeDelta.y);
    }

    public void SetPosToLeft() => background.localPosition = new Vector2(-canvasRT.rect.width, 0);
    
    public void SetPosToRight() => background.localPosition = new Vector2(canvasRT.rect.width, 0);
    
    public void SetCollapsedToLeft() => expandScript.collapasedPosition = new Vector2(-background.rect.width, 0);

    public void SetCollapsedToRight() => expandScript.collapasedPosition = new Vector2(background.rect.width, 0);


    public void ReturnToOrigin()
    {
        scrollView.normalizedPosition = new Vector2(0, 1);
    }

    public void ActivateBackToTopBtn()
    {
        backToTopBtn.gameObject.SetActive(elementsParent.localPosition.y > firstPosition.rect.y + 55);
    }

    public void CollapseAll()
    {
        foreach (var item in elements.ToList())
        {
            TreeViewElement script = item.GetComponent<TreeViewElement>();
            if (script.deepLevel == 1 && script.opened)
                script.OpenCloseClick();
        }
    }

    public void EnableDisableFascias(bool state)
    {
        foreach (var item in fascias)
        {
            item.SetActive(state);
            if(state)
                StaticMethods.SetActiveParentsRecursively(item.transform, true);
            IsVisible script = item.GetComponent<IsVisible>();
            if (script != null)
                script.isVisible = state;
            if(!state)
                SelectedObjectsManagement.instance.DeselectObjet(item);
        }
    }
}
