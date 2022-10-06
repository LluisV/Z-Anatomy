using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GizmoDrag : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IDragHandler
{
    private Image img;
    private bool mouseIn;
    private RectTransform rt;
    private Vector3 initialMousePos;

    private void Awake()
    {
        img = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        transform.localScale = Vector3.one * CubeBehaviour.instance.cubeSize * Camera.main.orthographicSize / 8;
        rt.anchoredPosition = new Vector2(CubeBehaviour.instance.cubePosition.x * Camera.main.orthographicSize / 7, CubeBehaviour.instance.cubePosition.y * Camera.main.orthographicSize / 7);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnter();
    }

    public void PointerEnter()
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0.03f);
        mouseIn = true;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!ActionControl.draggingGizmo)
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        mouseIn = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        PointerUp();
    }

    public void PointerUp()
    {
        ActionControl.draggingGizmo = false;
        if (!mouseIn)
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Drag();
    }

    public void Drag()
    {
        ActionControl.draggingGizmo = true;
    }

}
