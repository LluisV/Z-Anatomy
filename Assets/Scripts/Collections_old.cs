using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class Collections_1 : MonoBehaviour
{
    public static Collections_1 instance;
    [HideInInspector]
    public Dictionary<string, Bounds> bounds = new Dictionary<string, Bounds>();
    public GameObject canvas;
    public Camera collectionsCanvasCamera;
    public Camera mainCanvasCamera;
    public Camera mainCamera;
    public Transform canvasParent;
    public GameObject elementPrefab;
    public Transform collectionsRoot;
    public Button backBtn;
    public GameObject hierarchyButtonsParent;
    public GameObject hierarchyButtonPrefab;
    private ResponsiveCanvas canvasScript;
    [HideInInspector]
    public Transform actualRoot;
    private CameraController camScript;
    private int deepLevel = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        actualRoot = collectionsRoot;
        canvasScript = FindObjectOfType<ResponsiveCanvas>();
        camScript = FindObjectOfType<CameraController>();
        UpdateElements();
    }

    private void UpdateElements()
    {
        canvasScript.DeleteElements();
        foreach (Transform element in actualRoot)
        {
            if(!element.CompareTag("Debug") && !element.name.Contains(".t") && !element.name.Contains("-lin") && !element.name.Contains(".gLabel") && !element.name.Contains("label"))
            {
                GameObject newElement = Instantiate(elementPrefab, canvasScript.elementsParent.transform);
                newElement.name = element.name;

                Transform parent = newElement.transform.Find("Placeholder").Find("Center");
                GameObject objectInside = Instantiate(element.gameObject, parent);
                ClearObjectToSpawn(objectInside, parent);

                newElement.GetComponent<CollectionClick>().collection = element;
                int indexOfPoint = element.name.IndexOf('.');
                string cleanName = element.name;
                if (indexOfPoint != -1)
                    cleanName = element.name.Substring(0, indexOfPoint);
                newElement.GetComponentInChildren<TextMeshProUGUI>().text = cleanName;
                canvasScript.elements.Add(newElement.GetComponent<RectTransform>());
            }
        }
        canvasScript.UpdatePositions();
    }
    public void OpenCanvas()
    {
        collectionsRoot.gameObject.SetActive(false);
        camScript.enabled = false;
        //Deactivate all other canvas
        foreach (Transform canvas in canvasParent)
        {
            canvas.gameObject.SetActive(false);
        }

        canvas.SetActive(true);
        collectionsCanvasCamera.gameObject.SetActive(true);
        mainCanvasCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(false);
        canvasScript.UpdatePositions();
    }

    public void CloseCanvas()
    {
        collectionsRoot.gameObject.SetActive(true);
        camScript.enabled = true;
        //Activate all other canvas
        foreach (Transform canvas in canvasParent)
        {
            canvas.gameObject.SetActive(true);
        }
        canvas.SetActive(false);
        collectionsCanvasCamera.gameObject.SetActive(false);
        mainCanvasCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(true);
    }

    public void CollectionsClick(Transform newRoot)
    {
        deepLevel++;
        backBtn.interactable = true;
        actualRoot = newRoot;
        Instantiate(hierarchyButtonPrefab, hierarchyButtonsParent.transform);
        UpdateElements();
    }

    public void HierarchyClick(Transform newRoot, int deep)
    {
        deepLevel = deep;
        backBtn.interactable = deep > 0;
        actualRoot = newRoot;
        UpdateElements();
    }

    public void BackClick()
    {
        deepLevel--;
        if (deepLevel == 0)
            backBtn.interactable = false;
        actualRoot = actualRoot.parent;
        Destroy(hierarchyButtonsParent.transform.GetChild(hierarchyButtonsParent.transform.childCount - 1).gameObject);
        UpdateElements();
    }


    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    private void ClearObjectToSpawn(GameObject obj, Transform parent)
    {
        // RecursivelyBuildBounds(ref bounds, obj.transform);
        RecursivelyCleanComponents(obj.transform);
        Bounds objBounds = CalculateLocalBounds(obj);
        obj.transform.rotation = new Quaternion(0, 180, 0, 0);
        obj.transform.position = obj.transform.parent.transform.position;
        if(objBounds.size.magnitude > 0)
            obj.transform.localScale = new Vector3(300000, 300000, 300000) / objBounds.size.magnitude;
        obj.SetActive(true);
        SetLayerRecursively(obj, LayerMask.NameToLayer("UI"));
    }

    private void RecursivelyCleanComponents(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag("Debug") || child.name.Contains(".t") || child.name.Contains("-lin") || child.name.Contains(".gLabel") || child.name.Contains("label"))
                Destroy(child.gameObject);
            if (child.GetComponent<BodyPart>() != null)
                Destroy(child.GetComponent<BodyPart>());
            if (child.childCount > 0)
                RecursivelyCleanComponents(child);
        }
    }

    private void RecursivelyBuildBounds(ref Bounds bounds, Transform parent)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.SetActive(true);
            BodyPart script = child.GetComponent<BodyPart>();
            if (child.GetComponent<BodyPart>() != null)
            {
                Vector2 a = child.GetComponent<MeshCollider>().bounds.min;
            }
            if (child.childCount > 0)
                RecursivelyBuildBounds(ref bounds, child);
        }
    }

    private Bounds CalculateLocalBounds(GameObject obj)
    {
        Bounds localBounds = new Bounds();

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
           //Find first enabled renderer to start encapsulate from it
            foreach (Renderer renderer in renderers)
            {
                if (bounds.ContainsKey(renderer.name))
                {
                    localBounds = bounds[renderer.name];
                    break;
                }
            }

            //Encapsulate for all renderers

            foreach (Renderer renderer in renderers)
            {
                if (bounds.ContainsKey(renderer.name))
                {
                    localBounds.Encapsulate(bounds[renderer.name]);

                }
            }
        }
        return localBounds;
    }
}
