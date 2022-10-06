using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System;
using System.IO;
using System.Text;
using Assets.Scripts.Commands;

public class NamesManagement : MonoBehaviour
{
    public static NamesManagement Instance;
    [HideInInspector]
    public TextMeshProUGUI bodyPartDescription;
    public TMP_InputField bodyPartInputField;
    public ExpandCollapseUI expandScript;
    public RectTransform descriptionCanvasRT;
    public ScrollRect scrollView;

    private CameraController cam;
    public GameObject emptyDescPanel;
    public GameObject emptySelecPanel;
    public GameObject emptyInternetPanel;
    public GameObject warningMessage;

    public const string NO_SELECTION = "_NO_SELECTION_";

    public TextAsset translations;
    [HideInInspector]
    public Dictionary<string,string[]> splittedTranslations = new Dictionary<string, string[]>();

    private string selectedText;

    private void Awake()
    {
        Instance = this;
        cam = Camera.main.GetComponent<CameraController>();
        bodyPartDescription = (TextMeshProUGUI)bodyPartInputField.textComponent;
    }

    private void Start()
    {
        emptySelecPanel.SetActive(true);
        emptyDescPanel.SetActive(false);
    }

    public void GetNamesTranslations()
    {
        try
        {
            int notFoundCount = 0;
            List<string> notFound = new List<string>();

            //Split the text in lines
            string[] lines = translations.text.Replace("\t", "").Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            //Split the text in words
            for (int i = 0; i < lines.Length; i++)
            {
                var words = lines[i].Split(';');
                if (words.Length == 0)
                    continue;
                splittedTranslations[words[0].ToLower()] = words;
            }

            //Foreach object in scene
            foreach (var nameScript in GlobalVariables.Instance.allNameScripts)
            {
                //Remove suffix from its name
                var name = nameScript.name.RemoveSuffix();
                //If translation doc contains it, assign the languages array
                if (splittedTranslations.ContainsKey(name.ToLower()))
                {
                    var columns = splittedTranslations[name.ToLower()];
                    var length = columns.Length;
                    var nameTranslations = new string[length / 2];
                    var nameSynonyms = new List<string[]>();

                    //Set names
                    for (int i = 0, j = 0; i < length; i += 2, j++)
                        nameTranslations[j] = columns[i];

                    //Set synonims
                    for (int i = 1, j = 0; i < length; i += 2, j++)
                    {
                        if (columns[i].Length == 0)
                        {
                            nameSynonyms.Add(null);
                            continue;
                        }
                        var synonims = columns[i].Split('%');
                        if (synonims != null && synonims.Length > 0)
                            nameSynonyms.Add(synonims);
                        else
                            nameSynonyms.Add(null);
                    }

                    nameScript.allNames = nameTranslations;
                    nameScript.allSynonyms = nameSynonyms;
                }
                //Else, show its name in console
                else if (!notFound.Contains(name))
                {
                    notFoundCount++;
                    notFound.Add(name);
                }
            }


            Settings.Instance.SetLanguage();

            UnityEngine.Debug.Log("Not found count: " + notFoundCount);
            SaveNotFound(notFound.ToArray());

            SetLeftRightName();

            foreach (var item in GlobalVariables.Instance.allNameScripts)
            {
                item.SetTranslatedName();
            }
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogError(e);
        }
     

    }

    private void SetLeftRightName()
    {
        foreach (var item in GlobalVariables.Instance.allNameScripts)
        {
            if (item.originalName.Contains(".r"))
                item.SetRightLeftName(true);
            else if (item.originalName.Contains(".l"))
                item.SetRightLeftName(false);
        }
    }

    public string[] GetWordTranslations(string originalName)
    {
        if (splittedTranslations.ContainsKey(originalName.RemoveSuffix().ToLower()))
        {
            var columns = splittedTranslations[originalName.RemoveSuffix().ToLower()];
            var length = columns.Length;
            var nameTranslations = new string[length / 2];

            //Set names
            for (int i = 0, j = 0; i < length; i += 2, j++)
                nameTranslations[j] = columns[i];

            return nameTranslations;
        }
        else
            return null;
    }

    private void SaveNotFound(string[] content)
    {
        string localUrl = Application.persistentDataPath + "/Not found.txt";
        File.WriteAllLines(localUrl, content);
    }


    public bool SomeTextSelected()
    {
        return bodyPartInputField.selectionAnchorPosition != bodyPartInputField.selectionFocusPosition;
    }


    public void SetDesc(string description, bool isChecked = true, string name = null)
    {
        scrollView.enabled = true;

        warningMessage.SetActive(!isChecked);

        if (isChecked)
            bodyPartInputField.textComponent.margin = new Vector4(10, 15, 15, 10);
        else
            bodyPartInputField.textComponent.margin = new Vector4(10, 30, 15, 10);

        emptyInternetPanel.SetActive(false);
        emptyDescPanel.SetActive(description == null);
        if (description == null || description == NO_SELECTION)
            bodyPartInputField.text = "";
        emptySelecPanel.SetActive(description == NO_SELECTION);

        if (description != null && description != NO_SELECTION)
        {
            emptySelecPanel.SetActive(false);

            RebuildDescriptionPanel(description: description, name: name);

            StartCoroutine(Scroll());

            IEnumerator Scroll()
            {
                yield return new WaitForEndOfFrame();

                scrollView.normalizedPosition = new Vector2(0, 1);

            }
        }
    }

    public void RebuildDescriptionPanel(bool waitExpand = false, float time = 0.2f, string name = "", string description = "")
    {
        StartCoroutine(Rebuild());

        IEnumerator Rebuild()
        {
          /*  Stopwatch timeElapsed = new Stopwatch();
            timeElapsed.Start();*/

            yield return StartCoroutine(HighlightText.Hightlight(description, name.Replace("(R)", "").Replace("(L)", "").Replace(".t", "").Replace(".s","").Trim().ToLower(), bodyPartInputField));

            var textrt = bodyPartInputField.textComponent.GetComponent<RectTransform>();
            var contentrt = textrt.parent.GetComponent<RectTransform>();
            var containerrt = contentrt.parent.GetComponent<RectTransform>();
            var caret = bodyPartInputField.GetComponentInChildren<TMP_SelectionCaret>();

            float textheigth = textrt.GetHeight();
            var containerheight = containerrt.GetHeight();
            contentrt.SetHeight(textheigth);
            textrt.SetTop(0);
            textrt.SetBottom(0);

            if ((containerheight - textheigth) > 0)
            {
                contentrt.SetTop(0);
                contentrt.SetBottom(0);
                textrt.anchoredPosition = new Vector3(0, 0, 0);
            }

            contentrt.anchoredPosition = new Vector3(0, -textheigth / 2, 0);
            caret.Rebuild(CanvasUpdate.MaxUpdateValue);

            /*timeElapsed.Stop();
            UnityEngine.Debug.Log("Total: " + (float)(timeElapsed.ElapsedMilliseconds / 1000f));*/
        }

    }

    public void TextClicked(string clickedObject, bool leftClick)
    {
        //If it is a link
        if(clickedObject.Contains("http"))
        {
            Application.OpenURL(clickedObject);
        }
        else
        {
            Transform clickedGO = GlobalVariables.Instance.globalParent.transform.RecursiveFindChild(new StringBuilder().Append(clickedObject.ToLower()).Append(" (R)").ToString());       
            if (clickedGO == null)
                 clickedGO = GlobalVariables.Instance.globalParent.transform.RecursiveFindChild(clickedObject.ToLower());
            if(clickedGO == null)
            {
                var go = GlobalVariables.Instance.allNameScripts.Find(it => it.HasSynonims() && it.allSynonyms[Settings.languageIndex].Any(it2 => it2.ToLower().Equals(clickedObject.ToLower())));
                if(go != null)
                    clickedGO = go.transform;
            }
            if (clickedGO == null)
                return;

            //If it was right click -> show contextual menu
            if(!leftClick)
            {
                ContextualMenu.Instance.contextObject = clickedGO.gameObject;
                ContextualMenu.Instance.Show();
                return;
            }

            List<GameObject> shown = new List<GameObject>();
            clickedGO.transform.SetActiveParentsRecursively(true, shown);

            SelectedObjectsManagement.Instance.DeselectAllObjects();

            BodyPart bodyPartScript = clickedGO.GetComponent<BodyPart>();
            Label labelSript = clickedGO.GetComponent<Label>();

            //If it is a bodypart
            if (bodyPartScript != null)
            {
                //Select it
                SelectedObjectsManagement.Instance.SelectObject(clickedGO.gameObject);
                ActionControl.Instance.AddCommand(new SelectCommand(SelectedObjectsManagement.Instance.selectedObjects), false);
                ActionControl.Instance.UpdateButtons();

                //Focus camera
                cam.SetTarget(clickedGO.gameObject);
                cam.cameraCenter.position = bodyPartScript.center;
                cam.UpdateCameraPos(bodyPartScript.distanceToCamera);
            }
            //If it is a label
            else if(labelSript != null)
            {
                //Select the label's parent (jump the .labels obj)
                SelectedObjectsManagement.Instance.SelectObject(labelSript.parent.gameObject);
                ActionControl.Instance.AddCommand(new SelectCommand(SelectedObjectsManagement.Instance.selectedObjects), false);
                ActionControl.Instance.UpdateButtons();

                //Isolate it
                MeshManagement.Instance.IsolationClick();

                //Focus camera
                cam.SetTarget(labelSript.parent.gameObject);
                cam.cameraCenter.position = labelSript.parent.center;
                cam.UpdateCameraPos(labelSript.parent.distanceToCamera);

                //Then select the label
                SelectedObjectsManagement.Instance.SelectObject(clickedGO.gameObject);
                ActionControl.Instance.AddCommand(new SelectCommand(SelectedObjectsManagement.Instance.selectedObjects), false);
            }
            //If it is a global part
            else
            {
                SelectedObjectsManagement.Instance.activeObjects.Clear();
                SelectedObjectsManagement.Instance.SelectAllChildren(clickedGO.transform, true, shown);
                ActionControl.Instance.UpdateButtons();

                if(ActionControl.zoomSelected)
                    cam.CenterView(true);
            }

            NameAndDescription nameScript = clickedGO.GetComponent<NameAndDescription>();
            //Set the hierarchy bar
            HierarchyBar.Instance.Set(clickedGO.transform);
            //Expand in lexicon
            Lexicon.Instance.ExpandRecursively();
            ActionControl.Instance.AddCommand(new ShowCommand(shown), false);
            Lexicon.Instance.UpdateTreeViewCheckboxes();
            nameScript.SetDescription();
        }
    }


    public void NoConnectionScreen()
    {
        emptyInternetPanel.SetActive(true);
        emptyDescPanel.SetActive(false);
        emptySelecPanel.SetActive(false);
    }
}
