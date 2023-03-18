using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class MoveOnDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("The object to move (can be null if it is this obj)")]
    public RectTransform toMove;

    private bool mouseIn;
    private bool canDrag;

    private Vector2 delta = Vector3.zero;
    private Vector2 lastPos = Vector3.zero;


    private void Awake()
    {
        if(toMove == null)
            toMove = GetComponent<RectTransform>();
    }

    private void Start()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastPos = Mouse.current.position.ReadValue();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(Mouse.current.middleButton.isPressed || Mouse.current.leftButton.isPressed)
        {
            delta = Mouse.current.position.ReadValue() - lastPos;

            toMove.position += new Vector3(delta.x, delta.y, 0);

            lastPos = Mouse.current.position.ReadValue();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CameraController.instance.movementIsBlocked = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CameraController.instance.movementIsBlocked = false;
    }
}
