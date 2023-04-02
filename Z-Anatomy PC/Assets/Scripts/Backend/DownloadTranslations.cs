        
#if !UNITY_WEBGL
//using Firebase.Storage;
//using Firebase.Extensions;
//using Firebase.RemoteConfig;
#endif

using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class DownloadTranslations : MonoBehaviour
{
    public static DownloadTranslations Instance;
#if !UNITY_WEBGL
    //FirebaseStorage storage;
#endif
    int newVersion = 0;
    int localVersion = 0;

    public GameObject downloadingPanel;
    public Slider percentageSlider;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GetNewTranslationsVersion();
    }

    private void GetNewTranslationsVersion()
    {
        localVersion = PlayerPrefs.GetInt("TranslationsVersion", 0);
        newVersion = localVersion;
        string localUrl = Application.persistentDataPath + "/Translations" + localVersion + ".txt";
        /*
#if UNITY_WEBGL
            LoadText(localUrl);
            NamesManagement.Instance.GetNamesTranslations();
#else
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {

            FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.FromHours(12)).ContinueWithOnMainThread(task =>
            {
                FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(task =>
                {

                    newVersion = (int)FirebaseRemoteConfig.DefaultInstance.GetValue("TranslationsVersion").DoubleValue;

                    //Download
                    if (newVersion > localVersion)
                    {
                        Debug.Log("Proceed to update translations...");
                        localUrl = Application.persistentDataPath + "/Translations" + newVersion + ".txt";
                        DownloadNewTranslationsVersion(localUrl);
                    }
                    //SetCurrent
                    else
                    {
                        Debug.Log("Translations are up to date. Not updating.");
                        if(File.Exists(localUrl))
                        {
                            LoadText(localUrl);
                            NamesManagement.Instance.GetNamesTranslations();
                        }
                        else
                        {
                            Debug.Log("File not found. Downloading again.");
                            DownloadNewTranslationsVersion(localUrl);
                        }
                    }
                });
            });

        }
        else
        {
            LoadText(localUrl);
            NamesManagement.Instance.GetNamesTranslations();
        }
#endif
        */
        LoadText(localUrl);
        NamesManagement.Instance.GetNamesTranslations();

    }

    private void DownloadNewTranslationsVersion(string localURL)
    {
        /*
//if UNITY_WEBGL

//else
        storage = FirebaseStorage.DefaultInstance;

        StorageReference storageRef = storage.GetReferenceFromUrl("gs://z-anatomy.appspot.com/Translations/Translations" + newVersion + ".txt");

        //If the file already exists
        if (File.Exists(localURL))
        {
            Debug.Log(localURL + " is already downloaded. Loading it.");
            LoadText(localURL);
            NamesManagement.Instance.GetNamesTranslations();
            return;
        }

        // Else tart downloading the file
        storageRef.GetFileAsync(localURL,
        new StorageProgress<DownloadState>(state =>
        {
            downloadingPanel.SetActive(true);

            Debug.Log(String.Format(
                "Progress: {0} of {1} bytes transferred.",
                state.BytesTransferred,
                state.TotalByteCount
            ));
            percentageSlider.value = 100f / state.TotalByteCount * state.BytesTransferred;
        })).ContinueWithOnMainThread(resultTask =>
        {
            if (resultTask.IsCompleted)
            {
                Debug.Log("The translation file has been successfully downloaded.");
                LoadText(localURL);
            }
            else
            {
                Debug.Log("The translation file WAS NOT downloaded");
                Debug.Log(resultTask.Exception.Source);
                LoadText(localURL);
            }

            downloadingPanel.SetActive(false);

            NamesManagement.Instance.GetNamesTranslations();
        });
        
//endif
        */
    }

    public void LoadText(string localURL)
    {
        if (File.Exists(localURL))
        {
            NamesManagement.Instance.translations = new TextAsset(File.ReadAllText(localURL));
            Debug.Log("Loaded translations: " + localURL);
            PlayerPrefs.SetInt("TranslationsVersion", newVersion);
        }
        else
        {
            Debug.Log("Translations file does not exist: " + localURL);
        }
        RemoteConfig.Instance.Fetch();
    }
}
