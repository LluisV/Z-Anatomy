using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleChangeColor : MonoBehaviour
{
    public Image imgToChange;
    public Color normalColor;
    public Color pressedColor;
    public bool pressed;

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
            imgToChange.color = pressedColor;
        else
            imgToChange.color = normalColor;
    }

    //For elements without toggle
    public void SetEnabledColor()
    {
        imgToChange.color = pressedColor;
        pressed = true;
    }

    //For elements without toggle
    public void SetDisabledColor()
    {
        imgToChange.color = normalColor;
        pressed = false;
    }
}
