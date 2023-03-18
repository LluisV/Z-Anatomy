using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSpriteSwap : Button, IPointerClickHandler
{
    public Sprite defaultImage;
    public Sprite pressedImage;
    private Button btn;
    [HideInInspector]
    public bool isPressed;
    [HideInInspector]
    public bool isEnabled;

    protected override void Awake()
    {
        isEnabled = true;
        btn = GetComponent<Button>();
    }

    public void Check()
    {
        if (!isEnabled)
            return;

        if (btn == null)
            btn = GetComponent<Button>();
        isPressed = true;
        btn.image.sprite = pressedImage;
    }

    public void Uncheck()
    {
        if (!isEnabled)
            return;

        if (btn == null)
            btn = GetComponent<Button>();
        isPressed = false;
        btn.image.sprite = defaultImage;
    }

    public void SwapImage()
    {
        if (!isEnabled)
            return;

        isPressed = !isPressed;
        if (btn == null)
            btn = GetComponent<Button>();
        if (isPressed)
            btn.image.sprite = pressedImage;
        else
            btn.image.sprite = defaultImage;
    }

    private void OnMouseDown()
    {
        if (isEnabled)
        {
            SwapImage();
        }
    }
}
