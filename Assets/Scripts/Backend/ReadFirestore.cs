#if !UNITY_WEBGL
using Firebase.Extensions;
using Firebase.Firestore;
#endif
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class ReadFirestore : MonoBehaviour
{
    public static ReadFirestore instance;
#if !UNITY_WEBGL
    public static FirebaseFirestore db;
#endif

    private void Awake()
    {
        instance = this;
#if !UNITY_WEBGL
        db = FirebaseFirestore.DefaultInstance;
#endif
#if UNITY_EDITOR && !UNITY_WEBGL
        db.Settings.PersistenceEnabled = false;
#endif
    }

    public void GetDescription(string documentName, string translatedName)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            NamesManagement.Instance.NoConnectionScreen();
            return;
        }
#if UNITY_WEBGL

#else
        DocumentReference docRef = db.Collection("Descriptions").Document(documentName.Replace("*", ""));
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
                
            if (task.IsFaulted)
            {
                Debug.Log(String.Format("Error reading document {0}", task.Result.Id));
                NamesManagement.Instance.NoConnectionScreen();
            }
            else if (task.IsCompleted)
            {
                if(task.Result.Exists)
                {
                    var snapshot = task.Result;
                    Dictionary<string, object> description = snapshot.ToDictionary();
                    string language = Settings.language.ToString();
                    if (language == "Unknown")
                        language = "English";
                    string desc = (string)description[language];
                    if(description.ContainsKey("Checked"))
                    {
                        string check = (string)description["Checked"];
                        NamesManagement.Instance.SetDesc(desc, check.Contains(language), translatedName);
                    }
                    else
                        NamesManagement.Instance.SetDesc(desc, false, translatedName);
                }
                else
                {
                    Debug.Log(String.Format("Document {0} does not exist!", task.Result.Id));
                    NamesManagement.Instance.SetDesc(null);
                }

            }
        });
#endif
    }

    public class pageval
    {
        public int pageid { get; set; }
        public int ns { get; set; }
        public string title { get; set; }
        public string extract { get; set; }
    }


    public class Query
    {
        public Dictionary<string, pageval> pages { get; set; }
    }

    public class Limits
    {
        public int extracts { get; set; }
    }

    public class RootObject
    {
        public string batchcomplete { get; set; }
        public Query query { get; set; }
        public Limits limits { get; set; }
    }

    /*private void ReadWikipedia()
    {
        string LANG = "en";

        switch (Settings.language)
        {
            case SystemLanguage.English:
                LANG = "en";
                break;
            case SystemLanguage.Unknown:
                LANG = "en";
                break;
            case SystemLanguage.Spanish:
                LANG = "es";
                break;
            case SystemLanguage.French:
                LANG = "fr";
                break;
            case SystemLanguage.Portuguese:
                LANG = "pt";
                break;
            default:
                LANG = "en";
                break;
        }

        //https://www.mediawiki.org/wiki/Extension:TextExtracts#query+extracts

        string URL = "https://" + LANG + ".wikipedia.org/w/api.php";
        string parameters = "?action=query&titles=" + documentName + "&format=json&prop=extracts&explaintext&redirects&exintro";

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(URL);

        // Add an Accept header for JSON format.
        client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));

        // List data response.
        HttpResponseMessage response = client.GetAsync(parameters).Result;  
        if (response.IsSuccessStatusCode)
        {
            // Parse the response body.
            string dataObjects = response.Content.ReadAsStringAsync().Result; 
            RootObject m = JsonConvert.DeserializeObject<RootObject>(response.Content.ReadAsStringAsync().Result);

            string title = m.query.pages.First().Value.title;
            string desc = m.query.pages.First().Value.extract;

            if (desc == null || desc.Length == 0)
                NamesManagement.instance.SetDesc(null, documentName);
            else
                SetDescription(title, desc, documentName);
        }
        else
        {
            NamesManagement.instance.NoConnectionScreen();
        }

            
        client.Dispose();
    }*/

    /*private string RemoveExtras(string desc)
    {
        string[] fr = { "== Galerie ==", "== Voir aussi ==", "== Références ==", "== Notes et Références ==", "== Liens externes ==" };
        string[] es = { "== Imágenes adicionales ==", "== Véase también ==", "== Referencias ==", "== Bibliografía ==", "== Enlaces externos ==" };
        string[] pt = { "== Imagens ==", "== Ver também ==", "== Referências ==", "== Notas e referências ==", "== Ligações externas ==" };
        string[] en = { "== Images ==", " == Additional images ==", "== See also ==", "== References ==", "== Further reading == ", "== External links ==" };

        switch (Settings.language)
        {
            case SystemLanguage.English:
                foreach (var item in en)
                {
                    int index = desc.IndexOf(item);
                    if (index != -1)
                        return desc.Substring(0, index - item.Length);
                }
                break;
            case SystemLanguage.Spanish:
                foreach (var item in es)
                {
                    int index = desc.IndexOf(item);
                    if (index != -1)
                        return desc.Substring(0, index - item.Length);
                }
                break;
            case SystemLanguage.French:
                foreach (var item in fr)
                {
                    int index = desc.IndexOf(item);
                    if (index != -1)
                        return desc.Substring(0, index - item.Length);
                }
                break;
            case SystemLanguage.Portuguese:
                foreach (var item in pt)
                {
                    int index = desc.IndexOf(item);
                    if (index != -1)
                        return desc.Substring(0, index - item.Length);
                }
                break;
        }

        return desc;
    }*/

}
