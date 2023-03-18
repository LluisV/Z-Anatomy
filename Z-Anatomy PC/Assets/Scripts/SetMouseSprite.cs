using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SetMouseSprite : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Texture2D cursor;
    public Vector2 cursorOffset;

    public bool onMouseEnter;
    public bool onMouseDown;

    public bool hideOnMouseDown;

    public UnityEvent onMouseDownEvent;
    public UnityEvent onMouseUpEvent;
    public UnityEvent onMouseDragEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (onMouseDown)
            Cursor.SetCursor(cursor, cursorOffset, CursorMode.Auto);
        else if(hideOnMouseDown)
            Cursor.visible = false;

        onMouseDownEvent.Invoke();
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (onMouseDown)
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        else if (hideOnMouseDown)
            Cursor.visible = true;

        onMouseUpEvent.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onMouseEnter)
            Cursor.SetCursor(cursor, cursorOffset, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (onMouseEnter)
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onMouseDragEvent.Invoke();
    }
}
