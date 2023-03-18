using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SliderPercentage : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Slider slider;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        slider = GetComponentInParent<Slider>();

        slider.onValueChanged.AddListener(delegate { UpdatePercentage(); });
    }

    private void UpdatePercentage()
    {
        text.text = slider.value + "%";
    }


}
