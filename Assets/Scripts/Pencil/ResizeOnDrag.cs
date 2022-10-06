using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ResizeOnDrag : MonoBehaviour
{
    private bool mouseIn;
    private bool dragging;
    public RectTransform toResize;
    public Texture2D cursorTexture;
    private Vector2 delta = Vector2.zero;
    private Vector2 lastPos = Vector2.zero;

    public float minX;
    public float maxX;

    public float minY;
    public float maxY;

    private void Awake()
    {
        cursorTexture.Reinitialize(1, 1);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            dragging = mouseIn;
            lastPos = Mouse.current.position.ReadValue();
        }

        else if (dragging && Mouse.current.leftButton.isPressed)
        {
            dragging = true;
            delta = Mouse.current.position.ReadValue() - lastPos;

            float newWidth = toResize.GetWidth() + delta.x;
            if(newWidth > minX && newWidth < maxX)
                toResize.SetWidth(newWidth);

            float newHeight = toResize.GetHeight() - delta.y;
            if(newHeight > minY && newHeight < maxY)
                toResize.SetHeight(newHeight);

            lastPos = Mouse.current.position.ReadValue();
        }

        else if (dragging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            dragging = false;
            if (cursorTexture != null)
                Cursor.SetCursor(null, new Vector2(), CursorMode.Auto);
        }

    }

    public void MouseEnter()
    {
        mouseIn = true;
       /* if(cursorTexture != null)
            Cursor.SetCursor(cursorTexture, new Vector2(), CursorMode.Auto);*/
    }

    public void MouseExit()
    {
        mouseIn = false;
        if (!dragging && cursorTexture != null)
            Cursor.SetCursor(null, new Vector2(), CursorMode.Auto);
    }
}
