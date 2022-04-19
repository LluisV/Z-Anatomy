using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using TMPro;

public class CreatePrefabs : MonoBehaviour
{

/*[MenuItem("Prefabs/Create Simple Prefab")]
static void DoCreateSimplePrefab()
{
    Transform[] transforms = Selection.transforms;
    foreach (Transform t in transforms)
    {
        Object prefab = EditorUtility.CreateEmptyPrefab("Assets/Models/" + t.gameObject.name + ".prefab");
        EditorUtility.ReplacePrefab(t.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
    }
}

    [MenuItem("Prefabs/Process Model")]
 static void ProcessModel()
 {
     GameObject[] selectedObjects = Selection.gameObjects;
     try
     {
         AddScriptAndMesh(selectedObjects);
         Lines(selectedObjects);
         GLines(selectedObjects);
         //Invert(selectedObjects);
     }
     catch (System.Exception)
     {
         throw;
     }
 }


 [MenuItem("Prefabs/Disable labels")]
 static void DisableLabels()
 {
     GameObject[] labels = FindObjectsOfType<GameObject>().Where(it => it.name.Contains("labels")).ToArray();
     try
     {
         foreach (var item in labels)
         {
             item.SetActive(false);
         }
     }
     catch (System.Exception)
     {
         throw;
     }
 }

 [MenuItem("Prefabs/Disable groups (Select root only)")]
 static void DisableGroups()
 {
     GameObject selectedObject = Selection.gameObjects[0];
     try
     {
         RecursiveChildren(selectedObject.transform);
     }
     catch (System.Exception)
     {
         throw;
     }
 }

 static void RecursiveChildren(Transform parent)
 {
     try
     {
         foreach (Transform child in parent)
         {
             if (child.name.Contains(".gLabel") && DeepLevel(child, 0) > 4)
             {
                 parent.Find(child.gameObject.name.Replace(".gLabel", "-line")).gameObject.SetActive(false);
                 child.gameObject.SetActive(false);
             }
             if (child.childCount > 0)
                 RecursiveChildren(child);
         }
     }
     catch (System.Exception)
     {
         throw;
     }
 }

 static int DeepLevel(Transform child, int deepLevel)
 {
     if(child.parent != null)
     {
         return DeepLevel(child.parent, ++deepLevel);
     }
     return deepLevel;
 }

 private static void AddScriptAndMesh(GameObject[] gameObjects)
 {
     string actual = "";
     try
     {
         foreach (GameObject child in gameObjects)
         {
             actual = child.name;
             if (child != null && !child.transform.name.Contains(".t") && !child.transform.name.Contains(".g") && !child.transform.name.Contains("-lin") && child.GetComponent<MeshRenderer>() != null && child.GetComponent<MeshRenderer>().sharedMaterial.name != "Lit")
             {
                 BodyPart script = child.AddComponent<BodyPart>();
                 if (script == null)
                     continue;
                 child.AddComponent<MeshCollider>();
             }
         }
     }
     catch (System.Exception e)
     {
         Debug.Log("Error adding script and mesh to " + actual + ": " + e.Message);
     }
 }

 private static void Lines(GameObject[] gameObjects)
 {
     try
     {
         foreach (GameObject child in gameObjects)
         {
             child.gameObject.layer = LayerMask.NameToLayer("Body");
             if (child.name.Contains("-lin"))
             {
                 CreateLinePoints(child.gameObject);
                 if (child.transform.parent.name.Contains(".t"))
                     Deparent(child.gameObject);
                 Line script = child.gameObject.AddComponent<Line>();
                 script.cam = Camera.main;
                 script.lineMaterial = (Material)Resources.Load("LineMaterial", typeof(Material));
                 script.camScript = FindObjectOfType<CameraController>();
             }
             else if (child.name.Contains(".t"))
             {
                 DestroyImmediate(child.gameObject.GetComponent<MeshRenderer>());
                 DestroyImmediate(child.gameObject.GetComponent<MeshFilter>());
                 child.gameObject.AddComponent<TextMeshPro>();
                 Label script = child.gameObject.AddComponent<Label>();
                 script.labelMaterial = (Material)Resources.Load("LabelMaterial", typeof(Material));
                 script.cam = Camera.main;
                 script.camScript = FindObjectOfType<CameraController>();
             }
         }
     }
     catch (System.Exception e)
     {
         Debug.Log("Error reading childs of: " + e.Message);
     }

 }

 private static void GLines(GameObject[] gameObjects)
 {
     string name = "";
     try
     {
         foreach (GameObject child in gameObjects)
         {
             name = child.name;
             if (child.name.Contains(".g"))
             {
                 GameObject go = GameObject.Find(child.name.Replace(".g", "-line"));
                 if(go != null)
                 {
                     Transform line = go.transform;
                     if (line != null)
                     {
                         Transform maxPoint = line.Find("maxPoint");
                         Transform minPoint = line.Find("minPoint");
                         if (maxPoint == null || minPoint == null)
                             continue;
                         Vector3 newPos;
                         if (Vector3.Distance(maxPoint.position, child.transform.position) < Vector3.Distance(minPoint.position, child.transform.position))
                             newPos = maxPoint.position;
                         else
                             newPos = minPoint.position;

                         if (maxPoint != null)
                         {
                             GameObject newLabel = new GameObject();
                             newLabel.name = child.name.Replace(".g", ".gLabel");
                             newLabel.transform.SetParent(child.transform);
                             newLabel.gameObject.AddComponent<TextMeshPro>();
                             newLabel.transform.position = newPos;
                             newLabel.layer = LayerMask.NameToLayer("Body");
                             LabelGroup script = newLabel.gameObject.AddComponent<LabelGroup>();
                             script.labelMaterial = (Material)Resources.Load("LabelMaterial", typeof(Material));
                             script.hierarchyLevel = DeepLevel(newLabel.transform, 0);
                             script.cam = Camera.main;
                             script.camScript = FindObjectOfType<CameraController>();
                         }
                     }
                 }

             }
         }
     }
     catch (System.Exception e)
     {
         Debug.Log("Error reading childs of: " + name + ", " + e.Message);
     }

 }

 private static void Invert(GameObject[] objects)
 {
     try
     {
         foreach (GameObject obj in objects)
         {
             if (obj != null && obj.name.Contains(".r") && obj.transform.childCount < 2)
             {
                 //Clone the right one
                 GameObject clone = Instantiate(obj, GameObject.Find(obj.transform.parent.name.Replace(".r", ".l")).transform);
                // clone.transform.localScale = obj.transform.localScale;
               //  clone.transform.position = obj.transform.position;
                // clone.transform.rotation = obj.transform.rotation;

                 if(obj.transform.childCount < 2)
                     InvertPosition(clone);

                 DestroyImmediate(GameObject.Find(obj.name.Replace(".r", ".l")));
                 DestroyImmediate(GameObject.Find(obj.name.Replace(".r", "")));
                 Rename(clone);
             }
         }
     }
     catch (System.Exception e)
     {
         Debug.Log("Error reading childs: " + e.Message);
     }

 }

 private static void Deparent(GameObject gameObject)
 {
     try
     {
         GameObject parent = GameObject.Find(gameObject.transform.parent.parent.name + ".labels");
         if (parent == null)
         {
             parent = new GameObject();
             parent.transform.SetParent(gameObject.transform.parent.parent);
         }
         gameObject.transform.parent.transform.SetParent(parent.transform);
         gameObject.transform.SetParent(parent.transform);
         parent.name = gameObject.transform.parent.parent.name + ".labels";
     }
     catch (System.Exception e)
     {
         Debug.Log("Error creating parent of labels of " + gameObject.name + ": " + e.Message);
     }
 }

 private static void CreateLinePoints(GameObject gameObject)
 {
     try
     {
         string name = gameObject.name;
         Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
         if (mesh.vertices.Length == 0)
             return;
         Vector3 min = mesh.vertices[0];
         Vector3 max = mesh.vertices[7];
         if(max == null)
             max = mesh.vertices[mesh.vertices.Length-1];

         GameObject minPoint = new GameObject();
         minPoint.name = "minPoint";
         minPoint.transform.parent = gameObject.transform;
         minPoint.transform.localPosition = min;

         GameObject maxPoint = new GameObject();
         maxPoint.name = "maxPoint";
         maxPoint.transform.parent = gameObject.transform;
         maxPoint.transform.localPosition = max;

         DestroyImmediate(gameObject.GetComponent<MeshFilter>());
         DestroyImmediate(gameObject.GetComponent<MeshRenderer>());
     }
     catch (System.Exception e)
     {
         Debug.Log("Error creating line points of " + gameObject.name + ": " + e.Message);
     }
 }

 private static void InvertPosition(GameObject gameObject)
 {
     try
     {
         gameObject.transform.position = new Vector3(-gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.transform.position.z);
         gameObject.transform.localScale = new Vector3(-gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.transform.localScale.z);
     }
     catch (System.Exception e)
     {
         Debug.Log("Error inverting position of " + gameObject.name + ": " + e.Message);
     }    
 }

    private static void Rename(GameObject gameObject)
    {
        try
        {
            gameObject.name = gameObject.name.Replace(".r(Clone)", ".l");
            gameObject.name = gameObject.name.Replace("(Clone)", ".l");
        }
        catch (System.Exception e)
        {
            Debug.Log("Error renaming " + gameObject.name + ": " + e.Message);
        }

    }*/

}
