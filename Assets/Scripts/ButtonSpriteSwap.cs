using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteSwap : MonoBehaviour
{
    [SerializeField]
    private Sprite defaultImage;
    [SerializeField]
    private Sprite pressedImage;
    private Button btn;
    [HideInInspector]
    public bool isPressed;
    [HideInInspector]
    public bool isEnabled;

    private void Awake()
    {
        isEnabled = true;
        btn = GetComponent<Button>();
    }

    public void Check()
    {
        if (btn == null)
            btn = GetComponent<Button>();
        isPressed = true;
        btn.image.sprite = pressedImage;
    }

    public void Uncheck()
    {
        if (btn == null)
            btn = GetComponent<Button>();
        isPressed = false;
        btn.image.sprite = defaultImage;
    }

    public void SwapImage()
    {
        isPressed = !isPressed;
        if (btn == null)
            btn = GetComponent<Button>();
        if (isPressed)
            btn.image.sprite = pressedImage;
        else
            btn.image.sprite = defaultImage;
    }

    public void OnClick()
    {
        if(isEnabled)
        {
            SwapImage();
            if (isPressed)
                GetComponentInParent<TreeViewElement>().ShowElement();
            else
                GetComponentInParent<TreeViewElement>().HideElement();
        }
    }
}
