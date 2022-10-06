using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameAndDescription : MonoBehaviour
{
    public string[] allNames;
    public List<string[]> allSynonyms;
    [TextArea(15, 20)]
    public string description;

    [HideInInspector]
    public string originalName;

    public string leftRight = "";

    private void Awake()
    {
        originalName = gameObject.name.Trim();
        if(!CompareTag("Insertions"))
            gameObject.name = gameObject.name.RemoveSuffix();
    }


    public void SetDescription()
    {
        ReadFirestore.instance.GetDescription(originalName.RemoveSuffix().ToLower(), name.ToLower().Replace("(r)", "").Replace("(l)", "").Trim().RemoveSuffix());
    }

    public void SetTranslatedName()
    {
        try
        {
            if (allNames == null || allNames.Length == 0)
            {
                gameObject.name += leftRight;
                return;
            }
            string newName = gameObject.name;
            switch (Settings.language)
            {
                case SystemLanguage.English:
                    newName = allNames[0];
                    break;
                case SystemLanguage.Unknown:
                    //Latin
                    newName = allNames[1];
                    break;
                case SystemLanguage.French:
                    newName = allNames[2];
                    break;
                case SystemLanguage.Spanish:
                    newName = allNames[3];
                    break;
                case SystemLanguage.Portuguese:
                    newName = allNames[4];
                    break;
                default:
                    newName = allNames[0];
                    break;
            }          
            gameObject.name = (newName[0].ToString().ToUpper() + newName.Substring(1)) + leftRight;
        }
        catch
        {
            Debug.LogError("Error in name: " + name);
        }

    }

    public void SetRightLeftName(bool right)
    {
        if (right)
            leftRight = " (R)";
        else
            leftRight = " (L)";
    }


    public bool HasSynonims()
    {
        return allSynonyms != null && allSynonyms[Settings.languageIndex] != null;
    }

}
