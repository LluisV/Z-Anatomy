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
    [HideInInspector]
    public List<GameObject> hierarchyObjects = new List<GameObject>();
    public List<HierarchyButton> hierarchyButtons = new List<HierarchyButton>();

    private Transform lastObj;

    private void Awake()
    {
        Instance = this;
        layout = buttonsParent.GetComponent<HorizontalLayoutGroup>();
    }

    public void Set(Transform obj = null)
    {
        //Clear previous
        Clear();
        hierarchyButtons.Clear();

        if (obj == null)
            obj = lastObj;

        lastObj = obj;

        //Get all parents
        while (obj != null)
        {
            if(obj.GetComponent<BodyPartVisibility>() != null)
                hierarchyObjects.Add(obj.gameObject);
            obj = obj.transform.parent;
        }

        //Foreach object
        for (int i = hierarchyObjects.Count - 1; i >= 0; i--)
            AddButton(hierarchyObjects[i], i == hierarchyObjects.Count - 1, i == 0);

        StopCoroutine(SetWidth());
        StartCoroutine(SetWidth());
        SetWidth();         
    }


    private IEnumerator SetWidth()
    {

        yield return new WaitForEndOfFrame();
        float buttonsWidth = GetWidth();
        float parentWidth = buttonsParent.GetWidth();
        int i = 0;
        while (buttonsParent.childCount > 0 && buttonsWidth > parentWidth)
        {
            buttonsWidth -= buttonsParent.GetChild(i).GetComponent<RectTransform>().GetWidth() + layout.spacing + 5;
            Destroy(buttonsParent.GetChild(i).gameObject);
            i++;
        }

        moreImg.enabled = i > 0;
        if (i > 0)
            layout.padding.left = 50;

        foreach (var btn in hierarchyButtons)
            btn.Enable();
    }

    private float GetWidth()
    {

        float buttonsWidth = 0;
        foreach (RectTransform child in buttonsParent.transform)
            buttonsWidth += child.GetWidth() + layout.spacing;
        return buttonsWidth + layout.padding.left + 25;
    }

    private void AddButton(GameObject obj, bool isRoot, bool isFirst)
    {
        GameObject newBtn = Instantiate(btnPrefab, buttonsParent);
        HierarchyButton script = newBtn.GetComponent<HierarchyButton>();
        hierarchyButtons.Add(script);
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
