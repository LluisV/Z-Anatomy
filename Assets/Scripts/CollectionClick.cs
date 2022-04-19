using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionClick : MonoBehaviour
{
    private Collections_1 collectionsScript;
    [HideInInspector]
    public Transform collection;

    private void Start()
    {
        collectionsScript = FindObjectOfType<Collections_1>();
    }
    public void CollectionClicked()
    {
        collectionsScript.CollectionsClick(collection);
    }
}
