using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SelectedObjectsManagement : MonoBehaviour
{
    public static SelectedObjectsManagement instance;
    [HideInInspector]
    public Stack<GameObject[]> deletedObjects = new Stack<GameObject[]>();
    [HideInInspector]
    public List<GameObject> selectedObjects = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> activeObjects = new List<GameObject>();
    public CameraController cam;
    public Button deleteSelectionBtn;
    public Button undoBtn;
    public Button upHierarchyBtn;
    public Button partialIsonationBtn;
    public Button[] buttonsEnabledOnObjectSelected;
    [HideInInspector]
    public bool canUndoIsolation;
    private Transform lastParentSelected;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        EnableDisableButtons();
        GetActiveObjects();
    }

    public void GetActiveObjects()
    {
        //Optimize this
        activeObjects.Clear();
        List<MeshRenderer> activeRenderers = MeshManagement.instance.bodyPartsRenderers.Where(it => it.gameObject.activeInHierarchy).ToList();
        foreach (MeshRenderer item in activeRenderers)
        {
            activeObjects.Add(item.gameObject);
        }
    }
    public void DeleteOutlineToActiveObjects()
    {
        foreach (var selected in activeObjects)
        {
            //NORMAL MASK
            selected.layer = 6;
        }
    }
    public void DeleteOutlineToSelectedObjects()
    {
        foreach (var selected in selectedObjects)
        {
            //NORMAL MASK
            selected.layer = 6;
        }
    }
    public void SetOutlineToSelectedObjects()
    {
        foreach (var selected in selectedObjects)
        {
            //OUTLINE MASK
            selected.layer = 7;
        }
    }

    public void DeleteClicked(GameObject go)
    {
        GameObject[] deletedGo = new GameObject[1];
        deletedGo[0] = go;
        IsVisible isVisibleScript = go.GetComponent<IsVisible>();
        if (isVisibleScript != null)
            isVisibleScript.isVisible = false;
        deletedObjects.Push(deletedGo);
        undoBtn.interactable = true;
        go.SetActive(false);
        DeselectObjet(go);
        TreeViewCanvas.instance.UpdateTreeViewCheckboxes();
        EnableDisableButtons();
        SetOutlineVisibility();
        MeshManagement.instance.RefreshReflections();
    }

    public void DeleteSelected()
    {
        GameObject[] deletedArray = selectedObjects.ToArray();
        deletedObjects.Push(deletedArray);
        foreach (var item in selectedObjects)
        {
            IsVisible isVisibleScript = item.GetComponent<IsVisible>();
            if (isVisibleScript != null)
                isVisibleScript.isVisible = false;
            undoBtn.interactable = true;
            item.SetActive(false);
        }
        DeselectAllObjects();
        EnableDisableButtons();
        TreeViewCanvas.instance.UpdateTreeViewCheckboxes();
        EnableDisableButtons();
        SetOutlineVisibility();
        MeshManagement.instance.RefreshReflections();
    }

    public void DeleteList(List<GameObject> deletedArray)
    {
        deletedObjects.Push(deletedArray.ToArray());
        foreach (var item in deletedArray)
        {
            IsVisible isVisibleScript = item.GetComponent<IsVisible>();
            if(isVisibleScript != null)
            {
                isVisibleScript.isVisible = false;
            }
            undoBtn.interactable = true;
            item.SetActive(false);
            DeselectObjet(item);
        }
        TreeViewCanvas.instance.UpdateTreeViewCheckboxes();
        EnableDisableButtons();
        SetOutlineVisibility();
        MeshManagement.instance.RefreshReflections();
    }

    public void DeselectObjet(GameObject selected)
    {
        BodyPart script = selected.GetComponent<BodyPart>();
        IsVisible isVisible = selected.GetComponent<IsVisible>();
        if ((script != null && script.isSelected) || selected.GetComponent<Label>() != null)
        {
            selected.layer = 6;
            selectedObjects.Remove(selected);
            canUndoIsolation = false;
            if(script != null)
                script.isSelected = false;
            if (isVisible != null)
                isVisible.HideLabels();
            SetOutlineVisibility();
            ActionControl.someObjectSelected = selectedObjects.Count > 0;
            if (selectedObjects.Count == 0)
            {
                NamesManagement.instance.SetTitle("");
                NamesManagement.instance.SetDescription("");
            }
        }
    }

    public void DeselectAllObjects()
    {
        DeleteOutlineToSelectedObjects();
        foreach (var item in selectedObjects)
        {
            BodyPart script = item.GetComponent<BodyPart>();
            IsVisible isVisible = item.GetComponent<IsVisible>();
            if(isVisible != null)
                isVisible.HideLabels();
            if (script != null)
                script.isSelected = false;
        }
        selectedObjects.Clear();
        deleteSelectionBtn.interactable = false;
        ActionControl.someObjectSelected = false;
        SetOutlineVisibility();
        NamesManagement.instance.SetTitle("");
        NamesManagement.instance.SetDescription("");
        canUndoIsolation = false;
    }

    public void UndoAll()
    {
        int lent = deletedObjects.Count;
        for (int i = 0; i < lent; i++)
        {
            GameObject[] deletedGameObjets = deletedObjects.Pop();
            foreach (var item in deletedGameObjets)
            {
                IsVisible isVisibleScript = item.GetComponent<IsVisible>();
                if (isVisibleScript != null)
                    isVisibleScript.isVisible = true;
                item.SetActive(true);
            }
        }
        undoBtn.interactable = false;
        TreeViewCanvas.instance.UpdateTreeViewCheckboxes();
        EnableDisableButtons();
        SetOutlineVisibility();
        MeshManagement.instance.RefreshReflections();
        canUndoIsolation = false;
    }

    public void SelectObject(GameObject selected)
    {
        canUndoIsolation = false;
        //selected.SetActive(true);
        ActionControl.someObjectSelected = true;
        selectedObjects.Add(selected);
        deleteSelectionBtn.interactable = true;
        BodyPart script = selected.GetComponent<BodyPart>();
        IsVisible isVisible = selected.GetComponent<IsVisible>();
        //OUTLINE MASK
        selected.layer = 7;
        //SHOW LABELS
        if (script != null)
        {
            script.isSelected = true;
            if (activeObjects.Count == 1)
            {
                isVisible.ShowLabels();
            }
        }
        SetOutlineVisibility();
    }


  /*  private void HideAllLabels()
    {
        foreach (BodyPart selected in activeLabels)
        {
            selected.HideLabels();
        }
        activeLabels.Clear();
    }*/

    public void UndoDelete()
    {
        if (deletedObjects.Count == 0)
            return;
        GameObject[] deletedGameObjets = deletedObjects.Pop();
        foreach (var item in deletedGameObjets)
        {
            IsVisible isVisibleScript = item.GetComponent<IsVisible>();
            if (isVisibleScript != null)
                isVisibleScript.isVisible = true;
            item.SetActive(true);
        }
        //Hide labels
        foreach (var item in activeObjects)
        {
            if (item.name.Contains(".t") || item.name.Contains("-lin"))
            {
                IsVisible script = item.GetComponent<IsVisible>();
                if (script != null)
                    script.isVisible = false;
                item.SetActive(false);
            }
        }
        undoBtn.interactable = deletedObjects.Count > 0;
        TreeViewCanvas.instance.UpdateTreeViewCheckboxes();
        EnableDisableButtons();
        SetOutlineToSelectedObjects();
        SetOutlineVisibility();
        MeshManagement.instance.RefreshReflections();
        canUndoIsolation = false;
    }


    public void SelectAllChildren(Transform parent, bool lastIteration = true)
    {
        foreach (Transform child in parent)
        {
            if (child.GetComponent<Label>() == null && child.GetComponent<Line>() == null && !child.name.Contains("-lin"))
            {
                SelectObject(child.gameObject);
                child.gameObject.SetActive(true);
            }

            if (child.childCount > 0)
                SelectAllChildren(child, false);
        }

        //Delete duplicates
        if(lastIteration)
        {
            selectedObjects = selectedObjects.Distinct().ToList();
            SetOutlineVisibility();
        }
    }
    
    public void DeselectAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            DeselectObjet(child.gameObject);

            if (child.childCount > 0)
                DeselectAllChildren(child);
        }
    }

    public void EnableDisableButtons()
    {
        //Optimize this (Check which tags are active)
        bool isParentDisabled = !MeshManagement.instance.globalParent.activeInHierarchy;
        if (isParentDisabled)
            MeshManagement.instance.globalParent.SetActive(true);

        GetActiveObjects();

        List<string> selectedTags = new List<string>();
        foreach (var item in selectedObjects)
        {
            if (!selectedTags.Contains(item.tag))
            {
                selectedTags.Add(item.tag);
            }
        }
        List<string> actualTags = new List<string>();
        foreach (var item in activeObjects)
        {
            if (!actualTags.Contains(item.tag))
            {
                actualTags.Add(item.tag);
            }
        }
        partialIsonationBtn.interactable = selectedTags.Count == 1 && actualTags.Count > 1 && selectedObjects.Count > 0;
        foreach (Button btn in buttonsEnabledOnObjectSelected)
        {
            btn.interactable = selectedObjects.Count > 0;
        }
        if (selectedObjects.Count == 1)
            lastParentSelected = null;
        upHierarchyBtn.interactable = lastParentSelected != null || selectedObjects.Count == 1;

        if (isParentDisabled)
            MeshManagement.instance.globalParent.SetActive(false);
           
    }

    public void UpHierarchyClick()
    {
        if (lastParentSelected == null)
            lastParentSelected = selectedObjects[selectedObjects.Count - 1].transform.parent;
        else
            lastParentSelected = lastParentSelected.parent;

        //Jump the label's parent
        if (lastParentSelected.gameObject.transform.name.Contains(".labels"))
            lastParentSelected = lastParentSelected.parent;


        //DeselectAllObjects();
        lastParentSelected.gameObject.SetActive(true);
        NamesManagement.instance.SetTitle(lastParentSelected.name.Replace("@", ""));
        NameAndDescription descriptionScript = lastParentSelected.GetComponent<NameAndDescription>();
        SelectObject(lastParentSelected.gameObject);
        SelectAllChildren(lastParentSelected);
        EnableDisableButtons();
        if (descriptionScript != null)
            NamesManagement.instance.SetDescription(descriptionScript.Description(), lastParentSelected.gameObject);
        cam.CenterView(true);
    }

    private void SetOutlineVisibility()
    {
        Settings.instance.SetOutline(selectedObjects.Count > 0);
    }
}
