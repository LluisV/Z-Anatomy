
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_WEBGL
using Firebase.Extensions;
using Firebase.Firestore;
#endif


public class UploadDescriptions : MonoBehaviour
{
    public List<TextAsset> descriptionFiles = new List<TextAsset>();

    private void Start()
    {
        Upload();
    }

    private void Upload()
    {
#if UNITY_WEBGL
#else
        foreach (var file in descriptionFiles)
        {
            DocumentReference docRef = ReadFirestore.db.Collection("Descriptions").Document(file.name.ToLower());
            Dictionary<string, object> desc = new Dictionary<string, object>
            {
                    { "English", GetTranslatedDescription(file.text, SystemLanguage.English)},
                    { "French", GetTranslatedDescription(file.text, SystemLanguage.French)},
                    { "Spanish", GetTranslatedDescription(file.text, SystemLanguage.Spanish)},
                    { "Portuguese", GetTranslatedDescription(file.text, SystemLanguage.Portuguese)},
                    { "Checked", "English;"},
            };
            docRef.SetAsync(desc).ContinueWithOnMainThread(task => {
                Debug.Log("Added " + file.name);
            });
        }
#endif
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
}
