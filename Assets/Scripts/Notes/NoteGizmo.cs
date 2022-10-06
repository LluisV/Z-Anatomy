using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NoteGizmo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [HideInInspector]
    public Note note;
    [SerializeField]
    private Color defaultColor;
    [SerializeField]
    private Color highlightedColor;

    private Material mat;
    public float size;
    public float maxSize;
    public float offset;


    [HideInInspector]
    public RaycastHit hit;
    [HideInInspector]
    public bool placed;

    private void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
        mat.color = defaultColor;
    }

    private void Update()
    {
        UpdateScale();
        SetPosition(hit);
        if(!placed)
            SetRotation(hit);
    }

    public void SetPosition(RaycastHit hit)
    {
        transform.position = hit.point + hit.normal * offset * CameraController.instance.distance;
    }

    public void SetRotation(RaycastHit hit)
    {
        transform.LookAt(hit.point + hit.normal);
    }

    private void UpdateScale()
    {
        float newSize = size * CameraController.instance.distance;
        if(newSize < maxSize)
            transform.localScale = new Vector3(newSize, newSize, newSize);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (placed)
            mat.color = highlightedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (placed)
            mat.color = defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (placed)
            note.GizmoClick();
    }
}
