using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerExitHandler, IPointerEnterHandler 
{
    public UnityEvent onDrag;
    public Texture2D dragCursorTexture;
    public Vector2 cursorOffset;
    private bool dragging = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(dragCursorTexture, cursorOffset, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(WaitForEndDrag());
    }

    IEnumerator WaitForEndDrag()
    {
        yield return new WaitUntil(() => !dragging);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (onDrag.GetPersistentEventCount() == 1 && onDrag.GetPersistentMethodName(0) == "ResizePanels")
            PanelsManagement.instance.ResizePanels(-eventData.delta.x);
        else
            onDrag.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
    }

}
