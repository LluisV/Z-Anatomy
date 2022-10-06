using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class ColorPicker : MonoBehaviour
{
    public Texture2D colorsTexture;
    public RectTransform colorsTextureRt;
    public Image picker;
    public TMP_InputField tmp_colorCode;
    public Image openColorPickerBtn;

    private bool mouseIn;
    private Color color = Color.white;

    private void Awake()
    {
        SaveColor();
    }

    private void Update()
    {
        if (mouseIn)
        {
            if(Mouse.current.leftButton.isPressed)
            {
                picker.transform.position = Mouse.current.position.ReadValue();
                SetColor();
                SaveColor();
            }
        }
    }

    public void MouseEnter()
    {
        mouseIn = true;
    }

    public void MouseExit()
    {
        mouseIn = false;
    }

    private void SetColor()
    {
        Vector2 delta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(colorsTextureRt, Mouse.current.position.ReadValue(), null, out delta);

        float width = colorsTextureRt.rect.width;
        float height = colorsTextureRt.rect.height;

        delta += new Vector2(width * 0.5f, height * 0.5f);

        float x = Mathf.Clamp01(delta.x / width);
        float y = Mathf.Clamp01(delta.y / height);

        int texX = (int)(x * colorsTexture.width);
        int texY = (int)(y * colorsTexture.height);

        color = colorsTexture.GetPixel(texX, texY);

        picker.color = color;
        Pencil.instance.color = color;
    }

    private void SaveColor()
    {
        tmp_colorCode.text = "#" + ColorUtility.ToHtmlStringRGB(color);
        Pencil.instance.color = color;

        openColorPickerBtn.color = color;
    }
}
