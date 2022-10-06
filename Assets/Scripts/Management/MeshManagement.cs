using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Diagnostics;
using Assets.Scripts.Commands;

public class MeshManagement : MonoBehaviour
{
    public static MeshManagement Instance;
    private SelectedObjectsManagement selectedObjectsManagement;
    [HideInInspector]
    public Dictionary<int, Material[]> rendererMaterials = new Dictionary<int, Material[]>();
    private Label[] labels;
    private void Awake()
    {
        Instance = this;   
    }

    private void Start()
    {
        selectedObjectsManagement = GetComponent<SelectedObjectsManagement>();
        Color col = Color.white;

       
        int i = 0;

        //Hide body sections
        foreach (Transform section in GlobalVariables.Instance.globalParent.transform)
        {
            if (section.CompareTag("Skeleton"))
                section.transform.SetActiveRecursively(true);
            else
                section.transform.SetActiveRecursively(false);
        }

        //Hide insertions
        foreach (var item in GlobalVariables.Instance.insertions)
        {
            item.gameObject.SetActive(false);
            item.GetComponent<Visibility>().isVisible = false;
        }

        //Get all renderers and assign labels
        i = 0;
        foreach (var renderer in GlobalVariables.Instance.allBodyPartRenderers)
        {
            rendererMaterials[i] = renderer.sharedMaterials;
            i++;
            Visibility script = renderer.GetComponent<Visibility>();
        }

        HideAllLabels();

        //Set default shader params
        for (i = 0; i < rendererMaterials.Count; i++)
        {
            foreach (Material material in rendererMaterials[i])
            {
                if (material == null)
                    continue;
                material.SetVector("_PlanePosition", new Vector3(0, 0, 0));
                material.SetVector("_PlaneNormal", transform.up);
                material.SetFloat("_PlaneEnabled", 1f);
            }
        }

        foreach (var item in GlobalVariables.Instance.globalParent.GetComponentsInChildren<Visibility>(true))
            GetInsertionsInChild(item);

        SelectedObjectsManagement.Instance.GetActiveObjects();
        Lexicon.Instance.UpdateTreeViewCheckboxes();
    }

    private void GetInsertionsInChild(Visibility visibilityScript)
    {
        if (!visibilityScript.CompareTag("Muscles"))
            return;

        foreach (var item in visibilityScript.GetComponentsInChildren<Visibility>(true))
        {
            if (item != visibilityScript && item.HasInsertions())
            {
                visibilityScript.insertions.AddRange(item.insertions);
            }
        }

    }

    public void PartiallyDeleteNotSelected()
    {
        int i = 0;
        List<GameObject> toDelete = new List<GameObject>();
        foreach (var renderer in GlobalVariables.Instance.allBodyPartRenderers)
        {
            if (selectedObjectsManagement.selectedObjects.Count > 0 && !selectedObjectsManagement.selectedObjects.Contains(renderer.gameObject) && renderer.CompareTag(selectedObjectsManagement.selectedObjects[0].tag))
            {
                toDelete.Add(renderer.gameObject);
            }
            i++;
        }
        ActionControl.Instance.AddCommand(new DeleteCommand(toDelete), false);
        selectedObjectsManagement.DeleteList(toDelete);
    }

    public void IsolationClick(bool showLabels = true)
    {
        // - The selected objects are the same as isolated objects
        bool case1 = SelectedObjectsManagement.Instance.selectedObjects.SequenceEqual(SelectedObjectsManagement.Instance.lastIsolatedObjects);
        bool case2 = false;
        bool case3 = false;

        bool onlyOneIsolated = SelectedObjectsManagement.Instance.lastIsolatedObjects.Where(it => it.IsBodyPart()).Count() == 1;

        if(!case1 && onlyOneIsolated)
        {
            GameObject lastIsolated = SelectedObjectsManagement.Instance.lastIsolatedObjects.Find(it => it.IsBodyPart());

            // - Or there's only one bodypart selected and it's the same as the prev one
            case2 = onlyOneIsolated
                && SelectedObjectsManagement.Instance.selectedObjects.Where(it => it.IsBodyPart()).Count() == 1
                && lastIsolated == SelectedObjectsManagement.Instance.selectedObjects.Find(it => it.IsBodyPart());

            if(!case2)
            {
                // - Or there's no selected object and the active is the isolated one
                case3 = SelectedObjectsManagement.Instance.selectedObjects.Count == 0
                    && onlyOneIsolated
                    && SelectedObjectsManagement.Instance.activeObjects.Where(it => it.IsBodyPart()).Count() == 1
                    && lastIsolated == SelectedObjectsManagement.Instance.activeObjects.Find(it => it.IsBodyPart());
            }
        }


        //Can undo isolation?
        bool canUndoIsolation = case1 || case2 || case3;

        if (canUndoIsolation)
        {
            ActionControl.Instance.Undo();
            if(case3)
                ActionControl.Instance.Undo();

            SelectedObjectsManagement.Instance.lastIsolatedObjects.Clear();
            Lexicon.Instance.UpdateTreeViewCheckboxes();

            return;
        }

        if(SelectedObjectsManagement.Instance.selectedObjects.Count == 1)
        {
            //IF it is label, we select its parent too
            Label label = SelectedObjectsManagement.Instance.selectedObjects[0].GetComponent<Label>();
            if (label != null)
            {
                if(!SelectedObjectsManagement.Instance.selectedObjects.Contains(label.parent.gameObject))
                {
                    SelectedObjectsManagement.Instance.SelectObject(label.parent.gameObject);
                    ActionControl.Instance.AddCommand(new SelectCommand(SelectedObjectsManagement.Instance.selectedObjects), false);
                }
            }
        }

        SelectedObjectsManagement.Instance.lastIsolatedObjects = new List<GameObject>(SelectedObjectsManagement.Instance.selectedObjects);
        ActionControl.Instance.AddCommand(new DeleteCommand(NotSelected(), SelectedObjectsManagement.Instance.selectedObjects, showLabels), true);

        foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
            item.transform.SetActiveParentsRecursively(true);

        SelectedObjectsManagement.Instance.GetActiveObjects();
        SelectedObjectsManagement.Instance.DeleteOutlineToActiveObjects();

        ActionControl.Instance.UpdateButtons();
        Lexicon.Instance.UpdateTreeViewCheckboxes();
    }

    public void PartialIsolationClick()
    {

        //Can undo isolation
        if (SelectedObjectsManagement.Instance.selectedObjects.SequenceEqual(SelectedObjectsManagement.Instance.lastIsolatedObjects))
        {
            ActionControl.Instance.Undo();
            SelectedObjectsManagement.Instance.lastIsolatedObjects.Clear();
            return;
        }

        SelectedObjectsManagement.Instance.lastIsolatedObjects = new List<GameObject>(SelectedObjectsManagement.Instance.selectedObjects);

        PartiallyDeleteNotSelected();
        selectedObjectsManagement.DeleteOutlineToSelectedObjects();
        foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
        {
            item.transform.SetActiveParentsRecursively(true);
            item.gameObject.layer = 6;
        }
        Lexicon.Instance.UpdateTreeViewCheckboxes();

    }

    private List<GameObject> NotSelected()
    {
        List<GameObject> toDelete = new List<GameObject>();
        foreach (Visibility script in GlobalVariables.Instance.allVisibilityScripts)
        {
            if (!selectedObjectsManagement.selectedObjects.Contains(script.gameObject) && script.gameObject.activeSelf)
                toDelete.Add(script.gameObject);
        }
        return toDelete;
    }

    public void EnableInsertions()
    {
        List<GameObject> shown = new List<GameObject>();
        foreach (var insertion in GlobalVariables.Instance.insertions)
        {
            if (!insertion.gameObject.activeInHierarchy)
                shown.Add(insertion.gameObject);
        }
        ActionControl.Instance.AddCommand(new ShowCommand(shown), true);
    }

    public void DisableInsertions()
    {
        List<GameObject> hidden = new List<GameObject>();
        foreach (var insertion in GlobalVariables.Instance.insertions)
        {
            if (insertion.gameObject.activeInHierarchy)
                hidden.Add(insertion.gameObject);
        }
        ActionControl.Instance.AddCommand(new DeleteCommand(hidden), true);
    }

    public void HideAllLabels()
    {
        foreach (var child in GlobalVariables.Instance.allBodyParts)
        {
            if (child != null)
                child.GetComponent<Visibility>().HideLabels();
        }
    }
}
