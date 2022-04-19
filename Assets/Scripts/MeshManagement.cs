using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Diagnostics;

[System.Serializable]
public class MaterialGroup
{
    public string groupColor;
    public GameObject[] groupBodyParts;
}

public class MeshManagement : MonoBehaviour
{
    public static MeshManagement instance;
    public GameObject globalParent;
    public GameObject[] bodySections;
    public ReflectionProbe reflectionProbe;
    /*public Material[] colorMaterials;
    public MaterialGroup[] materialGroups;*/

    [Header("UI elements")]

    public Toggle eraseToggle;
    public Slider brushSelectionSlider;
    [HideInInspector]
    public List<MeshRenderer> bodyPartsRenderers;
    private SelectedObjectsManagement selectedObjectsManagement;

    [HideInInspector]
    public Dictionary<int, Material[]> rendererMaterials = new Dictionary<int, Material[]>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        selectedObjectsManagement = GetComponent<SelectedObjectsManagement>();
        Color col = Color.white;
        bodyPartsRenderers = globalParent.GetComponentsInChildren<MeshRenderer>().Where(it => it.GetComponent<Label>() == null &&  !it.gameObject.name.Contains(".g") && !it.gameObject.name.Contains(".labels")).ToList();

        List<BodyPart> transforms = globalParent.GetComponentsInChildren<BodyPart>()
            .OrderBy(it => it.name)
            .ToList();

        Dictionary<string, int> duplicatesDictionary = transforms
            .GroupBy(x => x.name)
            .ToDictionary(x => x.Key, y => y.Count());

        //Set Left or right name
        foreach (var item in duplicatesDictionary)
        {
            int toDeleteCount = item.Value;

            List<BodyPart> ordered = transforms
                .GetRange(0, item.Value)
                .OrderBy(it => it.transform.position.x)
                .ToList();

            if (item.Value > 1 && item.Value % 2 == 0)
            {
                for (int j = 0; j < item.Value; j++)
                {
                    NameAndDescription obj = ordered[j].GetComponent<NameAndDescription>();
                    if(obj != null && !obj.leftRight.Contains("(R)") && !obj.leftRight.Contains("(L)"))
                        obj.SetRightLeftName(j < item.Value / 2);
                }
            }
            else if(item.Value > 1)
            {

                Dictionary<string, int> orderedByTag = ordered
                    .GroupBy(it => it.tag)
                    .ToDictionary(x => x.Key, y => y.Count());

                foreach (var item_ in orderedByTag)
                {
                    if(item_.Value % 2 == 0)
                    {
                        for (int j = 0; j < item.Value; j++)
                        {
                            NameAndDescription obj = ordered[j].GetComponent<NameAndDescription>();
                            if (obj != null && !obj.leftRight.Contains("(R)") && !obj.leftRight.Contains("(L)"))
                                obj.SetRightLeftName(j < item.Value / 2);
                        }
                    }
                    else
                    {
                        transforms.RemoveRange(0, item_.Value);
                        toDeleteCount -= item_.Value;
                    }
                }
            }
            transforms.RemoveRange(0, toDeleteCount);
        }

        int i = 0;

        //Hide body sections
        foreach (var section in bodySections)
        {
            if (!section.CompareTag("Skeleton"))
            {
                StaticMethods.SetActiveRecursively(section.transform, false);

            }
        }

        //Get all renderers and assign labelss
        i = 0;
        foreach (var renderer in bodyPartsRenderers)
        {
            rendererMaterials[i] = (Material[])bodyPartsRenderers[i].sharedMaterials;
            i++;
            IsVisible script = renderer.GetComponent<IsVisible>();
            foreach (Transform child in renderer.transform)
            {
                if (child.name.Contains(".labels") && script != null)
                {
                    script.labels = child.gameObject;
                    script.HideLabels();
                    child.gameObject.SetActive(false);
                    break;
                } 
            }
        }

        //Set default shader params
        for (i = 0; i < rendererMaterials.Count; i++)
        {
            foreach (Material material in rendererMaterials[i])
            {
                material.SetVector("_PlanePosition", new Vector3(0, 0, 0));
                material.SetVector("_PlaneNormal", transform.up);
            }
        }

        SelectedObjectsManagement.instance.GetActiveObjects();
    }

    public void DeleteNotSelected()
    {
        int i = 0;
        List<GameObject> toDelete = new List<GameObject>();
        foreach (var renderer in bodyPartsRenderers)
        {
            if (!selectedObjectsManagement.selectedObjects.Contains(renderer.gameObject) && renderer.gameObject.activeSelf)
            {
                toDelete.Add(renderer.gameObject);
            }
            i++;
        }
        //selectedObjectsManagement.DeselectAllObjects();
        if(toDelete.Count > 0)
            selectedObjectsManagement.DeleteList(toDelete);
    }

    public void PartiallyDeleteNotSelected()
    {
        int i = 0;
        List<GameObject> toDelete = new List<GameObject>();
        foreach (var renderer in bodyPartsRenderers)
        {
            if (!selectedObjectsManagement.selectedObjects.Contains(renderer.gameObject) && renderer.CompareTag(selectedObjectsManagement.selectedObjects[0].tag))
            {
                toDelete.Add(renderer.gameObject);
            }
            i++;
        }
        selectedObjectsManagement.DeleteList(toDelete);
    }

    public void IsolationClick()
    {
        if(SelectedObjectsManagement.instance.canUndoIsolation)
        {
            IsVisible isolatedObjectScript = SelectedObjectsManagement.instance.selectedObjects[0].GetComponent<IsVisible>();
            if (isolatedObjectScript != null)
                isolatedObjectScript.HideLabels();
            SelectedObjectsManagement.instance.UndoDelete();
            SelectedObjectsManagement.instance.canUndoIsolation = false;
        }
        else
        {
            SelectedObjectsManagement.instance.canUndoIsolation = true;
            if (ActionControl.someObjectSelected)
            {
                if(SelectedObjectsManagement.instance.selectedObjects.Count == 1)
                {
                    //IF it is label, we select it's parent too
                    Label label = SelectedObjectsManagement.instance.selectedObjects[0].GetComponent<Label>();
                    if (label != null)
                        SelectedObjectsManagement.instance.SelectObject(label.transform.parent.parent.gameObject);

                }
                DeleteNotSelected();
                foreach (var item in SelectedObjectsManagement.instance.selectedObjects)
                    StaticMethods.SetActiveParentsRecursively(item.transform, true);
                SelectedObjectsManagement.instance.GetActiveObjects();
                SelectedObjectsManagement.instance.DeleteOutlineToActiveObjects();
                GameObject isolated = SelectedObjectsManagement.instance.selectedObjects[0];
                NameAndDescription nameScript = isolated.GetComponent<NameAndDescription>();
                //Heart is a special case where it's labels are not atached to a body part but a group.
                if (SelectedObjectsManagement.instance.activeObjects.Count == 1 || isolated.transform.Find(nameScript.originalName + ".g.labels") != null)
                {
                    IsVisible isolatedObjectScript = isolated.GetComponent<IsVisible>();
                    //If it is a bodypart, show it's labels
                    if(isolatedObjectScript != null)
                    {
                        isolatedObjectScript.ShowLabels();
                    }
                    //If it is a group
                }
                //SelectedObjectsManagement.instance.DeselectAllObjects();
                //selectedObjectsManagement.DeleteOutlineToSelectedObjects();
                // labelManagement.HideGlobalLabelsAndLines();
            }
        }
        RefreshReflections();
    }

    public void PartialIsolationClick()
    {
        if (SelectedObjectsManagement.instance.canUndoIsolation)
        {
            SelectedObjectsManagement.instance.UndoDelete();
            SelectedObjectsManagement.instance.canUndoIsolation = false;
        }
        else
        {
            SelectedObjectsManagement.instance.canUndoIsolation = true;
            if (ActionControl.someObjectSelected)
            {
                PartiallyDeleteNotSelected();
                selectedObjectsManagement.DeleteOutlineToSelectedObjects();
                foreach (var item in SelectedObjectsManagement.instance.selectedObjects)
                {
                    StaticMethods.SetActiveParentsRecursively(item.transform, true);
                    item.gameObject.layer = 6;
                }
                //  labelManagement.HideGlobalLabelsAndLines();
                //selectedObjectsManagement.DeselectAllObjects();
            }
        }
        RefreshReflections();
    }

    public void RefreshReflections() => reflectionProbe.RenderProbe();
    /* public void SetColors(string bodySectionTag)
     {
         int i = 0;
         foreach (var renderer in skeletonRenderers)
         {
             if(renderer.gameObject.CompareTag(bodySectionTag))
             {
                 Material[] coloredMaterials = skeletonRenderers[i].sharedMaterials;
                 for (int j = 0; j < coloredMaterials.Length; j++)
                 {
                     coloredMaterials[j] = colorMaterials[UnityEngine.Random.Range(0, colorMaterials.Length)];
                 }
                 renderer.sharedMaterials = coloredMaterials;
             }
             i++;
         }
     }*/

    /*   public void SetNormalColorByTag(string bodySectionTag)
       {
           int i = 0;
           foreach (var renderer in skeletonRenderers)
           {
               if (renderer.gameObject.CompareTag(bodySectionTag))
               {
                   renderer.sharedMaterials = rendererMaterials[i];
               }
               i++;
           }
       }*/
}
