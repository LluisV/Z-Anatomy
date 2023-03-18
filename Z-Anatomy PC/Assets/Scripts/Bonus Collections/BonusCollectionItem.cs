using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BonusCollectionItem : MonoBehaviour
{
    private TextMeshProUGUI tmpro;
    private int collectionIndex;
    private Transform hierarchyParent;
    private string collectionName;
    private ContextualMenu menu;

    private void Awake()
    {
        menu = GetComponentInParent<ContextualMenu>();
    }

    public void Init(string name, int index)
    {
        collectionIndex = index;
        collectionName = name;
        tmpro = GetComponentInChildren<TextMeshProUGUI>();
        tmpro.text = collectionName;
    }

    public void Init(string name, Transform parent)
    {
        hierarchyParent = parent;
        collectionName = name;
        tmpro = GetComponentInChildren<TextMeshProUGUI>();
        tmpro.text = collectionName;
    }

    public void Click()
    {
        if(hierarchyParent != null)
            BonusCollections.Instance.ShowCollection(hierarchyParent);
        else
            BonusCollections.Instance.ShowCollection(collectionIndex);

        menu.bonusCollectionPanel.gameObject.SetActive(false);
        menu.gameObject.SetActive(false);
    }

}
