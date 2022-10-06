using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler
{
    Vector2 deltaValue = Vector2.zero;
    public void OnDrag(PointerEventData eventData)
    {
        if (BorderlessWindow.framed || Screen.fullScreen)
            return;

        deltaValue += eventData.delta;
        if (eventData.dragging)
        {
            var currentRes = ResolutionManager.Instance.WindowedResolutions[ResolutionManager.Instance.currWindowedRes];
            BorderlessWindow.MoveWindowPos(deltaValue,(int)currentRes.x, (int)currentRes.y);
        }
    }
}
