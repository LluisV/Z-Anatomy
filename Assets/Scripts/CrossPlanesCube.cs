using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossPlanesCube : MonoBehaviour
{
    public GameObject orientationCube;
    public GameObject crossSectionsCube;
    private bool crossSectionsEnabled;
    public OnPlaneClick planeScript;
    private CubeFace lastClick;
    public static CrossPlanesCube instance;
    public Color selectedColor;
  /*  public Fade xScissors;
    public Fade IxScissors;
    public Fade yScissors;
    public Fade IyScissors;
    public Fade zScissors;
    public Fade IzScissors;*/
    public CubeText X;
    public CubeText iX;
    public CubeText Y;
    public CubeText iY;
    public CubeText Z;
    public CubeText iZ;
    public ToggleChangeColor Xtoggle;
    public ToggleChangeColor Ytoggle;
    public ToggleChangeColor Ztoggle;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        crossSectionsCube.SetActive(false);
    }

    public void OnClick()
    {
        crossSectionsEnabled = !crossSectionsEnabled;
        ActionControl.crossSectionsEnabled = crossSectionsEnabled;
        orientationCube.gameObject.SetActive(!crossSectionsEnabled);
        crossSectionsCube.gameObject.SetActive(crossSectionsEnabled);
        if (!crossSectionsEnabled)
        {
            ResetColors();
            planeScript.NoCutClick();
            Xtoggle.SetDisabledColor();
            Ytoggle.SetDisabledColor();
            Ztoggle.SetDisabledColor();
            lastClick = CubeFace.None;
        }

    }

    public void SetPlane(CubeFace cubeFace)
    {
        ResetColors();
        switch (cubeFace)
        {
            case CubeFace.Left:
                planeScript.XClick();
                //xScissors.ColorChanged(selectedColor);
                X.ColorChanged(selectedColor);
                Xtoggle.SetEnabledColor();
                Ytoggle.SetDisabledColor();
                Ztoggle.SetDisabledColor();
                break;
            case CubeFace.Right:
                planeScript.InvertedXClick();
                //IxScissors.ColorChanged(selectedColor);
                iX.ColorChanged(selectedColor);
                Xtoggle.SetEnabledColor();
                Ytoggle.SetDisabledColor();
                Ztoggle.SetDisabledColor();
                break;
            case CubeFace.Front:
                planeScript.YClick();
               //yScissors.ColorChanged(selectedColor);
                Y.ColorChanged(selectedColor);
                Xtoggle.SetDisabledColor();
                Ytoggle.SetEnabledColor();
                Ztoggle.SetDisabledColor();
                break;
            case CubeFace.Back:
                planeScript.InvertedYClick();
                //IyScissors.ColorChanged(selectedColor);
                iY.ColorChanged(selectedColor);
                Xtoggle.SetDisabledColor();
                Ytoggle.SetEnabledColor();
                Ztoggle.SetDisabledColor();
                break;
            case CubeFace.Up:
                planeScript.ZClick();
                //zScissors.ColorChanged(selectedColor);
                Z.ColorChanged(selectedColor);
                Xtoggle.SetDisabledColor();
                Ytoggle.SetDisabledColor();
                Ztoggle.SetEnabledColor();
                break;
            case CubeFace.Down:
                planeScript.InvertedZClick();
                //IzScissors.ColorChanged(selectedColor);
                iZ.ColorChanged(selectedColor);
                Xtoggle.SetDisabledColor();
                Ytoggle.SetDisabledColor();
                Ztoggle.SetEnabledColor();
                break;
            case CubeFace.None:
                lastClick = CubeFace.None;
                planeScript.NoCutClick();
                Xtoggle.SetDisabledColor();
                Ytoggle.SetDisabledColor();
                Ztoggle.SetDisabledColor();
                ResetColors();
                break;
        }

        if (lastClick == cubeFace)
        {
            lastClick = CubeFace.None;
            planeScript.NoCutClick();
            Xtoggle.SetDisabledColor();
            Ytoggle.SetDisabledColor();
            Ztoggle.SetDisabledColor();
            ResetColors();
        } 
        else
            lastClick = cubeFace;
    }

    private void ResetColors()
    {
       /* xScissors.SetOriginalColor();
        yScissors.SetOriginalColor();
        zScissors.SetOriginalColor();
        IxScissors.SetOriginalColor();
        IyScissors.SetOriginalColor();
        IzScissors.SetOriginalColor();*/
        X.SetOriginalColor();
        Y.SetOriginalColor();
        Z.SetOriginalColor();
        iX.SetOriginalColor();
        iY.SetOriginalColor();
        iZ.SetOriginalColor();
    }

    public void XClick()
    {
        if(lastClick == CubeFace.Right || lastClick == CubeFace.Left)
            SetPlane(CubeFace.None);
        else if(planeScript.inverted)
            SetPlane(CubeFace.Right);
        else
            SetPlane(CubeFace.Left);
    }

    public void YClick()
    {
        if (lastClick == CubeFace.Front || lastClick == CubeFace.Back)
            SetPlane(CubeFace.None);
        else if (planeScript.inverted)
            SetPlane(CubeFace.Back);
        else
            SetPlane(CubeFace.Front);
    }

    public void ZClick()
    {
        if (lastClick == CubeFace.Up || lastClick == CubeFace.Down)
            SetPlane(CubeFace.None);
        else if (planeScript.inverted)
            SetPlane(CubeFace.Down);
        else
            SetPlane(CubeFace.Up);

    }

    public void InvertClick()
    {
        switch (lastClick)
        {   
            case CubeFace.None:
                break;
            case CubeFace.Up:
                SetPlane(CubeFace.Down);
                break;
            case CubeFace.Down:
                SetPlane(CubeFace.Up);
                break;
            case CubeFace.Right:
                SetPlane(CubeFace.Left);
                break;
            case CubeFace.Left:
                SetPlane(CubeFace.Right);
                break;
            case CubeFace.Front:
                SetPlane(CubeFace.Back);
                break;
            case CubeFace.Back:
                SetPlane(CubeFace.Front);
                break;
            default:
                break;
        }
    }

}
