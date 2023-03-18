using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossPlanesGizmo : MonoBehaviour
{
    public static CrossPlanesGizmo Instance;

    public GameObject orientationGizmo;
    private bool crossSectionsEnabled;
    public CrossSections planeScript;
    [HideInInspector]
    public GizmoFace lastClick = GizmoFace.Left;
    private Color selectedColor;
    public CubeText X;
    public CubeText iX;
    public CubeText Y;
    public CubeText iY;
    public CubeText Z;
    public CubeText iZ;


    public RectTransform gizmoCanvasRT;
    private Vector2 cubeCanvasOriginalSize;

    [HideInInspector]
    public bool opened = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        selectedColor = GlobalVariables.HighligthColor;
        cubeCanvasOriginalSize = gizmoCanvasRT.sizeDelta;
    }

    public void OnClick()
    {
        crossSectionsEnabled = !crossSectionsEnabled;
        if (!crossSectionsEnabled)
        {
            ResetColors();
            planeScript.NoCutClick();
            CrossSections.Instance.sagitalToggle.SetDisabledColor();
            CrossSections.Instance.frontalToggle.SetDisabledColor();
            CrossSections.Instance.transversalToggle.SetDisabledColor();
            lastClick = GizmoFace.None;
        }
    }

    public void SetPlane(GizmoFace cubeFace)
    {
        ResetColors();
        EnableGizmoClick();
        switch (cubeFace)
        {
            case GizmoFace.Left:
                planeScript.XClick();
                X.SetSelectedColor(selectedColor);
                CrossSections.Instance.sagitalToggle.SetEnabledColor();
                CrossSections.Instance.frontalToggle.SetDisabledColor();
                CrossSections.Instance.transversalToggle.SetDisabledColor();
                break;
            case GizmoFace.Right:
                planeScript.InvertedXClick();
                iX.SetSelectedColor(selectedColor);
                CrossSections.Instance.sagitalToggle.SetEnabledColor();
                CrossSections.Instance.frontalToggle.SetDisabledColor();
                CrossSections.Instance.transversalToggle.SetDisabledColor();
                break;
            case GizmoFace.Front:
                planeScript.YClick();
                iY.SetSelectedColor(selectedColor);
                CrossSections.Instance.sagitalToggle.SetDisabledColor();
                CrossSections.Instance.frontalToggle.SetEnabledColor();
                CrossSections.Instance.transversalToggle.SetDisabledColor();
                break;
            case GizmoFace.Back:
                planeScript.InvertedYClick();
                Y.SetSelectedColor(selectedColor);
                CrossSections.Instance.sagitalToggle.SetDisabledColor();
                CrossSections.Instance.frontalToggle.SetEnabledColor();
                CrossSections.Instance.transversalToggle.SetDisabledColor();
                break;
            case GizmoFace.Up:
                planeScript.ZClick();
                Z.SetSelectedColor(selectedColor);
                CrossSections.Instance.sagitalToggle.SetDisabledColor();
                CrossSections.Instance.frontalToggle.SetDisabledColor();
                CrossSections.Instance.transversalToggle.SetEnabledColor();

                break;
            case GizmoFace.Down:
                planeScript.InvertedZClick();
                iZ.SetSelectedColor(selectedColor);
                CrossSections.Instance.sagitalToggle.SetDisabledColor();
                CrossSections.Instance.frontalToggle.SetDisabledColor();
                CrossSections.Instance.transversalToggle.SetEnabledColor();
                break;


            case GizmoFace.None:
                ActionControl.crossSectionsEnabled = false;
                DisableGizmoClick();
                planeScript.NoCutClick();
                CrossSections.Instance.sagitalToggle.SetDisabledColor();
                CrossSections.Instance.frontalToggle.SetDisabledColor();
                CrossSections.Instance.transversalToggle.SetDisabledColor();
                ResetColors();
                break;
        }

        if(cubeFace != GizmoFace.None)
            lastClick = cubeFace;
    }

    private void ResetColors()
    {
        X.SetOriginalColor();
        Y.SetOriginalColor();
        Z.SetOriginalColor();
        iX.SetOriginalColor();
        iY.SetOriginalColor();
        iZ.SetOriginalColor();
    }

    public void XClick()
    {
        if(planeScript.inverted)
            SetPlane(GizmoFace.Right);
        else
            SetPlane(GizmoFace.Left);
    }

    public void YClick()
    {
        if (planeScript.inverted)
            SetPlane(GizmoFace.Back);
        else
            SetPlane(GizmoFace.Front);
    }

    public void ZClick()
    {
        if (planeScript.inverted)
            SetPlane(GizmoFace.Down);
        else
            SetPlane(GizmoFace.Up);
    }

    public void InvertClick()
    {

        planeScript.inverted = !planeScript.inverted;
        switch (lastClick)
        {   
            case GizmoFace.None:
                break;
            case GizmoFace.Up:
                SetPlane(GizmoFace.Down);
                break;
            case GizmoFace.Down:
                SetPlane(GizmoFace.Up);
                break;
            case GizmoFace.Right:
                SetPlane(GizmoFace.Left);
                break;
            case GizmoFace.Left:
                SetPlane(GizmoFace.Right);
                break;
            case GizmoFace.Front:
                SetPlane(GizmoFace.Back);
                break;
            case GizmoFace.Back:
                SetPlane(GizmoFace.Front);
                break;
            default:
                break;
        }

        CrossSections.Instance.InvertSliders();

    }

    public void EnabelDisableGizmoClick()
    {
        ActionControl.crossSectionsEnabled = !ActionControl.crossSectionsEnabled;
        if (ActionControl.crossSectionsEnabled)
            EnableGizmoClick();
        else
            DisableGizmoClick();
    }

    public void EnableGizmoClick()
    {
        X.isPlanesCube = true;
        iX.isPlanesCube = true;
        Y.isPlanesCube = true;
        iY.isPlanesCube = true;
        Z.isPlanesCube = true;
        iZ.isPlanesCube = true;
    }

    public void DisableGizmoClick()
    {
        X.isPlanesCube = false;
        iX.isPlanesCube = false;
        Y.isPlanesCube = false;
        iY.isPlanesCube = false;
        Z.isPlanesCube = false;
        iZ.isPlanesCube = false;
    }

    public void OpenClosePlanesClick()
    {
        opened = !opened;

        if(opened)
        {
            SetPlane(lastClick);
            CrossSections.Instance.planesOptionsPanel.Expand();
        }
        else
        {
            SetPlane(GizmoFace.None);
            CrossSections.Instance.planesOptionsPanel.Collapse();
        }
    }

    public void ResetAll()
    {
        crossSectionsEnabled = false;
        ResetColors();
        planeScript.NoCutClick();
        orientationGizmo.GetComponent<RectTransform>().anchoredPosition = orientationGizmo.GetComponent<GizmoBehaviour>().originalCubePosition;
        orientationGizmo.transform.localScale = Vector3.one * 30;
        gizmoCanvasRT.sizeDelta = cubeCanvasOriginalSize;
        opened = false;
        lastClick = GizmoFace.Left;
    }

}
