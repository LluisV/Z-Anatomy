using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExplosionGizmo : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector3 lastMousePos;

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (lastMousePos == Vector3.zero)
        {
            lastMousePos = eventData.position;
            return;
        }

        Vector3 A = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 0));
        Vector3 B = Camera.main.ScreenToWorldPoint(new Vector3(lastMousePos.x, lastMousePos.y, 0));

        Vector3 direction = B - A;

        Explosion.Instance.Explode(Vector3.Dot(direction, Explosion.Instance.gizmo.position));

        lastMousePos = eventData.position;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lastMousePos = Vector3.zero;
    }
}
