using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RectTransformChangeListener : MonoBehaviour
{
    public UnityEvent onChange;

    private void OnRectTransformDimensionsChange()
    {
        onChange.Invoke();
    }
}
