using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TranslateGizmo : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool X;
    public bool Y;
    public bool Z;
    private Vector3 lastMousePos;
    private MeshRenderer[] renderers;
    float h;
    float s;
    float v;
    bool wasDragging;

    private void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
        Color.RGBToHSV(renderers[0].material.color, out h, out s, out v);
    }

    private void Update()
    {
        if(Mouse.current.leftButton.wasReleasedThisFrame && wasDragging)
        {
            wasDragging = false;
            TranslateObject.Instance.SaveAction();
            TranslateObject.Instance.UpdatePrevPos();
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if(lastMousePos == Vector3.zero)
        {
            lastMousePos = eventData.position;
            return;
        }

        Vector3 A = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 0));
        Vector3 B = Camera.main.ScreenToWorldPoint(new Vector3(lastMousePos.x, lastMousePos.y, 0));

        Vector3 direction = B - A;

        if (X)
            TranslateObject.Instance.Translate(new Vector3(-direction.x, 0, 0));
        else if (Y)
            TranslateObject.Instance.Translate(new Vector3(0, -direction.y, 0));
        else if (Z)
            TranslateObject.Instance.Translate(new Vector3(0, 0, -direction.z));

        lastMousePos = eventData.position;
        wasDragging = true;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lastMousePos = Vector3.zero;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (var renderer in renderers)
        {
            renderer.material.color = Color.HSVToRGB(h + 0.05f, s, v);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (var renderer in renderers)
        {
            renderer.material.color = Color.HSVToRGB(h, s, v);
        }
    }
}
