using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LexiconItemCheckbox: MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    static bool dragSelecting;
    static bool dragDeselecting;

    public ButtonSpriteSwap btn;
    public Sprite defaultImage;
    public Sprite pressedImage;

    private void Awake()
    {
        btn.defaultImage = defaultImage;
        btn.pressedImage = pressedImage;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            dragSelecting = false;
            dragDeselecting = false;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (btn.isEnabled)
        {
            btn.SwapImage();

            if (btn.isPressed)
            {
                GetComponentInParent<LexiconElement>().ShowElement();
                dragSelecting = true;
            }
            else
            {
                GetComponentInParent<LexiconElement>().HideElement();
                dragDeselecting = true;
            }
            Lexicon.Instance.UpdateTreeViewCheckboxes();
            SelectedObjectsManagement.Instance.GetActiveObjects();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (btn.isEnabled)
        {
            if (!btn.isPressed && dragSelecting)
            {
                btn.SwapImage();
                GetComponentInParent<LexiconElement>().ShowElement();
                SelectedObjectsManagement.Instance.GetActiveObjects();
                Lexicon.Instance.UpdateTreeViewCheckboxes();


            }
            if (btn.isPressed && dragDeselecting)
            {
                btn.SwapImage();
                GetComponentInParent<LexiconElement>().HideElement();
                SelectedObjectsManagement.Instance.GetActiveObjects();
                Lexicon.Instance.UpdateTreeViewCheckboxes();

            }

        }

    }
}
