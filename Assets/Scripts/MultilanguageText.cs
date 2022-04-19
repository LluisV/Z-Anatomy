using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultilanguageText : MonoBehaviour
{
    [TextArea(2, 20)]
    public string defaultText;
    [TextArea(2, 20)]
    public string spanishText;

    private TextMeshProUGUI textField;

    private void Start()
    {
        textField = GetComponent<TextMeshProUGUI>();
    }

    public void Translate()
    {
        if (textField == null)
            textField = GetComponent<TextMeshProUGUI>();
        switch (Settings.namesLanguage)
        {
            case SystemLanguage.Spanish:
                textField.text = spanishText;
                break;
            default:
                textField.text = defaultText;
                break;
        }
    }
}
