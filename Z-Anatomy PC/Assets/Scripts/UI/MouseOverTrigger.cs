using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ExpandCollapseUI animator;

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.ExpandButtonClick();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.Collapse();
    }

}
