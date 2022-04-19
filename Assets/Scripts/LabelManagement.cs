using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LabelManagement : MonoBehaviour
{
    private MeshManagement meshManagement;
    private SelectedObjectsManagement selectedObjects;
    private List<Transform> globalLabels;
    private List<Transform> globalLabelsLines = new List<Transform>();

    private void Start()
    {
        meshManagement = GetComponent<MeshManagement>();
        selectedObjects = GetComponent<SelectedObjectsManagement>();
        globalLabels = meshManagement.globalParent.GetComponentsInChildren<Transform>().Where(it => it.gameObject.name.Contains(".gLabel")).ToList();

        foreach (var label in globalLabels)
        {
            Transform line = label.transform.parent.Find(label.name.Replace(".gLabel", "-line"));
            if (line != null)
                globalLabelsLines.Add(line);
        }

        HideGlobalLabelsAndLines();
    }

    public void HideGlobalLabelsAndLines()
    {
        foreach (var item in globalLabels)
        {
            item.gameObject.SetActive(false);
        }
        foreach (var item in globalLabelsLines)
        {
            item.gameObject.SetActive(false);
        }
    }

    public void ShowGlobalLabelsAndLines()
    {
        foreach (var item in globalLabels)
        {
            item.gameObject.SetActive(true);
        }
        foreach (var item in globalLabelsLines)
        {
            item.gameObject.SetActive(true);
        }
    }

    public void ShowLabels()
    {
        foreach (var item in selectedObjects.selectedObjects)
        {
            item.GetComponent<IsVisible>().ShowLabels();
        }
    }

    public void HideLabels()
    {
        foreach (var item in selectedObjects.selectedObjects)
        {
            item.GetComponent<IsVisible>().HideLabels();
        }
    }
}
