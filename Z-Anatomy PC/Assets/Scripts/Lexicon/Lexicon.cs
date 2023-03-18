using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Diagnostics;
using System.Text;

public class Lexicon : MonoBehaviour
{
    public static Lexicon Instance;
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
    public float linesAlpha;
    [HideInInspector]
    public List<RectTransform> elements = new List<RectTransform>();
    [HideInInspector]
    public bool lexiconOnScreen;
    [HideInInspector]
    public LexiconElement highlighted;
    Vector2 newPos;
    [HideInInspector]
    public float realSpaceBtwnElements;

    public GameObject sliderPrefab;

    public Sprite noCheckboxSprite;

    private LexiconElement insertionsElement;

    private void Awake()
    {
        Instance = this;
        newPos = firstPosition.localPosition;
    }

    public void InstantiateSearchResult(List<GameObject> gameObjects3d)
    {
        ClearAllElements();
        newPos = firstPosition.localPosition;

        foreach (GameObject child in gameObjects3d)
        {
            AddElement(child);
        }

        UpdateTreeViewCheckboxes();
        CalculateScrollViewHeight();
    }

    public void InstatiateOriginalRootChilds()
    {
        Vector2 newPos = firstPosition.localPosition;

        foreach (Transform child in GlobalVariables.Instance.globalParent.transform)
        {
            BodyPartVisibility isVisibleScript = child.GetComponent<BodyPartVisibility>();
            if (!child.CompareTag("Debug"))
            {
                GameObject newElement = Instantiate(treeViewElementPrefab, elementsParent);
                RectTransform rt = newElement.GetComponent<RectTransform>();

                GameObject layersSlider = GetLayerByTag(child.tag);
                if (layersSlider != null)
                {
                    layersSlider.transform.SetParent(rt);
                    layersSlider.transform.localScale = new Vector2(.35f, .35f);
                    layersSlider.GetComponent<RectTransform>().anchoredPosition = new Vector2(rt.anchoredPosition.x - 60, 0);          
                }
                 
                elements.Add(rt);
                rt.localPosition = newPos;
                LexiconElement script = newElement.GetComponentInChildren<LexiconElement>();
                isVisibleScript.lexiconElement = script;
                script.element = child;
                script.deepLevel = 1;
                script.realName = child.name;
                script.nameScript = script.element.GetComponent<NameAndDescription>();
                script.checkBox = newElement.GetComponentInChildren<LexiconItemCheckbox>();

                if(child.CompareTag("Insertions"))
                {
                    insertionsElement = script;
                    script.expandBtn.gameObject.SetActive(false);
                }
                
                newElement.name = child.name;
                int indexOfPoint = child.name.LastIndexOf('.');
                string cleanName = child.name;
                if (indexOfPoint != -1)
                    cleanName = child.name.Substring(0, indexOfPoint);
                newElement.GetComponentInChildren<TextMeshProUGUI>().text = cleanName;
                newPos.y -= verticalSpacing;

                if (isVisibleScript.isVisible && script.checkBox != null)
                    script.checkBox.btn.Check();
            }
        }

        Layers.Instance.ReadLayers();
        CalculateScrollViewHeight();
        Layers.Instance.SyncLayers();
        RecalculateSpaceBtnElements();
    }

    private GameObject GetLayerByTag(string tag)
    {
        GameObject layersSlider = null;

        switch (tag)
        {
            case "Skeleton":
                layersSlider = Instantiate(sliderPrefab);
                layersSlider.name = "Skeleton slider";
                Layers.Instance.bonesSlider = layersSlider.GetComponent<Slider>();
                Layers.Instance.bonesSlider.onValueChanged.AddListener(delegate { Layers.Instance.UpdateBones(); });
                break;
            case "Joints":
                layersSlider = Instantiate(sliderPrefab);
                layersSlider.name = "Joints slider";
                Layers.Instance.ligamentsSlider = layersSlider.GetComponent<Slider>();
                Layers.Instance.ligamentsSlider.onValueChanged.AddListener(delegate { Layers.Instance.UpdateLigaments(); });
                break;
            case "Muscles":
                layersSlider = Instantiate(sliderPrefab);
                layersSlider.name = "Muscles slider";
                Layers.Instance.muscularSlider = layersSlider.GetComponent<Slider>();
                Layers.Instance.muscularSlider.onValueChanged.AddListener(delegate { Layers.Instance.UpdateMuscles(); });
                break;
            case "Nervous":
                layersSlider = Instantiate(sliderPrefab);
                layersSlider.name = "Nervous slider";
                Layers.Instance.nervesSlider = layersSlider.GetComponent<Slider>();
                Layers.Instance.nervesSlider.onValueChanged.AddListener(delegate { Layers.Instance.UpdateNerves(); });
                break;
            case "Visceral":
                layersSlider = Instantiate(sliderPrefab);
                layersSlider.name = "Viscera slider";
                Layers.Instance.visceralSlider = layersSlider.GetComponent<Slider>();
                Layers.Instance.visceralSlider.onValueChanged.AddListener(delegate { Layers.Instance.UpdateViscera(); });
                break;
            case "BodyParts":
                layersSlider = Instantiate(sliderPrefab);
                layersSlider.name = "Skin slider";
                Layers.Instance.skinSlider = layersSlider.GetComponent<Slider>();
                Layers.Instance.skinSlider.onValueChanged.AddListener(delegate { Layers.Instance.UpdateSkin(); });
                break;
            case "Fascia":
                layersSlider = Instantiate(sliderPrefab);
                layersSlider.name = "Fascia slider";
                Layers.Instance.fasciaSlider = layersSlider.GetComponent<Slider>();
                Layers.Instance.fasciaSlider.onValueChanged.AddListener(delegate { Layers.Instance.UpdateFascia(); });
                break;
            case "References":
                layersSlider = Instantiate(sliderPrefab);
                layersSlider.name = "Refs slider";
                Layers.Instance.refsSlider = layersSlider.GetComponent<Slider>();
                Layers.Instance.refsSlider.onValueChanged.AddListener(delegate { Layers.Instance.UpdateRefs(); });
                break;
            case "Veins":
                layersSlider = Instantiate(sliderPrefab);
                layersSlider.name = "Veins slider";
                Layers.Instance.veinsSlider = layersSlider.GetComponent<Slider>();
                Layers.Instance.veinsSlider.onValueChanged.AddListener(delegate { Layers.Instance.UpdateVeins(); });
                break;
            case "Arteries":
                layersSlider = Instantiate(sliderPrefab);
                layersSlider.name = "Arteries slider";
                Layers.Instance.arteriesSlider = layersSlider.GetComponent<Slider>();
                Layers.Instance.arteriesSlider.onValueChanged.AddListener(delegate { Layers.Instance.UpdateArteries(); });
                break;
            case "Lymph":
                layersSlider = Instantiate(sliderPrefab);
                layersSlider.name = "Lymphs slider";
                Layers.Instance.lymphsSlider = layersSlider.GetComponent<Slider>();
                Layers.Instance.lymphsSlider.onValueChanged.AddListener(delegate { Layers.Instance.UpdateLymphs(); });
                break;
        }

        return layersSlider;
    }

    public void InstatiateChilds(Transform parentIn3dView, RectTransform treeViewParent)
    {

        int childCount = 0;
        int indexOfParent = elements.IndexOf(treeViewParent) + 1;
        List<RectTransform> newElements = new List<RectTransform>();
        LexiconElement parentScript = treeViewParent.GetComponent<LexiconElement>();
        bool firstInstance = parentScript.childs.Count == 0;

        List<Transform> childs = new List<Transform>();

        foreach (Transform child in parentIn3dView)
        {
            childs.Add(child);
        }

        //Instance child
        foreach (Transform child in childs)
        {
            if (!child.name.Contains(".j") && !child.name.Contains(".i") && !child.CompareTag("Insertions"))
            {
                GameObject newElement;
                LexiconElement script;
                BodyPartVisibility isVisibleScript = child.GetComponent<BodyPartVisibility>();
                childCount++;
                if (!firstInstance)
                {
                    newElement = isVisibleScript.lexiconElement.gameObject;
                    newElement.SetActive(true);
                    script = newElement.GetComponentInChildren<LexiconElement>();
                    isVisibleScript.lexiconElement = script;
                    BodyPartVisibility parentIsVisibleScript = parentScript.element.GetComponent<BodyPartVisibility>();

                    if (script.checkBox.btn.isEnabled && parentIsVisibleScript.isVisible && (script.checkBox.btn.isPressed || (isVisibleScript != null && isVisibleScript.isVisible)))
                        script.checkBox.btn.Check();
                    if (isVisibleScript != null && isVisibleScript.isSelected)
                        script.SetElementAsSelected();
                }
                else
                {
                    newElement = Instantiate(treeViewElementPrefab, treeViewParent.position, Quaternion.identity, elementsParent);
                    script = newElement.GetComponentInChildren<LexiconElement>();
                    isVisibleScript.lexiconElement = script;
                    newElement.name = child.name;
                    script.element = child;
                    script.realName = child.name;
                    script.parent = parentScript;
                    if (parentScript != null)
                        script.deepLevel = parentScript.deepLevel + 1;
                    else
                        script.deepLevel = 1;

                    BodyPartVisibility parentIsVisibleScript = parentScript.element.GetComponent<BodyPartVisibility>();

                    if (isVisibleScript != null && isVisibleScript.isVisible && parentIsVisibleScript.isVisible)
                        script.checkBox.btn.Check();

                    if (isVisibleScript != null && isVisibleScript.isSelected)
                        script.SetElementAsSelected();

                    //Is it a real gameobject or it's just a hierachy object?
                    if ((child.childCount == 1 && (child.GetChild(0).name.Contains(".j") || child.GetChild(0).name.Contains(".i"))|| child.childCount == 0) && script.element.GetComponent<MeshRenderer>() == null)
                    {
                        script.checkBox.btn.image.sprite = noCheckboxSprite;
                        script.checkBox.btn.transform.localScale = new Vector2(0.2f ,0.2f);
                        script.checkBox.btn.isEnabled = false;
                    }

                    parentScript.childs.Add(newElement);
                    RectTransform rt = newElement.GetComponent<RectTransform>();
                    rt.offsetMax = new Vector2(-horizontalSpacing * script.deepLevel, rt.offsetMax.y);



                    var line = new GameObject();
                    Image img = line.AddComponent<Image>();
                    img.color = new Color(GlobalVariables.SecondaryColor.r, GlobalVariables.SecondaryColor.g, GlobalVariables.SecondaryColor.b, linesAlpha);

                    RectTransform lineRt = img.GetComponent<RectTransform>();
                    lineRt.SetParent(newElement.transform);
                    lineRt.pivot = new Vector2(0, 0.5f);
                    lineRt.anchorMin = new Vector2(0, 0.5f);
                    lineRt.anchorMax = new Vector2(0, 0.5f);

                    lineRt.anchoredPosition = new Vector2(-8, 0);
                    lineRt.SetWidth(12);
                    lineRt.SetHeight(1.5f);
                   
                }

                newElement.GetComponentInChildren<TextMeshProUGUI>().text = child.name.RemoveSuffix();
                newElements.Add(newElement.GetComponent<RectTransform>());
            }
        }


        parentScript.childCount = childCount;
        elements.InsertRange(indexOfParent, newElements);
        float xPos = treeViewParent.localPosition.x + horizontalSpacing;
        float yPos = treeViewParent.localPosition.y;

        //Set child position
        for (int i = indexOfParent; i < indexOfParent + childCount; i++)
        {
            yPos -= verticalSpacing;
            elements[i].localPosition = new Vector2(xPos, yPos);
           /* var elementScript = elements[i].GetComponent<TreeViewElement>();
            elementScript.checkBox.btn.transform.position = new Vector2(elementScript.parent.checkBox.btn.transform.position.x, elementScript.checkBox.btn.transform.position.y);*/
        }

        //Set other parents position
        for (int i = indexOfParent + childCount; i < elements.Count; i++)
        {
            elements[i].localPosition = new Vector2(elements[i].localPosition.x, elements[i - 1].localPosition.y - verticalSpacing);
        }

        //Update scroll view size
        CalculateScrollViewHeight();

    }

    public void AddElement(GameObject child, string lexName = null)
    {
        if (!child.CompareTag("Insertions"))
        {
            BodyPartVisibility isVisibleScript = child.GetComponent<BodyPartVisibility>();
            GameObject newElement = Instantiate(treeViewElementPrefab, elementsParent);
            RectTransform rt = newElement.GetComponent<RectTransform>();
            LexiconElement script = newElement.GetComponentInChildren<LexiconElement>();
            TextMeshProUGUI tmpro = newElement.GetComponentInChildren<TextMeshProUGUI>();
            isVisibleScript.lexiconElement = script;

            elements.Add(rt);
            rt.localPosition = newPos;
            script.element = child.transform;
            script.deepLevel = 1;

            script.realName = child.name;
            newElement.name = child.name;
            string cleanName = child.name.RemoveSuffix();
            if (isVisibleScript != null && isVisibleScript.isSelected)
                script.SetElementAsSelected();

            if(lexName != null)
                tmpro.text = new StringBuilder(lexName).Append(" [").Append(cleanName).Append("]").ToString();
            else
                tmpro.text = cleanName;


            //Highlight text
            int indexOfSearch = tmpro.text.ToLower().IndexOf(SearchEngine.Instance.mainInputField.text.ToLower());
            if (indexOfSearch == -1)
                indexOfSearch = tmpro.text.RemoveAccents().ToLower().IndexOf(SearchEngine.Instance.mainInputField.text.ToLower());
            string newText = tmpro.text;
            if (indexOfSearch != -1)
                newText = new StringBuilder().Append(tmpro.text.Substring(0, indexOfSearch)).Append("<color=#").Append(ColorUtility.ToHtmlStringRGB(GlobalVariables.HighligthColor)).Append(">")
                    .Append(tmpro.text.Substring(indexOfSearch, SearchEngine.Instance.mainInputField.text.Length)).Append("</color>")
                    .Append(tmpro.text.Substring(indexOfSearch + SearchEngine.Instance.mainInputField.text.Length)).ToString();
            tmpro.text = newText;

            newPos.y -= verticalSpacing;
        }

        UpdateTreeViewCheckboxes();
        CalculateScrollViewHeight();
    }

    public void ExpandRecursively()
    {
        SearchEngine.Instance.AbortSearch();
        CollapseAll();
        for (int i = HierarchyBar.Instance.hierarchyObjects.Count - 1; i >= 0; i--)
        {
            LexiconElement lexElement = HierarchyBar.Instance.hierarchyObjects[i].GetComponent<BodyPartVisibility>().lexiconElement;
            if(lexElement == null)
            {
                UnityEngine.Debug.LogError("Lexicon element not found: " + HierarchyBar.Instance.hierarchyObjects[i].name);
                continue;
            }
            if (!lexElement.opened)
                lexElement.Open(true);
            if (i == 0)
            {
                lexElement.SetElementAsSelected();
                SetHighlighted(lexElement);
                FocusOnItem(lexElement.GetComponent<RectTransform>());
            }
        }
    }

    public void UpdateTreeViewCheckboxes(bool updateSliders = true)
    {
        foreach (var element in elements)
        {
            LexiconElement elementScript = element.GetComponent<LexiconElement>();
            var isVisibleScript = elementScript.element.GetComponent<BodyPartVisibility>();

            if(elementScript != null)
            {
                if (elementScript.isParent)
                {
                    if (HasActiveChilds(elementScript.element))
                    {
                        isVisibleScript.isVisible = true;
                        elementScript.checkBox.btn.Check();
                    }
                    else
                    {
                        isVisibleScript.isVisible = false;
                        elementScript.RecursivelyUncheckChildren(elementScript.childs);
                        elementScript.checkBox.btn.Uncheck();
                    }
                }
                else
                {
                    if (isVisibleScript.isVisible)
                    {
                        elementScript.checkBox.btn.Check();
                        elementScript.RecursivelyCheckParent(elementScript);
                    }
                    else
                    {
                        elementScript.checkBox.btn.Uncheck();
                        isVisibleScript.isVisible = false;
                    }
                }
            }
        }

        if(insertionsElement != null)
        {
            if (GlobalVariables.Instance.insertions.Find(it => it.visibilityScript.isVisible))
                insertionsElement.checkBox.btn.Check();
            else
                insertionsElement.checkBox.btn.Uncheck();
        }

        if (!SearchEngine.onSearch && updateSliders)
            Layers.Instance.SyncLayers();
    }

    public void FocusOnItem(RectTransform rt)
    {
        StartCoroutine(scrollView.FocusOnItemCoroutine(rt, 2));
    }

    private bool HasActiveChilds(Transform parent)
    {
        return parent.GetComponentsInChildren<TangibleBodyPart>(true).Where(it => it.isActiveAndEnabled).Count() > 0;
    }

    public void UpdateSelected()
    {
        foreach (var element in elements)
        {
            LexiconElement elementScript = element.GetComponent<LexiconElement>();
            if (elementScript != null)
            {
                var script = elementScript.element.GetComponent<BodyPartVisibility>();
                if(script != null)
                {
                    elementScript.SetElementAsUnselected();
                    if (script.isSelected)
                        elementScript.SetElementAsSelected();
                }
            }
        }
    }

    public void SetHighlighted(LexiconElement element)
    {
        if(highlighted != null)
            highlighted.SetDefaultColor();

        highlighted = element;

        if(highlighted != null)
            highlighted.SetElementAsHighlighted();
    }


    public void CollapseChild(RectTransform treeViewParent)
    {
        LexiconElement script = treeViewParent.GetComponent<LexiconElement>();
        int indexOfParent = elements.IndexOf(treeViewParent) + 1;

        //Destroy childs
        for (int i = indexOfParent; i < indexOfParent + script.childCount; i++)
        {
            LexiconElement element = elements[i].GetComponent<LexiconElement>();
            if (element.opened)
                element.Close(true);
            elements[i].gameObject.SetActive(false);
        }

        elements.RemoveRange(indexOfParent, script.childCount);

        if (indexOfParent + 1 < elements.Count)
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
        if (elements.Count > 0)
            elementsParent.sizeDelta = new Vector2(elementsParent.sizeDelta.x, Vector2.Distance(elements[0].localPosition, elements[elements.Count - 1].localPosition) + verticalSpacing * 2);
    }

    public void ResetAll()
    {
        ClearAllElements();
        InstatiateOriginalRootChilds();
        CalculateScrollViewHeight();
        UpdateTreeViewCheckboxes();
        SearchEngine.Instance.emptyStateScreen.SetActive(false);
    }

    public void ClearAllElements()
    {
        foreach (Transform item in elementsParent)
        {
            if (item != firstPosition)
                Destroy(item.gameObject);
        }
        elements.Clear();
        newPos = firstPosition.localPosition;
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

    public void ReturnToOrigin()
    {
        // scrollView.normalizedPosition = new Vector2(0, 1);
        StartCoroutine(scrollView.FocusOnItemCoroutine(firstPosition, 2));
    }

    public void ActivateBackToTopBtn()
    {
        //backToTopBtn.gameObject.SetActive(elementsParent.localPosition.y > firstPosition.rect.y + 55);
    }

    public void CollapseAll()
    {
        List<LexiconElement> ordered = elements.ConvertAll(it => it.GetComponent<LexiconElement>()).Where(it => it.opened).OrderByDescending(it => it.deepLevel).ToList();
        foreach (var item in ordered)
        {
            if (item.opened)
                item.Close(true);
        }
    }

    public void RecalculateSpaceBtnElements()
    {
        if(elements.Count > 1)
            realSpaceBtwnElements = Mathf.Abs(elements[0].position.y - elements[1].position.y);
    }
}

