using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DescriptionClick : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI descriptionTMPro;
    private CameraController cam;

    private void Start()
    {
        descriptionTMPro = GetComponent<TextMeshProUGUI>();
        cam = Camera.main.GetComponent<CameraController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int index = TMP_TextUtilities.FindIntersectingLink(descriptionTMPro, Input.mousePosition, null);
        if (index != -1)
        {
            TMP_LinkInfo linkInfo = descriptionTMPro.textInfo.linkInfo[index];
            NamesManagement.instance.TextClicked(linkInfo.GetLinkText(), true);
        }
    }
}
