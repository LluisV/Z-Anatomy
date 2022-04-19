using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HierarchyButton : MonoBehaviour
{

    private Transform group;
    private TextMeshProUGUI text;
    private Collections_1 collectionsScript;
    public bool isRoot;
    // Start is called before the first frame update
    void Start()
    {
        collectionsScript = FindObjectOfType<Collections_1>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        group = collectionsScript.actualRoot;
        if(!isRoot)
        {
            int indexOfPoint = group.name.IndexOf('.');
            string cleanName = group.name;
            if (indexOfPoint != -1)
                cleanName = group.name.Substring(0, indexOfPoint);
            text.text = cleanName;
        }
    }

    public void CollectionClicked()
    {
        collectionsScript.HierarchyClick(group, transform.GetSiblingIndex());
        foreach (Transform item in transform.parent)
        {
            if(item.GetSiblingIndex() > transform.GetSiblingIndex())
            {
                Destroy(item.gameObject);
            }
        }
    }
}
