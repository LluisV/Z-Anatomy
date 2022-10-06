using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SelectedObjectsManagement : MonoBehaviour
{
    public static SelectedObjectsManagement Instance;
    [HideInInspector]
    public List<GameObject> selectedObjects = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> lastIsolatedObjects = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> activeObjects = new List<GameObject>();
    public CameraController cam;
    [HideInInspector]
    public Transform lastParentSelected;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ActionControl.Instance.UpdateButtons();
        GetActiveObjects();
        
    }

    public void GetActiveObjects()
    {
        //Optimize this
        activeObjects.Clear();
        List<MeshRenderer> activeRenderers = GlobalVariables.Instance.allBodyPartRenderers.Where(it => it.gameObject.activeInHierarchy).ToList();
        foreach (MeshRenderer item in activeRenderers)
        {
            activeObjects.Add(item.gameObject);
        }
        cam.UpdateBounds();
        UpdateCrossSectionToggles();
    }

    private void UpdateCrossSectionToggles()
    {
        bool skeletalActive = activeObjects.Find(it => it.CompareTag("Skeleton"));
        bool jointsActive = activeObjects.Find(it => it.CompareTag("Joints"));
        bool muscularActive = activeObjects.Find(it => it.CompareTag("Muscles"));
        bool fasciaActive = activeObjects.Find(it => it.CompareTag("Fascia"));
        bool arteriesActive = activeObjects.Find(it => it.CompareTag("Arteries"));
        bool veinsActive = activeObjects.Find(it => it.CompareTag("Veins"));
        bool lymphActive = activeObjects.Find(it => it.CompareTag("Lymph"));
        bool nervousActive = activeObjects.Find(it => it.CompareTag("Nervous"));
        bool visceralActive = activeObjects.Find(it => it.CompareTag("Visceral"));
        bool regionsActive = activeObjects.Find(it => it.CompareTag("BodyParts"));
        bool referencesActive = activeObjects.Find(it => it.CompareTag("References"));

        bool[] allBools = { skeletalActive, jointsActive, muscularActive, fasciaActive, veinsActive, arteriesActive, nervousActive, visceralActive, regionsActive, referencesActive };
        
        bool oneAtLeast = allBools.Where(it => it).Count() > 1;

        CrossSections.Instance.skeletalToggle.transform.parent.gameObject.SetActive(skeletalActive && oneAtLeast);
        CrossSections.Instance.jointsToggle.transform.parent.gameObject.SetActive(jointsActive && oneAtLeast);
        CrossSections.Instance.muscularToggle.transform.parent.gameObject.SetActive(muscularActive && oneAtLeast);
        CrossSections.Instance.fasciaToggle.transform.parent.gameObject.SetActive(fasciaActive && oneAtLeast);
        CrossSections.Instance.arteriesToggle.transform.parent.gameObject.SetActive(arteriesActive && oneAtLeast);
        CrossSections.Instance.veinsToggle.transform.parent.gameObject.SetActive(veinsActive && oneAtLeast);
        CrossSections.Instance.lymphsToggle.transform.parent.gameObject.SetActive(lymphActive && oneAtLeast);
        CrossSections.Instance.nervousToggle.transform.parent.gameObject.SetActive(nervousActive && oneAtLeast);
        CrossSections.Instance.visceralToggle.transform.parent.gameObject.SetActive(visceralActive && oneAtLeast);
        CrossSections.Instance.regionsToggle.transform.parent.gameObject.SetActive(regionsActive && oneAtLeast);
        CrossSections.Instance.referencesToggle.transform.parent.gameObject.SetActive(referencesActive && oneAtLeast);
    }

    public void DeleteOutlineToActiveObjects()
    {
        foreach (var selected in activeObjects)
        {
            //NORMAL MASK
            if(selected.GetComponent<BodyPart>() != null)
                selected.layer = 6;
        }
    }

    public void DeleteOutlineToSelectedObjects()
    {
        foreach (var selected in selectedObjects)
        {
            //NORMAL MASK
            if (selected.GetComponent<BodyPart>() != null)
                selected.layer = 6;
        }
    }

    public void SetOutlineToSelectedObjects()
    {
        foreach (var selected in selectedObjects)
        {
            //OUTLINE MASK
            if (selected.GetComponent<BodyPart>() != null)
                selected.layer = 7;
        }
    }

    public void DeleteClicked(GameObject go)
    {
        GameObject[] deletedGo = new GameObject[1];
        deletedGo[0] = go;
        Visibility isVisibleScript = go.GetComponent<Visibility>();
        if (isVisibleScript != null)
            isVisibleScript.isVisible = false;
        go.SetActive(false);
        ActionControl.Instance.AddCommand(new DeleteCommand(go), true);
        DeselectObject(go);
        Lexicon.Instance.UpdateTreeViewCheckboxes();
        ActionControl.Instance.UpdateButtons();
        SetOutlineVisibility();

    }

    public void DeleteSelected()
    {
        ActionControl.Instance.AddCommand(new DeleteCommand(selectedObjects), true);
    }

    public void DeleteList(List<GameObject> deletedArray)
    {
        foreach (var item in deletedArray)
        {
            Visibility isVisibleScript = item.GetComponent<Visibility>();
            if(isVisibleScript != null)
            {
                isVisibleScript.isVisible = false;
                if(isVisibleScript.isSelected)
                    DeselectObject(item);
            }
            activeObjects.Remove(item);
            item.SetActive(false);
        }
        Lexicon.Instance.UpdateTreeViewCheckboxes();
        ActionControl.Instance.UpdateButtons();
        SetOutlineVisibility();
    }

    public void DeselectObject(GameObject selected)
    {
        BodyPart script = selected.GetComponent<BodyPart>();
        Visibility isVisible = selected.GetComponent<Visibility>();
        Label label = selected.GetComponent<Label>();
        if (isVisible != null)
            isVisible.isSelected = false;

        selectedObjects.Remove(selected);
        if (script != null || label != null)
        {
            if(script != null)
                script.Deselect();
            if (label != null)
                label.Deselect();
            SetOutlineVisibility();

            if (selectedObjects.Count == 0)
            {
                NamesManagement.Instance.SetDesc(NamesManagement.NO_SELECTION);
                HierarchyBar.Instance.Clear();
            }
        }
        Lexicon.Instance.UpdateSelected();
    }

    public void DeselectAllObjects(bool hideLabels = true)
    {
        DeleteOutlineToSelectedObjects();
        foreach (var item in selectedObjects)
        {
            BodyPart script = item.GetComponent<BodyPart>();
            Visibility isVisible = item.GetComponent<Visibility>();
            Label label = item.GetComponent<Label>();
            if (isVisible != null && hideLabels)
            {
                isVisible.isSelected = false;
                isVisible.HideLabels();
            }
            if (script != null)
                script.Deselect(hideLabels);
            if (label != null)
                label.Deselect();
        }
        selectedObjects.Clear();

        SetOutlineVisibility();
        ActionControl.Instance.UpdateButtons();
        Lexicon.Instance.UpdateSelected();
        NoBodyPartInfo();
    }

    public void DeselectAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            DeselectObject(child.gameObject);
            if (child.childCount > 0)
                DeselectAllChildren(child);
        }
    }

    public void SelectObject(GameObject selected)
    {
        if (!selected.activeInHierarchy)
            selected.transform.SetActiveParentsRecursively(true);

        selected.SetActive(true);

        if(selected.GetComponent<Visibility>() != null)
            selectedObjects.Add(selected);

        BodyPart script = selected.GetComponent<BodyPart>();
        Visibility isVisible = selected.GetComponent<Visibility>();
        Label label = selected.GetComponent<Label>();

        if(isVisible != null)
        {
            isVisible.isVisible = true;
            isVisible.isSelected = true;
        }
        if (script != null)
            script.Select();
        if (label != null)
            label.Select();

        SetOutlineVisibility();
    }

    public void SelectAllChildren(Transform parent, bool lastIteration = true, List<GameObject> shown = null)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.IsLabel())
                continue;

            Line line = child.GetComponent<Line>();
            if (line != null)
                continue;

            Visibility isVisible = child.GetComponent<Visibility>();

            if (child.CompareTag("Insertions"))
                continue;

            selectedObjects.Add(child.gameObject);

            if(isVisible != null)
            {
                isVisible.isVisible = true;
                isVisible.isSelected = true;
            }

            BodyPart script = child.GetComponent<BodyPart>();
            if (script != null)
                script.Select();

            if (!child.gameObject.activeInHierarchy)
            {
                child.SetActiveParentsRecursively(true);
                if(shown != null)
                    shown.Add(child.gameObject);
            }

            child.gameObject.SetActive(true);           

            if (child.childCount > 0)
                SelectAllChildren(child, false, shown);
        }

        //Delete duplicates
        if (lastIteration)
        {
            selectedObjects = selectedObjects.Distinct().ToList();
            Lexicon.Instance.UpdateSelected();
            ActionControl.Instance.UpdateButtons();
            SetOutlineVisibility();

        }
    }

    public void UpHierarchyClick()
    {
        if (lastParentSelected == null)
            lastParentSelected = selectedObjects[selectedObjects.Count - 1].transform.parent;
        else
            lastParentSelected = lastParentSelected.parent;


        DeselectAllObjects();
        lastParentSelected.gameObject.SetActive(true);
        //Update hierarchy bar
        HierarchyBar.Instance.Set(lastParentSelected.transform);

        NameAndDescription descriptionScript = lastParentSelected.GetComponent<NameAndDescription>();
        if (descriptionScript != null)
            descriptionScript.SetDescription();

        SelectObject(lastParentSelected.gameObject);
        SelectAllChildren(lastParentSelected);

        ActionControl.Instance.AddCommand(new SelectCommand(selectedObjects), false);
        ActionControl.Instance.UpdateButtons();

        if(ActionControl.zoomSelected)
            cam.CenterView(true);
        CheckLabelsVisibility();

        //Expand in lexicon
        Lexicon.Instance.ExpandRecursively();
    }

    private void SetOutlineVisibility()
    {
        Settings.Instance.SetOutline(selectedObjects.Count > 0);
    }

    public void CheckLabelsVisibility()
    {
        GetActiveObjects();
        if (activeObjects.Count <= 1)
            return;
        foreach (var item in activeObjects)
        {
            Visibility isVisible = item.GetComponent<Visibility>();
            if (item != null && isVisible != null)
                isVisible.HideLabels();
        }
    }

    public void ShowBodyPartInfo(GameObject go)
    {
        NameAndDescription nameScript = go.GetComponent<NameAndDescription>();
        //Set the description
        nameScript.SetDescription();
        //Set the hierarchy bar
        HierarchyBar.Instance.Set(go.transform);
        //Expand in lexicon
        Lexicon.Instance.ExpandRecursively();
    }

    public void NoBodyPartInfo()
    {
        //Set the description
        //NamesManagement.instance.SetDesc(NamesManagement.NO_SELECTION);
        //Set the hierarchy bar
        HierarchyBar.Instance.Clear();
        Lexicon.Instance.SetHighlighted(null);
    }

    public void InvertSelection()
    {
        List<GameObject> toSelect = activeObjects.Where(it => !selectedObjects.Contains(it)).ToList();
        DeselectAllObjects();
        ActionControl.Instance.AddCommand(new SelectCommand(toSelect), true);
    }
}
