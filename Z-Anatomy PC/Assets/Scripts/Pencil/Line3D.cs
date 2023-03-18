using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Line3D : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [HideInInspector]
    public LineRenderer lineRenderer;

    public float lineWidth;
    public bool autoScaleWidth;

    [Space]
    [Header("Dotted Line Attributes")]
    public bool dottedLine;
    public float lineLength;
    public float lineHeigth;
    public float lineRepetition;
    public float lineAnimSpeed;


    [HideInInspector]
    public PencilPlacement pencilPlacement;

    public const float RESOLUTION = .001f;

    Camera cam;

    float defaultDist;

    private void Awake()
    {
        cam = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
        defaultDist = cam.orthographicSize;
        if (dottedLine)
        {
            lineRenderer.sharedMaterial.SetFloat("_Heigth", lineHeigth);
            lineRenderer.sharedMaterial.SetFloat("_Width", lineLength);
            lineRenderer.sharedMaterial.SetFloat("_AnimSpeed", lineAnimSpeed);
        }
    }

    private void Update()
    {
        UpdateProperties();
    }

    public void AddPoint(Vector3 pos)
    {
        if (!CanAppend(pos)) return;

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
    }

    public void SetPoint(int index, Vector3 pos)
    {
        lineRenderer.positionCount = index + 1;
        lineRenderer.SetPosition(index, pos);
    }

    private bool CanAppend(Vector3 pos)
    {
        if (lineRenderer.positionCount == 0) return true;

        return Vector3.Distance(lineRenderer.GetPosition(lineRenderer.positionCount - 1), pos) > RESOLUTION * Camera.main.orthographicSize;
    }

    public float LineLength()
    {
        if (lineRenderer.positionCount < 2)
            return 0;

        float length = 0;

        for (int i = 1; i < lineRenderer.positionCount; i++)
            length += Vector3.Distance(lineRenderer.GetPosition(i - 1), lineRenderer.GetPosition(i));

        return length;
    }

    public void UpdateProperties()
    {
        if(dottedLine)
            lineRenderer.sharedMaterial.SetFloat("_Repeat", lineRepetition / cam.orthographicSize);


        if(autoScaleWidth && pencilPlacement == PencilPlacement.Screen)
            transform.localScale = new Vector3(1/cam.orthographicSize * defaultDist, 1 / cam.orthographicSize * defaultDist, 1);

        if (autoScaleWidth || pencilPlacement == PencilPlacement.Screen)
        {
            if(autoScaleWidth)
            {
                lineRenderer.startWidth = lineWidth * cam.orthographicSize;
                lineRenderer.endWidth = lineWidth * cam.orthographicSize;
            }
            else
            {
                lineRenderer.startWidth = lineWidth * cam.orthographicSize / defaultDist;
                lineRenderer.endWidth = lineWidth * cam.orthographicSize / defaultDist;
            }
        }
        else 
        {
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //OUTLINE MASK
        if (gameObject.layer != 7)
            gameObject.layer = 13;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameObject.layer == 13)
            gameObject.layer = 6;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}