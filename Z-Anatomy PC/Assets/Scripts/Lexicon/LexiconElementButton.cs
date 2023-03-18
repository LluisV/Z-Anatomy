using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;

public class LexiconElementButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private TextMeshProUGUI tmpro;
    private IncreaseHeigth increaseHeigth;
    private IncreaseHeigth textIncreaseHeigth;
    private LexiconElement viewElement;

    private void Awake()
    {
        tmpro = transform.parent.GetComponentInChildren<TextMeshProUGUI>();
        increaseHeigth = GetComponent<IncreaseHeigth>();
        textIncreaseHeigth = tmpro.GetComponent<IncreaseHeigth>();
        viewElement = GetComponentInParent<LexiconElement>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tmpro.enableWordWrapping = true;
        increaseHeigth.SetExpanded();
        textIncreaseHeigth.SetExpanded();
        viewElement.MouseEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tmpro.enableWordWrapping = false;
        increaseHeigth.SetNormal();
        textIncreaseHeigth.SetNormal();
        viewElement.MouseExit();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            viewElement.ElementClick();
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            viewElement.ShowContextualMenu();
        }
    }
}
