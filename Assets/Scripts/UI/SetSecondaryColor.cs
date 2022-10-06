using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetSecondaryColor : MonoBehaviour
{
    private Image img;
    public bool ignoreAlpha;

    private void Awake()
    {
        img = GetComponent<Image>();
        if (ignoreAlpha)
            img.color = new Color(GlobalVariables.SecondaryColor.r, GlobalVariables.SecondaryColor.g, GlobalVariables.SecondaryColor.b, 1);
        else
            img.color = GlobalVariables.SecondaryColor;

    }
}
