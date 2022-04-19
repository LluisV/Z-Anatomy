using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.Text;
using System.Globalization;
using System;

public class SearchEngine : MonoBehaviour
{
    public static SearchEngine instance;
    public TMP_InputField mainInputField;
    private MeshManagement meshManagement;
    private SelectedObjectsManagement selectionManagement;
    public CameraController cam;
    public NamesManagement descriptions;

    private List<GameObject> gameObjectFound = new List<GameObject>();
    private int lastLenght = 0;
    public GameObject uiFoundObjectPrefab;
    public GameObject uiFoundObjectPrefabParent;
    [HideInInspector]
    public bool search = true;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainInputField.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        meshManagement = GetComponent<MeshManagement>();
        selectionManagement = GetComponent<SelectedObjectsManagement>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            //uiFoundObjectPrefabParent.SetActive(false);
        }
    }

    public void ValueChangeCheck()
    {
        if(!search)
        {
            search = true;
        }
        else
        {
            gameObjectFound.Clear();
            DestroyChildren(uiFoundObjectPrefabParent.transform);
            StopAllCoroutines();
            if (mainInputField.text.Length >= 3 && mainInputField.text.Length > lastLenght)
            {
                StartCoroutine(GetResults());
            }
            lastLenght = mainInputField.text.Length;
            uiFoundObjectPrefabParent.SetActive(true);
        }
    }

    IEnumerator GetResults()
    {
       // yield return StartCoroutine(RecursiveFindChilds(meshManagement.globalParent.transform, mainInputField.text));
        yield return StartCoroutine(RecursiveStartWithChild(meshManagement.globalParent.transform, mainInputField.text));
        yield return StartCoroutine(RecursiveContainsChild(meshManagement.globalParent.transform, mainInputField.text));
        gameObjectFound = gameObjectFound.OrderByDescending(it => it.activeInHierarchy).ToList();
        foreach (var item in gameObjectFound)
        {
            GameObject go = Instantiate(uiFoundObjectPrefab, uiFoundObjectPrefabParent.transform);
            OnRecommendationClick script = go.GetComponentInChildren<OnRecommendationClick>();
            TextMeshProUGUI tmpro = go.GetComponentInChildren<TextMeshProUGUI>();

            if (!item.activeInHierarchy)
                tmpro.color = Color.gray;
            //Clean text
            script.realName = item.name;
            tmpro.text = item.name.Replace(".g", "").Replace(".t", "").Replace(".r", " R").Replace(".l", " L");

            //Highlight text
            int indexOfSearch = tmpro.text.ToLower().IndexOf(mainInputField.text.ToLower());
            if(indexOfSearch == -1)
                indexOfSearch = StaticMethods.RemoveAccents(tmpro.text).ToLower().IndexOf(mainInputField.text.ToLower());
            string newText = tmpro.text;
            if (indexOfSearch != -1)
                newText = string.Concat(tmpro.text.Substring(0, indexOfSearch), "<color=#fc7b03>" + tmpro.text.Substring(indexOfSearch, mainInputField.text.Length), "</color>" + tmpro.text.Substring(indexOfSearch + mainInputField.text.Length));
            tmpro.text = newText;
            yield return null;
        }
    }

    private void DestroyChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    IEnumerator RecursiveFindChilds(Transform parent, string childName)
    {
        childName = childName.ToLower();
        foreach (Transform child in parent)
        {
            string childN = StaticMethods.RemoveAccents(child.name.ToLower());
        //    if (child.gameObject.activeSelf)
            {
                if (childN == StaticMethods.RemoveAccents(childName) && !childN.Contains("-line") && !childN.Contains("label"))
                {
                    if(!gameObjectFound.Contains(child.gameObject))
                        gameObjectFound.Add(child.gameObject);
                    //return child;
                }
                if(child.childCount > 0)
                {
                    StartCoroutine(RecursiveFindChilds(child, childName));
                }
            }

        }
        yield return null;
    }

    IEnumerator RecursiveStartWithChild(Transform parent, string childName)
    {
        childName = childName.ToLower();
        foreach (Transform child in parent)
        {
            string childN = StaticMethods.RemoveAccents(child.name.ToLower());
          //  if (child.gameObject.activeInHierarchy )
            {
                if(childN.StartsWith(StaticMethods.RemoveAccents(childName)) && !child.CompareTag("Insertions") && !childN.Contains("-lin") && !childN.Contains("label"))
                {
                    if (!gameObjectFound.Contains(child.gameObject))
                        gameObjectFound.Add(child.gameObject);
                }
                if(child.childCount > 0)
                {
                    StartCoroutine(RecursiveStartWithChild(child, childName));
                }
            }
        }
        yield return null;
    }

    IEnumerator RecursiveContainsChild(Transform parent, string childName)
    {
        childName =childName.ToLower();
        foreach (Transform child in parent)
        {
            string childN = StaticMethods.RemoveAccents(child.name.ToLower());
          //  if (child.gameObject.activeInHierarchy)
            {
                if (childN.Contains(StaticMethods.RemoveAccents(childName)) && !child.CompareTag("Insertions") && !childN.Contains("-lin") && !childN.Contains("label"))
                {
                    if (!gameObjectFound.Contains(child.gameObject))
                        gameObjectFound.Add(child.gameObject);
                    //return child;
                }
                if(child.childCount > 0)
                {
                    StartCoroutine(RecursiveContainsChild(child, childName));
                }
            }
        }
        yield return null;
    }

    public void RecommendationClicked(GameObject pressedBtn)
    {
        OnRecommendationClick script = pressedBtn.GetComponentInChildren<OnRecommendationClick>();
        Transform found = StaticMethods.RecursiveFindChild(meshManagement.globalParent.transform, script.realName);
        GameObject goFound = found.gameObject;
        StaticMethods.SetActiveParentsRecursively(goFound.transform, true);

        //Hide searchbar found objects panel
        uiFoundObjectPrefabParent.SetActive(false);
        uiFoundObjectPrefabParent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

        selectionManagement.DeselectAllObjects();
        ActionControl.someObjectSelected = true;

        BodyPart bodyPartScript = goFound.GetComponent<BodyPart>();
        Label labelSript = goFound.GetComponent<Label>();

        //If it is a bodypart
        if (bodyPartScript != null)
        {
            //If it is not on scene
            if(!goFound.activeInHierarchy)
            {
                //Set active
                StaticMethods.SetActiveParentsRecursively(bodyPartScript.transform, true);
            }

            //Select it
            selectionManagement.SelectObject(goFound);
            selectionManagement.EnableDisableButtons();

            //Focus camera
            cam.target = found.gameObject;
            cam.cameraCenter = bodyPartScript.center;
            cam.UpdateDistance(bodyPartScript.distanceToCamera);

        }
        //If it is a label
        else if(labelSript != null)
        {
            //Select the label's parent (jump the .labels obj)
            selectionManagement.SelectObject(goFound.transform.parent.parent.gameObject);
            selectionManagement.EnableDisableButtons();

            //Isolate it
            meshManagement.IsolationClick();

            //Get the parent script
            BodyPart parentScript = goFound.transform.parent.parent.GetComponent<BodyPart>();

            //Focus camera
            cam.target = goFound.transform.parent.parent.gameObject;
            cam.cameraCenter = parentScript.center;
            cam.UpdateDistance(parentScript.distanceToCamera);

            //Then select the label
            selectionManagement.SelectObject(goFound);
        }
        //If it is a global part
        else
        {
            selectionManagement.activeObjects.Clear();
            selectionManagement.SelectAllChildren(goFound.transform);
            selectionManagement.EnableDisableButtons();

            cam.CenterView(true);
        }


        descriptions.SetTitle(goFound.name);

        //Reset visited objects stack
        NamesManagement.instance.ResetStack();

        NameAndDescription descriptionScript = goFound.GetComponent<NameAndDescription>();
        if (descriptionScript != null)
            descriptions.SetDescription(descriptionScript.Description());
    }

}
