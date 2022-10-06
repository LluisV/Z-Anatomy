using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GizmoFace
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
    private RectTransform rt;
    GizmoFace actualFace;

    private void Awake()
    {
        instance = this;
        rt = GetComponent<RectTransform>();
    }
    private void Start()
    {
        originalCubePosition = cubePosition;
        mainCamera = Camera.main;
        camScript = mainCamera.GetComponent<CameraController>();
    }

    void LateUpdate()
    {
        transform.LookAt(target);
        transform.localScale = Vector3.one * cubeSize * Camera.main.orthographicSize / 7;
        rt.anchoredPosition = new Vector2(cubePosition.x * Camera.main.orthographicSize / 7, cubePosition.y * Camera.main.orthographicSize / 7);

  
    }

    public void SetFaceShortcut(GizmoFace arrow)
    {
        if (!CameraController.instance.CanRotate())
            return;

        float left = Vector3.Dot(Camera.main.transform.forward, Vector3.left);
        float right = Vector3.Dot(Camera.main.transform.forward, Vector3.right);
        float up = Vector3.Dot(Camera.main.transform.forward, Vector3.up);
        float down = Vector3.Dot(Camera.main.transform.forward, Vector3.down);
        float forward = Vector3.Dot(Camera.main.transform.forward, Vector3.forward);
        float back = Vector3.Dot(Camera.main.transform.forward, Vector3.back);

        float currentOrientation = Mathf.Min(left, right, up, down, forward, back);

        if(currentOrientation == right)
        {
            switch (arrow)
            {   
                case GizmoFace.Up:
                    SetCameraRotation(GizmoFace.Up);
                    break;
                case GizmoFace.Down:
                    SetCameraRotation(GizmoFace.Down);
                    break;
                case GizmoFace.Right:
                    SetCameraRotation(GizmoFace.Back);
                    break;
                case GizmoFace.Left:
                    SetCameraRotation(GizmoFace.Front);
                    break;
            }
        }
        else if(currentOrientation == left)
        {
            switch (arrow)
            {
                case GizmoFace.Up:
                    SetCameraRotation(GizmoFace.Up);
                    break;
                case GizmoFace.Down:
                    SetCameraRotation(GizmoFace.Down);
                    break;
                case GizmoFace.Right:
                    SetCameraRotation(GizmoFace.Front);
                    break;
                case GizmoFace.Left:
                    SetCameraRotation(GizmoFace.Back);
                    break;
            }
        }
        else if (currentOrientation == up)
        {
            switch (arrow)
            {
                case GizmoFace.Up:
                    SetCameraRotation(GizmoFace.Back);
                    break;
                case GizmoFace.Down:
                    SetCameraRotation(GizmoFace.Front);
                    break;
                case GizmoFace.Right:
                    SetCameraRotation(GizmoFace.Left);
                    break;
                case GizmoFace.Left:
                    SetCameraRotation(GizmoFace.Right);
                    break;
            }
        }
        else if (currentOrientation == down)
        {
            switch (arrow)
            {
                case GizmoFace.Up:
                    SetCameraRotation(GizmoFace.Front);
                    break;
                case GizmoFace.Down:
                    SetCameraRotation(GizmoFace.Back);
                    break;
                case GizmoFace.Right:
                    SetCameraRotation(GizmoFace.Right);
                    break;
                case GizmoFace.Left:
                    SetCameraRotation(GizmoFace.Left);
                    break;
            }
        }
        else if (currentOrientation == forward)
        {
            switch (arrow)
            {
                case GizmoFace.Up:
                    SetCameraRotation(GizmoFace.Up);
                    break;
                case GizmoFace.Down:
                    SetCameraRotation(GizmoFace.Down);
                    break;
                case GizmoFace.Right:
                    SetCameraRotation(GizmoFace.Right);
                    break;
                case GizmoFace.Left:
                    SetCameraRotation(GizmoFace.Left);
                    break;
            }
        }
        else if (currentOrientation == back)
        {
            switch (arrow)
            {
                case GizmoFace.Up:
                    SetCameraRotation(GizmoFace.Up);
                    break;
                case GizmoFace.Down:
                    SetCameraRotation(GizmoFace.Down);
                    break;
                case GizmoFace.Right:
                    SetCameraRotation(GizmoFace.Left);
                    break;
                case GizmoFace.Left:
                    SetCameraRotation(GizmoFace.Right);
                    break;
            }
        }
    }

    public GizmoFace GetHitFace(RaycastHit hit)
    {
        Vector3 incomingVec = hit.normal - Vector3.up;
        incomingVec.x = Mathf.Round(incomingVec.x);
        incomingVec.y = Mathf.Round(incomingVec.y);
        incomingVec.z = Mathf.Round(incomingVec.z);

        if (incomingVec == new Vector3(0, -1, -1))
            return GizmoFace.Front;

        if (incomingVec == new Vector3(0, -1, 1))
            return GizmoFace.Back;

        if (incomingVec == new Vector3(0, 0, 0))
            return GizmoFace.Up;

        if (incomingVec == new Vector3(0, -2, 0))
            return GizmoFace.Down;

        if (incomingVec == new Vector3(-1, -1, 0))
            return GizmoFace.Right;

        if (incomingVec == new Vector3(1, -1, 0))
            return GizmoFace.Left;

        return GizmoFace.None;
    }

    public void SetCameraRotation(GizmoFace orientation)
    {
        if (!CameraController.instance.CanRotate())
            return;

        Vector3 angle = mainCamera.transform.eulerAngles;

        switch (orientation)
        {
            case GizmoFace.Up:
                if(actualFace == orientation)
                {
                    SetCameraRotation(GizmoFace.Down);
                    if (ActionControl.crossSectionsEnabled && Shortcuts.Instance.setPlaneGizmo.IsPressed())
                        CrossPlanesGizmo.Instance.SetPlane(GizmoFace.Down);
                    return;
                }
                angle = new Vector3(90, 0, 0);
                break;
            case GizmoFace.Down:
                if (actualFace == orientation)
                {
                    SetCameraRotation(GizmoFace.Up);
                    if (ActionControl.crossSectionsEnabled && Shortcuts.Instance.setPlaneGizmo.IsPressed())
                        CrossPlanesGizmo.Instance.SetPlane(GizmoFace.Up);
                    return;
                }
                angle = new Vector3(-90, 0, 0);
                break;
            case GizmoFace.Right:
                if (actualFace == orientation)
                {
                    SetCameraRotation(GizmoFace.Left);
                    if (ActionControl.crossSectionsEnabled && Shortcuts.Instance.setPlaneGizmo.IsPressed())
                        CrossPlanesGizmo.Instance.SetPlane(GizmoFace.Left);
                    return;
                }
                angle = new Vector3(0, 90, 0);
                break;
            case GizmoFace.Left:
                if (actualFace == orientation)
                {
                    SetCameraRotation(GizmoFace.Right);
                    if (ActionControl.crossSectionsEnabled && Shortcuts.Instance.setPlaneGizmo.IsPressed())
                        CrossPlanesGizmo.Instance.SetPlane(GizmoFace.Right);
                    return;
                }
                angle = new Vector3(0, -90, 0);
                break;
            case GizmoFace.Front:
                if (actualFace == orientation)
                {
                    SetCameraRotation(GizmoFace.Back);
                    if (ActionControl.crossSectionsEnabled && Shortcuts.Instance.setPlaneGizmo.IsPressed())
                        CrossPlanesGizmo.Instance.SetPlane(GizmoFace.Back);
                    return;
                }
                angle = new Vector3(0, 0, 0);
                break;
            case GizmoFace.Back:
                if (actualFace == orientation)
                {
                    SetCameraRotation(GizmoFace.Front);
                    if (ActionControl.crossSectionsEnabled && Shortcuts.Instance.setPlaneGizmo.IsPressed())
                        CrossPlanesGizmo.Instance.SetPlane(GizmoFace.Front);
                    return;
                }
                angle = new Vector3(0, 180, 0);
                break;
            default:
                break;
        }

        actualFace = orientation;
        camScript.SetCameraRotation(angle);
    }

    public void HasRotated()
    {
        actualFace = GizmoFace.None;
    }
}
