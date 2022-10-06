using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_WEBGL
using Firebase;
using Firebase.RemoteConfig;
using Firebase.Extensions;
#endif
using System;
using TMPro;


public class RemoteConfig : MonoBehaviour
{

    public static RemoteConfig Instance;

    string localVersion;
    string updatedVersion;
    string downloadUrl;

    public GameObject UpdatePanel;
    public TextMeshProUGUI body;
    public TextMeshProUGUI releaseNote;

    private bool show;

    private void Awake()
    {
        Instance = this;
        show = PlayerPrefs.HasKey("Welcome");
    }

    public void Fetch()
    {
#if UNITY_WEBGL

#else
        localVersion = updatedVersion = Application.version;
        FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.FromHours(12)).ContinueWithOnMainThread(task => {
            FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(task => {
                updatedVersion = FirebaseRemoteConfig.DefaultInstance.GetValue("version").StringValue;
                releaseNote.text = FirebaseRemoteConfig.DefaultInstance.GetValue("releaseNote").StringValue;
                downloadUrl = FirebaseRemoteConfig.DefaultInstance.GetValue("downloadUrl").StringValue;
                if (string.Compare(localVersion, updatedVersion) < 0)
                    ShowUpdate();
            });
        });
#endif
    }

    private void ShowUpdate()
    {
        UpdatePanel.SetActive(true);
        body.text = body.text.Replace("{new_version}", updatedVersion).Replace("{old_version}",localVersion);
    }

    public void DowloadClick()
    {
        Application.OpenURL(downloadUrl);
    }
}
