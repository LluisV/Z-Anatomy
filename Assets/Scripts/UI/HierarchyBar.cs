using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HierarchyBar : MonoBehaviour
{
    public static HierarchyBar Instance;
    public GameObject btnPrefab;
    public RectTransform buttonsParent;
    public Image moreImg;
    private HorizontalLayoutGroup layout;
    private RectTransform layoutRect;
    [HideInInspector]
    public List<GameObject> hierarchyObjects = new List<GameObject>();

    private Transform lastObj;

    private void Awake()
    {
        Instance = this;
        layout = buttonsParent.GetComponent<HorizontalLayoutGroup>();
        layoutRect = layout.GetComponent<RectTransform>();
    }

    public void Set(Transform obj = null)
    {
        //Clear previous
        Clear();

        if (obj == null)
            obj = lastObj;

        lastObj = obj;

        //Get all parents
        while (obj != null)
        {
            if(obj.GetComponent<Visibility>() != null)
                hierarchyObjects.Add(obj.gameObject);
            obj = obj.transform.parent;
        }

        //Foreach object
        for (int i = hierarchyObjects.Count - 1; i >= 0; i--)
            AddButton(hierarchyObjects[i], i == hierarchyObjects.Count - 1, i == 0);

        StartCoroutine(SetWidth());
    }


    private IEnumerator SetWidth()
    {

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        float buttonsWidth = GetWidth() + 12.5f;

        int i = 0;
        while (buttonsWidth > buttonsParent.rect.width)
        {
            buttonsWidth -= buttonsParent.GetChild(i).GetComponent<RectTransform>().rect.width + layout.spacing;
            Destroy(buttonsParent.GetChild(i).gameObject);
            i++;
        }
        moreImg.enabled = i > 0;
        if (i > 0)
            layout.padding.left = 90;
    }

    private float GetWidth()
    {

        float buttonsWidth = 0;

        foreach (RectTransform child in buttonsParent.transform)
        {
            buttonsWidth += child.rect.width + layout.spacing;
        }
        return buttonsWidth;
    }

    private void AddButton(GameObject obj, bool isRoot, bool isFirst)
    {
        GameObject newBtn = Instantiate(btnPrefab, buttonsParent);
        HierarchyButton script = newBtn.GetComponent<HierarchyButton>();
        script.obj = obj;
        script.isRoot = isRoot;
        script.isFirst = isFirst;
    }

    public void Clear()
    {
        layout.padding.left = 0;
        moreImg.enabled = false;
        hierarchyObjects.Clear();
        foreach (Transform child in buttonsParent)
            Destroy(child.gameObject);
    }
}
