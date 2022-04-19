using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LabelGroup : MonoBehaviour
{
    public Camera cam;
    [HideInInspector]
    public string name;
    private TextMeshPro text;
    private float parentScale;
    private float initialSize;
    Vector3 initialPos;
    private bool onPositionRoutine;
    public CameraController camScript;
    private MaterialPropertyBlock _propBlock;
    private Renderer _renderer;
    private Transform originPoint;
    RectTransform rect;
    BoxCollider boxCollider;
    [HideInInspector]
    public int hierarchyLevel;
    [HideInInspector]
    public bool selected;
    [HideInInspector]
    public Material labelMaterial;

    private bool onRoutine;
    private void Awake()
    {
        parentScale = transform.parent.lossyScale.x;
        text = gameObject.GetComponent<TextMeshPro>();
        initialSize = 2.1f - hierarchyLevel * 0.2f;
        text.margin = new Vector4(8f, 2f, 8f, 2f);
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = initialSize * 1.5f * 0.35f;
        text.material = labelMaterial;
        rect = GetComponent<RectTransform>();
        rect.localScale = new Vector3(1 / parentScale, 1 / parentScale, 1 / parentScale);
        initialPos = rect.localPosition;
        _propBlock = new MaterialPropertyBlock();
        _renderer = text.renderer;

        SetText(gameObject.name.Replace("-line", "").Replace(".gLabel", ""));
        try
        {
            Transform maxPoint = transform.parent.Find(gameObject.name.Replace(".gLabel", "-line")).Find("maxPoint");
            Transform minPoint = transform.parent.Find(gameObject.name.Replace(".gLabel", "-line")).Find("minPoint");
            if (Vector3.Distance(maxPoint.position, transform.position) > Vector3.Distance(minPoint.position, transform.position))
                originPoint = maxPoint;
            else
                originPoint = minPoint;
        }
        catch (System.Exception)
        {
            originPoint = transform.parent;
        }

        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.center = Vector3.zero;
    }
    void Update()
    {
        transform.rotation = cam.transform.rotation;
       // if (!onRoutine)
        {
            boxCollider.size = text.textBounds.size;
            if (cam.orthographicSize < 1.5f && cam.orthographicSize > 0.35f)
            {
                text.fontSize = initialSize * cam.orthographicSize * 0.35f;
            }

           UpdateColor();
        }

        /*if (!onPositionRoutine && cam.orthographicSize < 5)
            StartCoroutine(UpdatePosition());*/
    }

    void UpdateColor()
    {
        //  yield return new WaitForSeconds(0.1f);
        float angle = Vector3.Angle(originPoint.position - transform.position, -cam.transform.forward);
        float a = angle * angle * angle * angle * 0.00000005f;
        //text.color = new Color(1, 1, 1, a);
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
        text.text = name;
        if (transform.parent.name.Contains("."))
        {
            int indexOfPoint = transform.parent.name.IndexOf('.');
            string substring = transform.parent.name.Substring(0, indexOfPoint);
          /*  if (name.Equals(substring))
            {
                text.fontStyle = FontStyles.Bold;
                initialSize *= 1.5f;
            }*/
        }

    }

    public void LabelGroupClick()
    {
        selected = !selected;
        ActivateChildrenLabelGroup();
    }

    public void ActivateChildrenLabelGroup()
    {
        foreach (Transform child in transform.parent)
        {
            foreach (Transform label in child.transform)
            {
                if (label.name.Contains(".gLabel"))
                {
                    label.gameObject.SetActive(true);
                    child.Find(label.name.Replace(".gLabel", "-line")).gameObject.SetActive(true);
                }

            }
        }
    }
}
