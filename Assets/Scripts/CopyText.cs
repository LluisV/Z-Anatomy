using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CopyText : MonoBehaviour
{
    private TMP_InputField inputField;


    private void Awake()
    {
        inputField = GetComponentInChildren<TMP_InputField>();

    }

    public void OnPointerClick(BaseEventData eventData)
    {
        if (((PointerEventData)(eventData)).button == PointerEventData.InputButton.Right && inputField.selectionAnchorPosition != inputField.selectionFocusPosition)
        {
            ContextualMenu.Instance.Show();
            ContextualMenu.Instance.ShowCopyBtn();

        }

    }
}
