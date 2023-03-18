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
    [TextArea(2, 20)]
    public string portugueseText;
    [TextArea(2, 20)]
    public string frenchText;
    private TextMeshProUGUI textField;
    private ExpandTextUI expandableText;
    private void Awake()
    {
        textField = GetComponent<TextMeshProUGUI>();
        expandableText = GetComponent<ExpandTextUI>();
        Translate();
    }

    public void Translate()
    {
        if (textField == null)
            textField = GetComponent<TextMeshProUGUI>();
        switch (Settings.language)
        {
            case SystemLanguage.Spanish:
                textField.text = spanishText;
                break;
            case SystemLanguage.Portuguese:
                textField.text = portugueseText;
                break;
            case SystemLanguage.French:
                textField.text = frenchText;
                break;
            default:
                textField.text = defaultText;
                break;
        }
        if (textField.text == "")
            textField.text = defaultText;
        if (expandableText != null)
            expandableText.text = textField.text;
    }
}
