#if !UNITY_WEBGL
using Firebase.Extensions;
using Firebase.Firestore;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class FirestoreAnalytics : MonoBehaviour
{
    private string today;
    private void Start()
    {
        today = DateTime.Today.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture);

        //If user has internet connection
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            #if !UNITY_EDITOR
                string userID = GetUserID();

                if (!PlayerPrefs.HasKey("NewUser"))
                    RegisterUser();

                IncrementSessions();
            #endif
        }
    }

    private void RegisterUser()
    {
        int todayValue = 0;
        int totalValue = 0;
        int webGLValue = 0;
        int windowsValue = 0;
#if UNITY_WEBGL
#else
        DocumentReference docRef = ReadFirestore.db.Collection("Analytics").Document("User acquisition");
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Dictionary<string, object> city = snapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in city)
                {
                    if (pair.Key == today)
                        todayValue = int.Parse(pair.Value.ToString()) + 1;
                    if (pair.Key == "Total count")
                        totalValue = int.Parse(pair.Value.ToString()) + 1;
                    if (pair.Key == "WebGL")
                    {
                        webGLValue = int.Parse(pair.Value.ToString());
                        if (Application.platform == RuntimePlatform.WebGLPlayer)
                            webGLValue++;
                    }
                    if (pair.Key == "Windows")
                    {
                        windowsValue = int.Parse(pair.Value.ToString());
                        if (Application.platform == RuntimePlatform.WindowsPlayer)
                            windowsValue++;
                    }
                }

                var data = new Dictionary<string, object>
                {
                    { "Total count", totalValue},
                    { "WebGL", webGLValue},
                    { "Windows", windowsValue},
                    { today, todayValue}
                };
                UpdateField("User acquisition", data);
            }
            else
            {
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
            }
        });
#endif
    }

    private void IncrementSessions()
    {
        int todayValue = 0;
#if UNITY_WEBGL
#else
        DocumentReference docRef = ReadFirestore.db.Collection("Analytics").Document("Sessions");
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Dictionary<string, object> city = snapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in city)
                {
                    if (pair.Key == today)
                    {
                        todayValue = int.Parse(pair.Value.ToString());
                        break;
                    }
                }

                var data = new Dictionary<string, object>
                {
                    { today, todayValue + 1}
                };
                UpdateField("Sessions", data);
            }
            else
            {
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
            }
        });
#endif
    }

    private string GetUserID()
    {
        string userID;
        if (PlayerPrefs.HasKey("UserID"))
            userID = PlayerPrefs.GetString("UserID");
        else
        {
            //Generate unique ID
            Guid guid = Guid.NewGuid();
            userID = guid.ToString();
            PlayerPrefs.GetString(userID);
        }
        return userID;
    }

    private void UpdateField(string document, Dictionary<string, object> data)
    {
#if UNITY_WEBGL
#else
        DocumentReference docRef = ReadFirestore.db.Collection("Analytics").Document(document);
        docRef.SetAsync(data, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Set value done");
                PlayerPrefs.SetInt("NewUser", 1);
            }
            else
                Debug.Log("Set value was faulted");
        });
#endif
    }

}
