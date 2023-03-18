using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class CustomSlider : Slider
{
    private TMP_InputField valueField;

    protected override void Awake()
    {
        valueField = GetComponentInChildren<TMP_InputField>();
        onValueChanged.AddListener(delegate { SetTextValue(); });
    }

    public void SetTextValue()
    {
        valueField.text = value.ToString("F1");
    }

    public void SetSliderValue()
    {
        float v;
        if(float.TryParse(valueField.text, out v))
            value = v;
    }


}
