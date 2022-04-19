using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Label : MonoBehaviour
{
    public Camera cam;
    private TextMeshPro text;
    private float parentScale;
    private float initialSize;
    public CameraController camScript;
    private MaterialPropertyBlock _propBlock;
    private Renderer _renderer;
    private Transform originPoint;
    private BoxCollider boxCollider;
    RectTransform rect;
    [HideInInspector]
    public Material labelMaterial;
    [HideInInspector]
    public GameObject line;
    private NameAndDescription nameScript;
    private IsVisible isVisibleScript;
    private bool hasLine;
    private void Awake()
    {
        nameScript = GetComponent<NameAndDescription>();
        isVisibleScript = GetComponent<IsVisible>();

    }

    private void Start()
    {
        try
        {
            string lineName = nameScript.originalName.Replace(".t", "") + "-line";
            line = transform.parent.Find(lineName).gameObject;
            line.SetActive(true);
            originPoint = line.transform.Find("maxPoint");
            hasLine = true;
        }
        catch (System.Exception)
        {
            hasLine = false;
        }
        nameScript.SetTranslatedName();
        isVisibleScript.SetLabelVisibility();
        Initialize();
    }

    public void Initialize()
    {
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.center = Vector3.zero;
        parentScale = transform.parent.lossyScale.x;
        text = gameObject.GetComponent<TextMeshPro>();
        if (SystemInfo.deviceType == DeviceType.Handheld)
            initialSize = 0.7f;
        else
            initialSize = 0.85f;

        text.margin = new Vector4(8f, 2f, 8f, 2f);
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = initialSize * 1.5f * 0.35f;
        text.material = labelMaterial;
        rect = GetComponent<RectTransform>();
        rect.localScale = new Vector3(1 / parentScale, 1 / parentScale, 1 / parentScale);
        _propBlock = new MaterialPropertyBlock();
        _renderer = text.renderer;
        SetText(gameObject.name.Replace("-line", "").Replace(".t", ""));
    }

    // Update is called once per frame
    void Update()
    {
        boxCollider.size = text.textBounds.size;
        transform.rotation = cam.transform.rotation;
        if (cam.orthographicSize < 1.5f && cam.orthographicSize > 0.075f)
            text.fontSize = initialSize * cam.orthographicSize * 0.35f;
        if(hasLine)
            UpdateColor();
    }

    void UpdateColor()
    {
        float angle = Vector3.Angle(originPoint.position - transform.position, -cam.transform.forward);
        float a = angle * angle * angle * angle * 0.000000025f;

        _renderer.enabled = a > .005f;

        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetColor("_FaceColor", new Color(1, 1, 1, a));
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }

    public void SetText(string name)
    {
        if(text != null)
        {
            text.text = name;
            string substring = name;
            if (transform.parent.name.Contains("."))
            {
                int indexOfPoint = transform.parent.name.IndexOf('.');
                substring = transform.parent.name.Substring(0, indexOfPoint);
            }
            if (name.Equals(substring))
            {
                text.fontStyle = FontStyles.Bold;
                initialSize *= 1.5f;
                text.fontSize = initialSize * 1.5f * 0.35f;
            }
        }
    }

    public void Click()
    {
        if (ActionControl.canErase)
        {
            List<GameObject> labelAndLine = new List<GameObject>();
            labelAndLine.Add(gameObject);
            if (line != null)
                labelAndLine.Add(line);
            SelectedObjectsManagement.instance.DeleteList(labelAndLine);
        }
        else if (!SelectedObjectsManagement.instance.selectedObjects.Contains(gameObject))
        {
            SelectedObjectsManagement.instance.DeselectAllChildren(gameObject.transform.parent);
            SelectedObjectsManagement.instance.SelectObject(gameObject);
            NamesManagement.instance.SetTitle(gameObject.name);
            NamesManagement.instance.SetDescription(gameObject.GetComponent<NameAndDescription>().Description(), gameObject);
        }
        else
        {
            SelectedObjectsManagement.instance.DeselectObjet(gameObject);
            GetComponent<IsVisible>().isVisible = false;
        }
        SelectedObjectsManagement.instance.EnableDisableButtons();
    }
}

