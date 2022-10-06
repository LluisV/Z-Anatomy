using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [HideInInspector]
    public Material lineMaterial;
    private Camera cam;
    public CameraController camScript;
    private MaterialPropertyBlock _propBlock;
    [HideInInspector]
    public LineRenderer _renderer;
    Color lineColor;
    private float initialSize;
    [HideInInspector]
    public Vector3 lineDirection;

    [HideInInspector]
    public Transform minPoint;
    [HideInInspector]
    public Transform maxPoint;

    private void Awake()
    {
        cam = Camera.main;

        minPoint = transform.Find("minPoint");
        maxPoint = transform.Find("maxPoint");
        _renderer = gameObject.AddComponent<LineRenderer>();
        _renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        DrawLine(minPoint.position, maxPoint.position);
        _propBlock = new MaterialPropertyBlock();

        lineColor = lineMaterial.color;
        gameObject.SetActive(true);

    }

    private void Start()
    {
        initialSize = GlobalVariables.Instance.lineSize / 500;

    }

    // Update is called once per frame
    void Update()
    {
        _renderer.startWidth = initialSize * cam.orthographicSize;
        _renderer.endWidth = initialSize * cam.orthographicSize;
    }

    void DrawLine(Vector3 start, Vector3 end)
    {
        //  transform.position = start;
        _renderer.material = lineMaterial;
        _renderer.startColor = lineColor;
        _renderer.endColor = lineColor;
        _renderer.startWidth = 0.005f;
        _renderer.endWidth = 0.005f;
        _renderer.SetPosition(0, start);
        _renderer.SetPosition(1, end);
    }

    public void SetColor(float a)
    {
        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetColor("_Color", new Color(lineColor.r, lineColor.g, lineColor.b, a));
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
        // onRoutine = false;
    }

    public void SetColor(Color color)
    {
        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetColor("_Color", color);
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
        // onRoutine = false;
    }

    public void UpdatePos()
    {
        _renderer.SetPosition(0, minPoint.position);
        _renderer.SetPosition(1, maxPoint.position);
    }
}
