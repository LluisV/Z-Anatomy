using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class Label : MonoBehaviour
{
    private Camera cam;
    private TextMeshPro text;
    private float parentScale;
    private MaterialPropertyBlock _propBlock;
    private Renderer _renderer;
    private Transform originPoint;
    private BoxCollider boxCollider;
    RectTransform rect;
    [HideInInspector]
    public Material labelMaterial;
    [HideInInspector]
    public Line line;
    private NameAndDescription nameScript;
    private Visibility visibilityScript;
    private bool hasLine;
    private Color color;
    private float fontSize;

    [HideInInspector]
    public BodyPart parent;

    [HideInInspector]
    public Vector3 lineDirection;

    private void Awake()
    {
        nameScript = GetComponent<NameAndDescription>();
        visibilityScript = GetComponent<Visibility>();
        text = GetComponent<TextMeshPro>();
        boxCollider = gameObject.AddComponent<BoxCollider>();
        rect = GetComponent<RectTransform>();
        cam = Camera.main;
        color = Color.white;
        parent = GetComponentInParent<BodyPart>();
    }

    private void Start()
    {
        try
        {
            var lineObj = transform.parent.Find(new StringBuilder().Append(nameScript.originalName.Replace(".t", "").Replace(".s", "")).Append(".j").ToString());
            if(lineObj != null)
                line = lineObj.GetComponent<Line>();
            else
                line = transform.parent.Find(new StringBuilder().Append(nameScript.originalName.Replace(".t", "").Replace(".s", "")).Append(".i").ToString()).GetComponent<Line>();
            line.gameObject.SetActive(true);
            originPoint = line.transform.Find("maxPoint");
            hasLine = true;
        }
        catch (System.Exception)
        {
            hasLine = false;
        }


        if (line != null && line.minPoint != null && line.maxPoint != null)
            lineDirection = line.maxPoint.position - line.minPoint.position;
        else
            lineDirection = parent.transform.position - transform.position;
        
        Initialize();
    }

    public void Initialize()
    {
        boxCollider.center = Vector3.zero;
        parentScale = transform.parent.lossyScale.x;

        text.margin = new Vector4(8f, 2f, 8f, 2f);
        text.alignment = TextAlignmentOptions.Center;
        fontSize = GlobalVariables.Instance.labelFontSize * 0.15f;
        text.fontSize = fontSize * Mathf.Clamp(cam.orthographicSize, 0.075f, 1.5f);
        text.material = labelMaterial;

        rect.localScale = new Vector3(1 / parentScale, 1 / parentScale, 1 / parentScale);

        _propBlock = new MaterialPropertyBlock();
        _renderer = text.renderer;

        SetText(gameObject.name.Replace(".j", "").Replace(".i", "").Replace(".t", "").Replace(".s",""));
    }

    // Update is called once per frame
    void Update()
    {
        boxCollider.size = text.textBounds.size;
        transform.rotation = cam.transform.rotation;
        text.fontSize = fontSize * Mathf.Clamp(cam.orthographicSize, 0.075f, 1.5f);

        if (hasLine)
            UpdateColor();
    }

    void UpdateColor()
    {
        float angle = Vector3.Angle(originPoint.position - transform.position, -cam.transform.forward);
        float a = angle * angle * angle * angle * 0.000000025f;
        _renderer.enabled = a > .075f;
        line._renderer.enabled = _renderer.enabled;

        if (a > 1)
            a = 1;

        Color newColor = new Color(color.r, color.g, color.b, a);

        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetColor("_FaceColor", newColor);
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);

        if(visibilityScript.isSelected)
            line.SetColor(newColor);
        else
            line.SetColor(a * 0.5f);
    }

    public void SetText(string name)
    {
        if(text != null)
        {
            text.text = name;
            string substring = transform.parent.name.Replace("(R)", "").Replace("(L)", "").Trim();
            if (substring.Contains("."))
            {
                int indexOfPoint = transform.parent.name.IndexOf('.');
                substring = transform.parent.name.Substring(0, indexOfPoint);
            }
            if (name.Equals(substring))
            {
                text.fontStyle = FontStyles.Bold;
                fontSize = GlobalVariables.Instance.titleLabelFontSize * 0.15f;
            }
        }
    }

    public void Click()
    {
        if (!SelectedObjectsManagement.Instance.selectedObjects.Contains(gameObject))
        {
            SelectedObjectsManagement.Instance.DeselectAllChildren(gameObject.transform.parent);
            SelectedObjectsManagement.Instance.SelectObject(gameObject);

            //Update hierarchy bar
            HierarchyBar.Instance.Set(transform);
            //Expand in lexicon
            Lexicon.Instance.ExpandRecursively();
            nameScript.SetDescription();
        }
        else
        {
            SelectedObjectsManagement.Instance.DeselectObject(gameObject);
            GetComponent<Visibility>().isVisible = false;
        }
        ActionControl.Instance.UpdateButtons();
    }

    public void Select()
    {
        color = GlobalVariables.HighligthColor;
        if(!hasLine && _renderer != null)
        {
            // Get the current value of the material properties in the renderer.
            _renderer.GetPropertyBlock(_propBlock);
            // Assign our new value.
            _propBlock.SetColor("_FaceColor", color);
            // Apply the edited values to the renderer.
            _renderer.SetPropertyBlock(_propBlock);
        }
    }


    public void Deselect()
    {
        color = Color.white;
        if (!hasLine && _renderer != null)
        {
            // Get the current value of the material properties in the renderer.
            _renderer.GetPropertyBlock(_propBlock);
            // Assign our new value.
            _propBlock.SetColor("_FaceColor", color);
            // Apply the edited values to the renderer.
            _renderer.SetPropertyBlock(_propBlock);
        }
    }

    public void UpdatePos()
    {
        if(line != null)
            line.UpdatePos();
    }
}

