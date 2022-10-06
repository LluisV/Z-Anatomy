using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetSurfaceColor : MonoBehaviour
{
    private Image img;
    public bool ignoreAlpha;

    private void Awake()
    {
        img = GetComponent<Image>();
        if (ignoreAlpha)
            img.color = new Color(GlobalVariables.SurfaceColor.r, GlobalVariables.SurfaceColor.g, GlobalVariables.SurfaceColor.b, 1);
        else
            img.color = GlobalVariables.SurfaceColor;

    }
}
