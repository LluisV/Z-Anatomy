using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerByResolution : MonoBehaviour
{
    const int referenceWidth = 1920;
    public bool simulate;
    public int simulateWidth;
    private void Awake()
    {
#if UNITY_EDITOR
        if(simulate)
            GetComponent<CanvasScaler>().scaleFactor *= (float)simulateWidth / (float)referenceWidth;
#else
        GetComponent<CanvasScaler>().scaleFactor *= (float)Screen.currentResolution.width/ (float)referenceWidth;
#endif

    }
}
