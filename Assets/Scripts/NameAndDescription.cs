using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameAndDescription : MonoBehaviour
{
    public string[] allNames;
    [TextArea(15, 20)]
    public string description;

    [HideInInspector]
    public string originalName;

    public string leftRight = "";

    private void Awake()
    {
        originalName = gameObject.name;
        if (originalName.Contains(".labels"))
            gameObject.SetActive(false);
        gameObject.name = gameObject.name.Replace(".r", "").Replace(".l", "").Replace("_l", "").Replace("_r", "");
    }


    public string Description()
    {
        string translatedDescription = GetTranslatedDescription(description);
        if (translatedDescription == null || translatedDescription.Length == 0)
        {
            switch (Settings.descriptionsLanguage)
            {
                case SystemLanguage.Spanish:
                    return "Esta definición aún no está disponible.";
                case SystemLanguage.English:
                    return "This definition is not yet available.";
                case SystemLanguage.Portuguese:
                    return "Esta definição ainda não está disponível.";
                case SystemLanguage.French:
                    return "Cette définition n'est pas encore disponible.";
                case SystemLanguage.Catalan:
                    return "Aquesta definició encara no està disponible.";
                default:
                    return "This definition is not yet available.";
            }
        }
        else
            return translatedDescription;
    }

    private string GetTranslatedDescription(string description)
    {
        try
        {
            string translated = null;
            //SPANISH
            if (Settings.descriptionsLanguage == SystemLanguage.Spanish)
            {

                int startIndex = description.IndexOf(";;;ES;;;");
                int endIndex = description.IndexOf(";;;FR;;;");
                int length = endIndex - startIndex;
                if (startIndex != -1 && endIndex != 1)
                    translated = description.Substring(startIndex + 9, length - 9);
            }
            //FRENCH
            else if (Settings.descriptionsLanguage == SystemLanguage.French)
            {

                int startIndex = description.IndexOf(";;;FR;;;");
                int endIndex = description.IndexOf(";;;PT;;;");
                int length = endIndex - startIndex;
                if (startIndex != -1 && endIndex != 1)
                    translated = description.Substring(startIndex + 9, length - 9);
            }
            //PORTUGUESE
            else if (Settings.descriptionsLanguage == SystemLanguage.Portuguese)
            {
                int startIndex = description.IndexOf(";;;PT;;;");
                if (startIndex != -1)
                    translated = description.Substring(startIndex + 9);
            }
            //ENGLISH
            else
            {
                int index = description.IndexOf(";;;ES;;;");
                if (index != -1)
                    translated = description.Substring(0, index);
            }
            return translated.Trim().Replace("   ", "\n\n").Replace("  ", "\n"); ;
        }
        catch
        {
            return null;
        }
    }

    public void SetTranslatedName()
    {
        try
        {
            if (allNames == null || allNames.Length == 0)
            {
                return;
            }
            string newName = gameObject.name;
            switch (Settings.namesLanguage)
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
                case SystemLanguage.Italian:
                    newName = allNames[5];
                    break;
                case SystemLanguage.Dutch:
                    newName = allNames[6];
                    break;
                case SystemLanguage.German:
                    newName = allNames[7];
                    break;
                case SystemLanguage.Polish:
                    newName = allNames[8];
                    break;
                case SystemLanguage.Chinese:
                    newName = allNames[9];
                    break;
                default:
                    newName = allNames[0];
                    break;
            }          
            gameObject.name = (newName[0].ToString().ToUpper() + newName.Substring(1)) + leftRight;
        }
        catch
        {

        }

    }

    public void SetRightLeftName(bool right)
    {
        if (right)
            leftRight = " (R)";
        else
            leftRight = " (L)";
    }

}
