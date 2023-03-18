using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScreenSize : MonoBehaviour
{
    RectTransform rt;
    public Canvas mainCanvas;
    RectTransform mainCanvasRT;
    private Vector2 resolution;
    private float proportion;
    private Camera mainCamera;
    private float lastDistance;
    void Start()
    {
        mainCamera = Camera.main;
        lastDistance = mainCamera.orthographicSize;
        resolution = new Vector2(Screen.width, Screen.height);
        rt = GetComponent<RectTransform>();
        mainCanvasRT = mainCanvas.GetComponent<RectTransform>();
        proportion = mainCanvasRT.sizeDelta.x / mainCanvasRT.sizeDelta.y;
        SetCanvasSize();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(lastDistance != mainCamera.orthographicSize || (Screen.height != resolution.y || Screen.width != resolution.x))
            SetCanvasSize();
    }

    void SetCanvasSize()
    {
        proportion = Screen.height / (mainCamera.orthographicSize * 2);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width / proportion);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mainCamera.orthographicSize * 2);
    }
}
