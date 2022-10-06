using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
public enum PencilPlacement
{
    WorldPos,
    Surface,
    Screen
}

public enum PencilMode
{
    Pencil,
    Line,
    Polygon,
    Eraser
}

public class Pencil : MonoBehaviour
{
    public static Pencil instance;

    private Camera cam;
    private bool blocked;
    private Line3D currentLine;
    [SerializeField] private Line3D linePrefab;


    [Header("Parameters")]
    public float width;
    public bool autoScaleWidth;
    public bool alwaysVisible = false;
    public Color color;

    [Space]
    [Header("UI objects")]
    public Slider valueSlider;
    public TMP_Dropdown dropdownPencilMode;
    public RectTransform screenModeCanvas;
    public GameObject alwaysVisibleOption;

    [Space]
    [Header("Toggle buttons")]
    public ToggleChangeColor pencilToggle;
    public ToggleChangeColor pencilLineToggle;
    public ToggleChangeColor pencilPolygonToggle;
    public ToggleChangeColor pencilEraserToggle;

    [Space]
    [Header("Line shaders")]
    public Shader defaultShader;
    public Shader alwaysVisibleShader;


    [HideInInspector]
    public PencilPlacement placement = PencilPlacement.WorldPos;
    [HideInInspector]
    public PencilMode mode = PencilMode.Pencil;

    int layer_mask1;
    int layer_mask2;
    int layer_mask3;
    LayerMask finalmask;

    private float offset = 0.005f;

    private void Awake()
    {
        instance = this;
        cam = Camera.main;

        layer_mask1 = LayerMask.GetMask("Body");
        layer_mask2 = LayerMask.GetMask("Outline");
        layer_mask3 = LayerMask.GetMask("HighlightedOutline");

        finalmask = layer_mask1 | layer_mask2 | layer_mask3;
    }

    private void Update()
    {
        if (!ActionControl.pencil || EventSystem.current.IsPointerOverGameObject())
            return;

        if (placement == PencilPlacement.WorldPos || placement == PencilPlacement.Screen)
            NormalPlacement();
        else if (placement == PencilPlacement.Surface)
            SurfacePlacement();

    }

    private void NormalPlacement()
    {
        Vector3 mousePos;
        if (placement == PencilPlacement.Screen)
            mousePos = new Vector2(Mouse.current.position.ReadValue().x - screenModeCanvas.GetWidth() / 2, 
                Mouse.current.position.ReadValue().y - screenModeCanvas.GetHeight() / 2);
        else
            mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue()) 
                + cam.transform.forward 
                * Vector3.Distance(CameraController.instance.cameraCenter.position, cam.transform.position);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            blocked = EventSystem.current.IsPointerOverGameObject();
            CreateLine();
            if (mode == PencilMode.Line)
                currentLine.SetPoint(0, mousePos);
        }
        else if (Mouse.current.leftButton.isPressed  && !blocked)
        {
            if (mode == PencilMode.Pencil)
                currentLine.AddPoint(mousePos);
            else if (mode == PencilMode.Line)
                currentLine.SetPoint(1, mousePos);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame && currentLine != null)
        {
            blocked = !EventSystem.current.IsPointerOverGameObject();
            if (currentLine.lineRenderer.positionCount < 2)
                Destroy(currentLine.gameObject);
            else
                ActionControl.Instance.AddCommand(new DrawLineCommand(currentLine.gameObject), false);
        }
    }

    private void SurfacePlacement()
    {
        RaycastHit hit;
        Ray raycast = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(raycast, out hit, 100, finalmask))
        {
            if (hit.transform.GetComponent<BodyPart>() != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    blocked = EventSystem.current.IsPointerOverGameObject();
                    CreateLine();
                    if (mode == PencilMode.Line)
                        currentLine.SetPoint(0, hit.point + hit.normal * offset);
                }
                else if(Mouse.current.leftButton.isPressed&& !blocked)
                {
                    if (mode == PencilMode.Pencil)
                        currentLine.AddPoint(hit.point + hit.normal * offset);
                    else if (mode == PencilMode.Line)
                        currentLine.SetPoint(1, hit.point + hit.normal * offset);
                }
            }
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame && currentLine != null)
        {
            blocked = !EventSystem.current.IsPointerOverGameObject();
            if (currentLine.lineRenderer.positionCount < 2)
                Destroy(currentLine.gameObject);
            else
                ActionControl.Instance.AddCommand(new DrawLineCommand(currentLine.gameObject), false);
        }
    }

    private void CreateLine()
    {
        if(placement == PencilPlacement.Screen)
            currentLine = Instantiate(linePrefab, screenModeCanvas);
        else
            currentLine = Instantiate(linePrefab);

        if (alwaysVisible)
            currentLine.lineRenderer.material = new Material(alwaysVisibleShader);
        else
            currentLine.lineRenderer.material = new Material(defaultShader);

        currentLine.lineRenderer.useWorldSpace = placement != PencilPlacement.Screen;
        currentLine.lineRenderer.material.color = color;
        currentLine.pencilPlacement = placement;
        if (autoScaleWidth)
            currentLine.lineWidth = width / 100f;
        else
            currentLine.lineWidth = width / 100f * cam.orthographicSize;
        currentLine.autoScaleWidth = autoScaleWidth;
    }

    public void DestroyLine(GameObject line)
    {
        line.gameObject.SetActive(false);
    }

    public void InstantiateLine(GameObject line)
    {
        line.gameObject.SetActive(true);
    }

    public void SetSize()
    {
        width = valueSlider.value;
    }

    public void SetAutoScale()
    {
        autoScaleWidth = !autoScaleWidth;
    }

    public void SetAlwaysVisible()
    {
        alwaysVisible = !alwaysVisible;
    }

    public void SetPencilPlacement()
    {
        switch(dropdownPencilMode.value)
        {
            case 0:
                placement = PencilPlacement.WorldPos;
                break;
            case 1:
                placement = PencilPlacement.Surface;
                break;
            case 2:
                placement = PencilPlacement.Screen;
                break;
            default:
                break;
        }

        alwaysVisibleOption.SetActive(placement != PencilPlacement.Screen);
    }

    public void SetPencilMode()
    {
        mode = PencilMode.Pencil;
        pencilToggle.SetEnabledColor();
        pencilLineToggle.SetDisabledColor();
        pencilPolygonToggle.SetDisabledColor();
        pencilEraserToggle.SetDisabledColor();
    }

    public void SetLinePencilMode()
    {
        mode = PencilMode.Line;
        pencilToggle.SetDisabledColor();
        pencilLineToggle.SetEnabledColor();
        pencilPolygonToggle.SetDisabledColor();
        pencilEraserToggle.SetDisabledColor();
    }

    public void SetPolygonPencilMode()
    {
        /*mode = PencilMode.Polygon;
        pencilToggle.SetDisabledColor();
        pencilLineToggle.SetDisabledColor();
        pencilPolygonToggle.SetEnabledColor();
        pencilEraserToggle.SetDisabledColor();*/

        PopUpManagement.Instance.Show("Not implemented yet!");
    }

    public void SetEraserPencilMode()
    {
        /*mode = PencilMode.Eraser;
        pencilToggle.SetDisabledColor();
        pencilLineToggle.SetDisabledColor();
        pencilPolygonToggle.SetDisabledColor();
        pencilEraserToggle.SetEnabledColor();*/
        PopUpManagement.Instance.Show("Not implemented yet!");

    }
}
