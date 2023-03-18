using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BonusCollections : MonoBehaviour
{
    public static BonusCollections Instance;

    public List<TextAsset> collections;
    private List<string[]> collectionsNames;

    private HashSet<string>[] collectionParts;

    public RectTransform collectionsPanel;
    public GameObject collectionPrefab;

    private void Awake()
    {
        Instance = this;

        collectionParts = new HashSet<string>[collections.Count];

        //Foreach collection
        for (int i = 0; i < collections.Count; i++)
        {
            collectionParts[i] = new HashSet<string>();
            var splittedText = collections[i].text.Split('\n');
            //Foreach part
            foreach (var part in splittedText)
            {
                collectionParts[i].Add(part.Replace("\r", ""));
            }
        }
    }

    public bool SetCollections(string name, Transform obj)
    {
        collectionsNames = new List<string[]>();
        foreach (var collection in collections)
            collectionsNames.Add(NamesManagement.Instance.GetWordTranslations(collection.name));

        bool result = false;
        int i = 0;
        foreach (var collection in collectionParts)
        {
            if (collection.Contains(name))
            {
                if (collectionsNames[i] != null)
                    AddCollection(collectionsNames[i][Settings.languageIndex], i);
                else
                    AddCollection(collections[i].name, i);


                result = true;
            }
            i++;
        }

        obj = obj.parent;

        if(result && obj.GetComponent<BodyPartVisibility>() != null)
            AddSeparator();

        //Get all parents
        while (obj != null)
        {
            if (obj.GetComponent<BodyPartVisibility>() != null)
            {
                AddCollection(obj.name, obj.transform);
                result = true;
            }
            obj = obj.transform.parent;
            i++;
        }

        return result;
    }

    private void AddSeparator()
    {
        GameObject imgObject = new GameObject("Separator");
        RectTransform trans = imgObject.AddComponent<RectTransform>();

        trans.transform.SetParent(collectionsPanel);
        trans.localScale = Vector3.one;
        trans.anchoredPosition = new Vector2(0f, 0f);
        trans.SetHeight(1);
        trans.SetWidth(collectionsPanel.GetWidth() - 20);

        Image image = imgObject.AddComponent<Image>();
        imgObject.transform.SetParent(collectionsPanel);
        image.gameObject.AddComponent<SetSecondaryColor>();
    }

    private void AddCollection(string name, int index)
    {
        GameObject newObj = Instantiate(collectionPrefab, collectionsPanel);
        newObj.GetComponent<RectTransform>().SetWidth(collectionsPanel.GetWidth() - 10);
        BonusCollectionItem newItem = newObj.GetComponent<BonusCollectionItem>();
        newItem.Init(name, index);
    }

    private void AddCollection(string name, Transform parent)
    {
        GameObject newObj = Instantiate(collectionPrefab, collectionsPanel);
        newObj.GetComponent<RectTransform>().SetWidth(collectionsPanel.GetWidth() - 10);
        BonusCollectionItem newItem = newObj.GetComponent<BonusCollectionItem>();
        newItem.Init(name, parent);
    }

    public void ShowCollection(int index)
    {
        SelectedObjectsManagement.Instance.DeselectAllObjects();

        foreach (var item in collectionParts[index])
        {
            var found = GlobalVariables.Instance.allNameScripts.Find(it => it.originalName == item);
            if (found == null || !found.gameObject.activeInHierarchy)
                continue;

            SelectedObjectsManagement.Instance.SelectObject(found.gameObject);
        }

        SelectedObjectsManagement.Instance.CheckLabelsVisibility();
        ActionControl.Instance.UpdateButtons();
        ActionControl.Instance.AddCommand(new SelectCommand(SelectedObjectsManagement.Instance.selectedObjects), false);
        if(ActionControl.zoomSelected)
            CameraController.instance.CenterView(true);
    }

    public void ShowCollection(Transform hierarchyParent)
    {
        SelectedObjectsManagement.Instance.DeselectAllObjects();
        SelectedObjectsManagement.Instance.lastParentSelected = hierarchyParent;
        SelectedObjectsManagement.Instance.SelectObject(hierarchyParent.gameObject);
        SelectedObjectsManagement.Instance.SelectAllChildren(hierarchyParent);
        SelectedObjectsManagement.Instance.CheckLabelsVisibility();
        NameAndDescription nameScript = hierarchyParent.GetComponent<NameAndDescription>();
        nameScript.SetDescription();
        HierarchyBar.Instance.Set(hierarchyParent.transform);
        //Expand in lexicon
        Lexicon.Instance.ExpandRecursively();
        if (ActionControl.zoomSelected)
            CameraController.instance.CenterView(true);
        TranslateObject.Instance.UpdateSelected();
        ActionControl.Instance.AddCommand(new SelectCommand(SelectedObjectsManagement.Instance.selectedObjects), false);
        ActionControl.Instance.UpdateButtons();
    }

}
