using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TextMeshPro))]
public class CubeText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler, IDragHandler
{
    public Camera cam;
    public float fontSize;
    public Transform cubeCenter;
    public Material labelMaterial;
    public float maxColorAlpha = 1;
    public float maxTextAlpha = 1;
    public float minTextAlpha = 0;
    public GizmoFace cubeFace;
    public bool isPlanesCube;
    public CrossPlanesGizmo planesScript;
    public Renderer line;
    public Renderer sphere;
    public bool isMinus;
    private GizmoDrag gizmoDrag;

    private TextMeshPro text;
    private MaterialPropertyBlock _propBlock_t;
    private Renderer _renderer;
    private Material mat;
    private Color originalTextColor;
    private Color originalSLColor;
    private CubeBehaviour cubeScript;
    private bool selected;
    private bool mouseOver;

    private float prevMinAlpha;

    private void Awake()
    {
        gizmoDrag = transform.parent.parent.parent.GetComponentInChildren<GizmoDrag>();
    }

    private void Start()
    {
        cubeScript = GetComponentInParent<CubeBehaviour>();
        text = gameObject.GetComponent<TextMeshPro>();
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = fontSize * 0.05f;
        text.material = labelMaterial;
        originalTextColor = labelMaterial.color;
        originalSLColor = sphere.material.color;
        mat = new Material(text.material);
        _propBlock_t = new MaterialPropertyBlock();
        _renderer = text.renderer;
        prevMinAlpha = minTextAlpha;
    }
    public void SetSelectedColor(Color color)
    {
        sphere.material.color = new Color(originalSLColor.r + 0.15f, originalSLColor.g + 0.15f, originalSLColor.b + 0.15f);
        selected = true;
    }

    public void SetOriginalColor()
    {
        sphere.material.color = originalSLColor;
        selected = false;
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = cam.transform.rotation;
        UpdateColor();
    }

    void UpdateColor()
    {
        float angle = Vector3.Angle(cubeCenter.position - transform.position, -cam.transform.forward);
        float a = angle * angle * angle * angle * 0.000000025f;
        float b = angle * angle * angle * 0.00000025f;

        if (b < minTextAlpha)
            b = minTextAlpha;
        if (b > maxColorAlpha)
            b = maxColorAlpha;

        sphere.material.color = new Color(sphere.material.color.r, sphere.material.color.g, sphere.material.color.b, b);
        line.material.color = new Color(originalSLColor.r, originalSLColor.g, originalSLColor.b, b);

        if (a > maxTextAlpha)
            a = maxTextAlpha;

        if (isMinus && !mouseOver && !selected)
            a = 0;

        _renderer.enabled = a > .005f;

        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock_t);
        // Assign our new value.
        _propBlock_t.SetColor("_FaceColor", new Color(mat.color.r, mat.color.g, mat.color.b, a));
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock_t);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        if (!selected)
        {
            mat.color = Color.white;
            minTextAlpha = 1;
        }
        gizmoDrag.PointerEnter();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cubeScript.SetCameraRotation(cubeFace);
        if (Shortcuts.Instance.setPlaneGizmo.IsPressed())
            planesScript.SetPlane(cubeFace);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
        {
            mat.color = originalTextColor;
            minTextAlpha = prevMinAlpha;
        }
        mouseOver = false;
        RaycastObject.instance.raycastBlocked = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //We block the main raycast so we don't click other things
        RaycastObject.instance.raycastBlocked = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        gizmoDrag.PointerUp();
    }

    public void OnDrag(PointerEventData eventData)
    {
        gizmoDrag.Drag();
    }
}
