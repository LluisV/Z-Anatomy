using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadLocalDefinitions : MonoBehaviour
{
//#if UNITY_WEBGL
    public static ReadLocalDefinitions Instance;

    public TextAsset[] descriptions;
    public TextAsset[] FRdescriptions;
    private Dictionary<string, TextAsset> descDictionary = new Dictionary<string, TextAsset>();
    private Dictionary<string, TextAsset> FRdescDictionary = new Dictionary<string, TextAsset>();

    private void Awake()
    {
        Instance = this;

        foreach (var item in descriptions)
        {
            descDictionary.Add(item.name.ToLower(), item);
        }
        foreach (var item in FRdescriptions)
        {
            FRdescDictionary.Add(item.name.Replace("-FR", "").ToLower(), item);
        }
    }

    public string GetDescription(string fileName, ref bool check)
    {

        if (Settings.language == SystemLanguage.French && FRdescDictionary.ContainsKey(fileName))
        {
            check = true;
            return GetTranslatedDescription(FRdescDictionary[fileName].text, Settings.language);
        }
        else if (descDictionary.ContainsKey(fileName))
        {
            check = Settings.language == SystemLanguage.English;
            return GetTranslatedDescription(descDictionary[fileName].text, Settings.language);
        }
        else
        {
            check = true;
            return null;
        }
    }

    private string GetTranslatedDescription(string description, SystemLanguage language)
    {
        try
        {
            string translated = null;
            //SPANISH
            if (language == SystemLanguage.Spanish)
            {

                int startIndex = description.IndexOf(";;;ES;;;");
                int endIndex = description.IndexOf(";;;", description.IndexOf(";;;ES;;;") + 9);
                int length = endIndex - startIndex;
                if (startIndex != -1 && endIndex != -1)
                    translated = description.Substring(startIndex + 9, length - 9);
                else
                    translated = description.Substring(startIndex + 9, description.Length - startIndex - 9);

            }
            //FRENCH
            else if (language == SystemLanguage.French)
            {

                int startIndex = description.IndexOf(";;;FR;;;");
                int endIndex = description.IndexOf(";;;", description.IndexOf(";;;FR;;;") + 9);

                int length = endIndex - startIndex;
                if (startIndex != -1 && endIndex != -1)
                    translated = description.Substring(startIndex + 9, length - 9);
                else
                    translated = description.Substring(startIndex + 9, description.Length - startIndex - 9);
            }
            //PORTUGUESE
            else if (language == SystemLanguage.Portuguese)
            {
                int startIndex = description.IndexOf(";;;PT;;;");
                int endIndex = description.IndexOf(";;;", description.IndexOf(";;;PT;;;") + 9);

                int length = endIndex - startIndex;
                if (startIndex != -1 && endIndex != -1)
                    translated = description.Substring(startIndex + 9, length - 9);
                else
                    translated = description.Substring(startIndex + 9, description.Length - startIndex - 9);
            }
            //ENGLISH
            else
            {
                int startIndex = 0;
                int endIndex = description.IndexOf(";;;");
                int length = endIndex - startIndex;
                if (startIndex != -1 && endIndex != -1)
                    translated = description.Substring(startIndex, length);
            }
            return translated.Trim().Replace("   ", "\n\n").Replace("  ", "\n"); ;
        }
        catch
        {
            return null;
        }
    }

//#endif
}
