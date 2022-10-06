using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleChangeColor : MonoBehaviour
{
    public Image imgToChange;
    public bool pressed;
    public bool overrideEnabledColor;
    public Color enabledColor;
    public bool overrideDisabledColor;
    public Color disabledColor;
    private void Start()
    {
        if (pressed)
            SetEnabledColor();
        else
            SetDisabledColor();
    }

    public void ChangeState()
    {
        pressed = !pressed;

        if (pressed)
        {
            if(overrideEnabledColor)
                imgToChange.color = enabledColor;
            else
                imgToChange.color = GlobalVariables.HighligthColor;

        }
        else
        {
            if (overrideDisabledColor)
                imgToChange.color = disabledColor;
            else
                imgToChange.color = GlobalVariables.IconColor;


        }
    }

    //For elements without toggle
    public void SetEnabledColor()
    {
        if(overrideEnabledColor)
            imgToChange.color = enabledColor;
        else
            imgToChange.color = GlobalVariables.HighligthColor;

        pressed = true;
    }

    //For elements without toggle
    public void SetDisabledColor()
    {
        if (overrideDisabledColor)
            imgToChange.color = disabledColor;
        else
            imgToChange.color = GlobalVariables.IconColor;

        pressed = false;
    }
}
