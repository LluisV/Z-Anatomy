#if !UNITY_WEBGL
//using Firebase.Extensions;
//using Firebase.Firestore;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class UploadFrenchDefinitions : MonoBehaviour
{
    public List<TextAsset> descriptionFiles = new List<TextAsset>();

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        Upload();
    }

    private void Upload()
    {
#if !UNITY_WEBGL
        /*List<string> wrong = new List<string>();
        foreach (var file in descriptionFiles)
        {
            try
            {
                 DocumentReference docRef = ReadFirestore.db.Collection("Descriptions").Document(file.name.Replace("-FR", "").ToLower());
                 Dictionary<string, object> desc = new Dictionary<string, object>
                 {
                         { "French", file.text},
                         { "Checked", "English;French;"},
                 };
                 docRef.UpdateAsync(desc).ContinueWithOnMainThread(task => {
                     Debug.Log("Added " + file.name);
                 });
            }
            catch
            {
                Debug.LogError("Error uploading " + file);
            }
        }*/
#endif
    }
}
