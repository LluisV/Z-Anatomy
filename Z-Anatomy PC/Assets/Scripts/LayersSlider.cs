using Assets.Scripts.Commands;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class LayersSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool valueChanged;
    List<GameObject> beforeDrag = new List<GameObject>();

    public void ValueChanged()
    {
        valueChanged = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SelectedObjectsManagement.Instance.GetActiveObjects();
        beforeDrag = new List<GameObject>(SelectedObjectsManagement.Instance.activeObjects);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!valueChanged)
            return;

        valueChanged = false;

        SelectedObjectsManagement.Instance.GetActiveObjects();
        List<GameObject> afterDrag = SelectedObjectsManagement.Instance.activeObjects;

        var shown = afterDrag.Where(it => !beforeDrag.Contains(it)).ToList();
        var hidden = beforeDrag.Where(it => !afterDrag.Contains(it)).ToList();
        if(shown.Count > 0)
            ActionControl.Instance.AddCommand(new ShowCommand(shown), false);
        if(hidden.Count > 0)
            ActionControl.Instance.AddCommand(new DeleteCommand(hidden), false);

    }
}
