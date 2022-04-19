using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System.Diagnostics;
using System.Text;
using System.Globalization;

public class TreeViewSearchEngine : MonoBehaviour
{
    private List<GameObject> gameObjectFound = new List<GameObject>();
    private int lastLenght = 0;
    public TMP_InputField mainInputField;
    private MeshManagement meshManagement;
    TreeViewCanvas treeViewCanvas;
    public static bool onSearch;
    public static TreeViewSearchEngine instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        mainInputField.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        meshManagement = GetComponent<MeshManagement>();
        treeViewCanvas = GetComponent<TreeViewCanvas>();
    }

    public void ValueChangeCheck()
    {
        gameObjectFound.Clear();
        if (mainInputField.text.Length >= 3 && mainInputField.text.Length > lastLenght)
        {
            onSearch = true;
            RecursiveStartWithChild(meshManagement.globalParent.transform, mainInputField.text);
            RecursiveContainsChild(meshManagement.globalParent.transform, mainInputField.text);

            if (gameObjectFound.Count > 0)
            {
                treeViewCanvas.InstantiateRoots(gameObjectFound, true);
            }
        }
        else if(mainInputField.text.Length < 3 && onSearch)
        {
            treeViewCanvas.Reset();
            onSearch = false;
        }
        lastLenght = mainInputField.text.Length;
    }
    void RecursiveStartWithChild(Transform parent, string childName)
    {
        childName = childName.ToLower();
        foreach (Transform child in parent)
        {
            string childN = StaticMethods.RemoveAccents(child.name.ToLower());
            //   if (child.gameObject.activeInHierarchy)
            {
                if (childN.StartsWith(StaticMethods.RemoveAccents(childName)))
                {
                    if (!gameObjectFound.Contains(child.gameObject))
                    {
                        gameObjectFound.Add(child.gameObject);
                    }

                    //return child;
                }
                if (child.childCount > 0)
                {
                   RecursiveStartWithChild(child, childName);
                }
            }
        }

    }

    void RecursiveContainsChild(Transform parent, string childName)
    {
        childName = childName.ToLower();
        foreach (Transform child in parent)
        {
            string childN = StaticMethods.RemoveAccents(child.name.ToLower());
            //   if (child.gameObject.activeInHierarchy)
            {
                if (childN.Contains(StaticMethods.RemoveAccents(childName)))
                {
                    if (!gameObjectFound.Contains(child.gameObject))
                    {
                        gameObjectFound.Add(child.gameObject);
                    }

                    //return child;
                }
                if (child.childCount > 0)
                {
                    RecursiveContainsChild(child, childName);
                }
            }

        }

    }

    private void DestroyChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }


    public void SetText(string text)
    {
        mainInputField.text = text;
    }

}
