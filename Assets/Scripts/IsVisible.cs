using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsVisible : MonoBehaviour
{
    public bool isVisible;
    [HideInInspector]
    public GameObject labels;

    private void Start()
    {
        Transform labelsTrans = transform.Find(name + ".labels");
        if (labelsTrans != null)
            labels = labelsTrans.gameObject;
        if (CompareTag("Skeleton"))
            isVisible = true;

    }

    public void SetLabelVisibility()
    {
        Label labelScript = GetComponent<Label>();
        if (labelScript != null && !name.Contains(".t"))
            name += ".t";
        if (name.Contains(".t"))
        {
            isVisible = false;
        }
    }


    public void ShowLabels()
    {
        if (labels != null)
        {
            labels.SetActive(true);
            foreach (Transform label in labels.transform)
            {
                label.gameObject.SetActive(true);
                IsVisible isVisibleScript = label.GetComponent<IsVisible>();
                if (isVisibleScript != null)
                    isVisibleScript.isVisible = true;
            }
        }
    }

    public void HideLabels()
    {
        if (labels != null)
        {
            foreach (Transform label in labels.transform)
            {
                label.gameObject.SetActive(false);
                IsVisible isVisibleScript = label.GetComponent<IsVisible>();
                if (isVisibleScript != null)
                    isVisibleScript.isVisible = false;
            }
            labels.SetActive(false);
        }
    }
}
