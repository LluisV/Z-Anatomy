using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerButton : MonoBehaviour
{
    public CreateCollectionPanel createCollectionScript;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnClick()
    {
        createCollectionScript.SetColor(image.color);
    }
}
