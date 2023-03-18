using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to handle the name and description of a game object. 
/// It contains variables for storing the original name, all names (in different languages), all synonyms (in different languages), the description, and the side (left or right). 
/// It also contains methods for setting the description, translated name, and side name, and for checking if the game object has synonyms for the current language setting.
/// </summary>
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

    /// <summary>
    /// Sets the description of the game object by fetching it from the database.
    /// </summary>
    public void SetDescription()
    {
        ReadDB.instance.GetDescription(originalName.RemoveSuffix().ToLower(), name.ToLower().Replace("(r)", "").Replace("(l)", "").Trim().RemoveSuffix());
    }

    /// <summary>
    /// Sets the translated name of the game object based on the current language setting.
    /// </summary>
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

    /// <summary>
    /// Sets the name for the right or left side based on the given boolean value.
    /// </summary>
    /// <param name="right">A boolean indicating whether the name should be for the right side.</param>
    public void SetRightLeftName(bool right)
    {
        if (right)
            leftRight = " (R)";
        else
            leftRight = " (L)";
    }

    /// <summary>
    /// Checks if the game object has synonyms for the current language setting.
    /// </summary>
    /// <returns>A boolean indicating whether the game object has synonyms for the current language setting.</returns>
    public bool HasSynonims()
    {
        return allSynonyms != null && allSynonyms[Settings.languageIndex] != null;
    }

}
