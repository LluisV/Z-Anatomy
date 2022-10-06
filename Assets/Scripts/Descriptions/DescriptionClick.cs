using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DescriptionClick : MonoBehaviour
{
    public static DescriptionClick Instance;

    private TextMeshProUGUI descriptionTMPro;
    private TMP_InputField inputField;
    string link = null;
    Vector2 mousePos;

    [HideInInspector]
    public string selectedText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        descriptionTMPro = GetComponent<TextMeshProUGUI>();
        inputField = descriptionTMPro.GetComponentInParent<TMP_InputField>();
        inputField.onTextSelection.AddListener(GetSelectedText);
    }

    private void GetSelectedText(string str, int start, int end)
    {
        selectedText = str.Substring(Mathf.Min(start, end), Mathf.Abs(end - start));
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            try
            {
                int index = TMP_TextUtilities.FindIntersectingLink(descriptionTMPro, Mouse.current.position.ReadValue(), null);
                if (index != -1)
                {
                    mousePos = Mouse.current.position.ReadValue();
                    TMP_LinkInfo linkInfo = descriptionTMPro.textInfo.linkInfo[index];
                    link = linkInfo.GetLinkText();
                }
            }
            catch
            {

            }
        }
        else if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            float distance = Vector2.Distance(Mouse.current.position.ReadValue(), mousePos);
            if (link != null && distance < 5)
                NamesManagement.Instance.TextClicked(link, Mouse.current.leftButton.wasReleasedThisFrame);
            link = null;
        }  
        
    }
}
