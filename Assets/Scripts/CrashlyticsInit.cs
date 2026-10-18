using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashlyticsInit : MonoBehaviour
{
    void Start()
    {
#if FIREBASE_ENABLED
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
#endif
    }
}
