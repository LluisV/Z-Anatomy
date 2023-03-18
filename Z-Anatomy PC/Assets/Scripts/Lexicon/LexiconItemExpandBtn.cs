using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LexiconItemExpandBtn : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    static bool dragOpening;
    static bool dragClosing;
    LexiconElement LexeElement;

    private void Awake()
    {
        LexeElement = GetComponentInParent<LexiconElement>();
    }

    private void Update()
    {
        if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            dragOpening = false;
            dragClosing = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (LexeElement.opened)
        {
            LexeElement.Close(false);
            dragClosing = true;
        }
        else
        {
            LexeElement.Open();
            dragOpening = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (LexeElement.opened && dragClosing)
        {
            LexeElement.Close(false);
        }
        if (!LexeElement.opened && dragOpening)
        {
            LexeElement.Open();
        }

    }

}
