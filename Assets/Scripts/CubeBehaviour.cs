using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CubeFace
{
    None,
    Up,
    Down,
    Right,
    Left,
    Front,
    Back
}
public class CubeBehaviour : MonoBehaviour
{
    public Transform target;
    public float cubeSize;
    public static CubeBehaviour instance;
    private Camera mainCamera;
    private CameraController camScript;
    public Vector2 cubePosition;
    [HideInInspector]
    public Vector2 originalCubePosition;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        originalCubePosition = cubePosition;
        mainCamera = Camera.main;
        camScript = mainCamera.GetComponent<CameraController>();
        if (!GlobalVariables.instance.mobile)
            cubePosition.y -= .2f;
    }

    void LateUpdate()
    {
        transform.LookAt(target);
        transform.localScale = Vector3.one * cubeSize * Camera.main.orthographicSize / 7;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(cubePosition.x * Camera.main.orthographicSize / 7, cubePosition.y * Camera.main.orthographicSize / 7);
    }

    public CubeFace GetHitFace(RaycastHit hit)
    {
        Vector3 incomingVec = hit.normal - Vector3.up;
        incomingVec.x = Mathf.Round(incomingVec.x);
        incomingVec.y = Mathf.Round(incomingVec.y);
        incomingVec.z = Mathf.Round(incomingVec.z);

        if (incomingVec == new Vector3(0, -1, -1))
            return CubeFace.Front;

        if (incomingVec == new Vector3(0, -1, 1))
            return CubeFace.Back;

        if (incomingVec == new Vector3(0, 0, 0))
            return CubeFace.Up;

        if (incomingVec == new Vector3(0, -2, 0))
            return CubeFace.Down;

        if (incomingVec == new Vector3(-1, -1, 0))
            return CubeFace.Right;

        if (incomingVec == new Vector3(1, -1, 0))
            return CubeFace.Left;

        return CubeFace.None;
    }

    public void SetCameraRotation(CubeFace orientation)
    {
        Vector3 angle = mainCamera.transform.eulerAngles;
        switch (orientation)
        {
            case CubeFace.Up:
               // angle = new Vector3(90, 0, -angle.y);
                angle = new Vector3(90, 0, 0);
                break;
            case CubeFace.Down:
                //angle = new Vector3(-90, 0, angle.y);
                angle = new Vector3(-90, 0, 0);
                break;
            case CubeFace.Right:
                angle = new Vector3(0, 90, 0);
                break;
            case CubeFace.Left:
                angle = new Vector3(0, -90, 0);
                break;
            case CubeFace.Front:
                angle = new Vector3(0, 0, 0);
                break;
            case CubeFace.Back:
                angle = new Vector3(0, 180, 0);
                break;
            default:
                break;
        }
        //camScript.CenterView(false);
        camScript.SetCameraRotation(angle);
    }
}
