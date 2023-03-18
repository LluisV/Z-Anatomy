using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// Handles the selection and deselection of objects.
/// </summary>
public class SelectedObjectsManagement : MonoBehaviour
{
    public static SelectedObjectsManagement Instance;
    [HideInInspector]
    public List<GameObject> selectedObjects = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> lastIsolatedObjects = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> activeObjects = new List<GameObject>();
    private CameraController cam;
    [HideInInspector]
    public Transform lastParentSelected;

    private void Awake()
    {
        Instance = this;
        cam = Camera.main.GetComponent<CameraController>();
    }
    private void Start()
    {
        ActionControl.Instance.UpdateButtons();
        GetActiveObjects();
        
    }

    /// <summary>
    /// This method clears the list of active objects and updates it with all currently active objects in the scene. 
    /// It then updates the camera's bounds and cross-section toggles to reflect any changes.
    /// </summary>
    public void GetActiveObjects()
    {
        activeObjects.Clear();
        //Can be optimized!
        activeObjects.AddRange(GlobalVariables.Instance.allBodyParts.Where(it => it.gameObject.activeInHierarchy).Select(it => it.gameObject).ToList());
        cam.UpdateBounds();
        UpdateCrossSectionToggles();
    }

    ///<summary>
    /// Updates the visibility of the cross-section toggles based on the currently active objects.
    ///</summary>
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

    /// <summary>
    /// Removes the outline to all the active objects.
    /// </summary>
    public void DeleteOutlineToActiveObjects()
    {
        foreach (var selected in activeObjects)
        {
            //NORMAL MASK
            if(selected.GetComponent<TangibleBodyPart>() != null)
                selected.layer = 6;
        }
    }

    /// <summary>
    /// Removes the outline to all the selected objects.
    /// </summary>
    public void DeleteOutlineToSelectedObjects()
    {
        foreach (var selected in selectedObjects)
        {
            //NORMAL MASK
            if (selected.GetComponent<TangibleBodyPart>() != null)
                selected.layer = 6;
        }
    }

    /// <summary>
    /// Sets the outline to all the selected objects.
    /// </summary>
    public void SetOutlineToSelectedObjects()
    {
        foreach (var selected in selectedObjects)
        {
            //OUTLINE MASK
            if (selected.GetComponent<TangibleBodyPart>() != null)
                selected.layer = 7;
        }
    }

    /// <summary>
    /// Deletes the selected objects.
    /// </summary>
    public void DeleteSelected()
    {
        ActionControl.Instance.AddCommand(new DeleteCommand(selectedObjects), true);
    }

    /// <summary>
    /// Deletes a list of GameObjects from the scene, setting their visibility to false and removing them from the activeObjects list.
    /// Then updates the UI.
    /// </summary>
    /// <param name="deletedList">The list of GameObjects to be deleted.</param>
    public void DeleteList(List<GameObject> deletedList)
    {
        foreach (var item in deletedList)
        {
            BodyPartVisibility isVisibleScript = item.GetComponent<BodyPartVisibility>();
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

    /// <summary>
    /// Deselects a GameObject from the currently selected objects.
    /// </summary>
    /// <param name="selected">The GameObject to be deselected.</param>
    public void DeselectObject(GameObject selected)
    {
        TangibleBodyPart script = selected.GetComponent<TangibleBodyPart>();
        BodyPartVisibility isVisible = selected.GetComponent<BodyPartVisibility>();
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

    /// <summary>
    /// Deselects all selected objects.
    /// </summary>
    public void DeselectAllObjects(bool hideLabels = true)
    {
        DeleteOutlineToSelectedObjects();
        foreach (var item in selectedObjects)
        {
            TangibleBodyPart script = item.GetComponent<TangibleBodyPart>();
            BodyPartVisibility isVisible = item.GetComponent<BodyPartVisibility>();
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


    /// <summary>
    /// Recursively deselects all child objects of a given parent object.
    /// </summary>
    /// <param name="parent">The parent transform.</param>
    public void DeselectAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            DeselectObject(child.gameObject);
            if (child.childCount > 0)
                DeselectAllChildren(child);
        }
    }

    /// <summary>
    /// Selects a GameObject.
    /// </summary>
    /// <param name="selected">The GameObject to be selected.</param>
    public void SelectObject(GameObject selected)
    {
        if (!selected.activeInHierarchy)
            selected.transform.SetActiveParentsRecursively(true);

        selected.SetActive(true);

        if(selected.GetComponent<BodyPartVisibility>() != null)
            selectedObjects.Add(selected);

        TangibleBodyPart script = selected.GetComponent<TangibleBodyPart>();
        BodyPartVisibility isVisible = selected.GetComponent<BodyPartVisibility>();
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

    /// <summary>
    /// Recursively selects all the children of a parent Transform, excluding objects with Line or Label components and objects with the tag "Insertions".
    /// Selected objects are added to the list of selectedObjects and their visibility is set to true.
    /// If the child object is not active in the hierarchy, it is set active and its parents are also set active recursively.
    /// </summary>
    /// <param name="parent">The parent Transform of the children to select.</param>
    /// <param name="lastIteration">A bool that specifies if this is the last iteration of the recursive function. Default is true.</param>
    /// <param name="shown">A List of GameObjects that specifies which objects are to be shown. Default is null.</param>
    public void SelectAllChildren(Transform parent, bool lastIteration = true, List<GameObject> shown = null)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.IsLabel())
                continue;

            Line line = child.GetComponent<Line>();
            if (line != null)
                continue;

            BodyPartVisibility isVisible = child.GetComponent<BodyPartVisibility>();

            if (child.CompareTag("Insertions"))
                continue;

            selectedObjects.Add(child.gameObject);

            if(isVisible != null)
            {
                isVisible.isVisible = true;
                isVisible.isSelected = true;
            }

            TangibleBodyPart script = child.GetComponent<TangibleBodyPart>();
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

    ///<summary>
    ///Handles click event to select the parent of the current selected object, and show its children objects in the scene.
    ///</summary>
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

    /// <summary>
    /// Checks the visibility of labels for all the active objects, and hides them if there is more than one object active.
    /// </summary>
    public void CheckLabelsVisibility()
    {
        GetActiveObjects();
        if (activeObjects.Count <= 1)
            return;
        foreach (var item in activeObjects)
        {
            BodyPartVisibility isVisible = item.GetComponent<BodyPartVisibility>();
            if (item != null && isVisible != null)
                isVisible.HideLabels();
        }
    }

    /// <summary>
    /// Shows the body part information of a given game object. Sets the description of the object using its <see cref="NameAndDescription"/> component. Sets the hierarchy bar to the object's transform. Expands the object in the lexicon.
    /// </summary>
    /// <param name="go">The game object to show the body part information for.</param>
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

    /// <summary>
    /// Clear the hierarchy bar and highlighted item in the lexicon.
    /// </summary>
    public void NoBodyPartInfo()
    {
        //Set the description
        //NamesManagement.instance.SetDesc(NamesManagement.NO_SELECTION);
        //Set the hierarchy bar
        HierarchyBar.Instance.Clear();
        Lexicon.Instance.SetHighlighted(null);
    }

    /// <summary>
    /// Inverts the current selection by selecting all objects that are not currently selected, and deselecting all objects that are currently selected.
    /// </summary>
    public void InvertSelection()
    {
        List<GameObject> toSelect = activeObjects.Where(it => !selectedObjects.Contains(it)).ToList();
        DeselectAllObjects();
        ActionControl.Instance.AddCommand(new SelectCommand(toSelect), true);
    }
}
