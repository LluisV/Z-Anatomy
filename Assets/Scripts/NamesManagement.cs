using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;

public class NamesManagement : MonoBehaviour
{
    public static NamesManagement instance;
    public TextMeshProUGUI bodyPartDescription;
    public RectTransform descriptionPanelRt;
    public ExpandCollapseUI expandScript;
    public RectTransform descriptionCanvasRT;
    public ScrollRect scrollView;
    public Button previousDescBtn;
    [HideInInspector]
    public bool titleOnScreen = false;

    private List<string> bodypartsHyperlinks;
    private CameraController cam;
    private Stack<GameObject> visitedDesc = new Stack<GameObject>();
    private GameObject prevDescGO;

    private void Awake()
    {
        instance = this;
        cam = Camera.main.GetComponent<CameraController>();
    }

    private void Start()
    {

      //  SetDescriptionPanelSize();
        SetPosToLeft();
        SetCollapsedToLeft();
        expandScript.expandedPosition = new Vector2(0, 0);
        //descriptionPanelRt.gameObject.SetActive(false);
        //------------------------

        /*string text = namesTranslations.text.ToLower();
        titles = text.Split('\n');
        AssignDescriptions(MeshManagement.instance.globalParent.transform);
        descriptionFiles = null;
        titles = null;*/
    }

    public void SetDescriptionPanelSize()
    {
        if(GlobalVariables.instance.mobile)
        {
            descriptionPanelRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, descriptionCanvasRT.sizeDelta.x);

        }
    }

    public void SetPosToLeft() => descriptionPanelRt.localPosition = new Vector2(-descriptionCanvasRT.rect.width, 0);

    public void SetPosToRight() => descriptionPanelRt.localPosition = new Vector2(descriptionCanvasRT.rect.width, 0);
    

    public void SetCollapsedToLeft() => expandScript.collapasedPosition = new Vector2(-descriptionPanelRt.rect.width, 0);


    public void SetCollapsedToRight() => expandScript.collapasedPosition = new Vector2(descriptionPanelRt.rect.width, 0);

    public void GetTranslatedNames()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        bodypartsHyperlinks = allObjects
        .Where(it => !it.CompareTag("Insertions") && it.GetComponent<NameAndDescription>() != null)
        .Select(it => it.name.Replace(" (R)", "").Replace(" (L)", ""))
        .OrderBy(it => it)
        .ToList();
    }


    //---
    /* public List<TextAsset> descriptionFiles = new List<TextAsset>();
     private string[] titles;
      public TextAsset namesTranslations;*/

    //---------------------------------------------------------------------------------------------------------//

    /*  void AssignDescriptions(Transform parent)
      {

          foreach (Transform child in parent)
          {
             //TODO ---------> CLEAR DATA BEFORE RUNTIME!!!!!
             NameAndDescription script = child.GetComponent<NameAndDescription>();
              if (script != null)
              {
                  string name = RemoveSuffix(child.name);
                  string desc = FindDescription(name.Replace("-", " ").ToLower());
                  string cleanName = name.ToLower().Replace(" (", "(").Replace("( ", "(").Replace(" )", ")").Replace(") ", ")").Trim();
                  if (cleanName.Contains("-"))
                  {
                      cleanName = cleanName.Substring(0, cleanName.IndexOf("-"));
                  }
                  script.allNames = FindNames(cleanName);
                  if (desc == null)
                      desc = "\nNo description yet.";
                  else
                      desc = desc.TrimStart();

                  script.description = desc;
              }
              if (child.childCount > 0)
                  AssignDescriptions(child);
          }
      }


        public string[] FindNames(string goName)
    {
        try
        {            
            bool found = false;
            int i = 0;
            //Equals
            while (!found && i < titles.Length)
            {
                string[] splitted = titles[i].Split(';');
                for (int j = 0; j < splitted.Length; j++)
                {
                    found = splitted[j].Equals(goName);
                    if (found)
                        break;
                }
                if (!found)
                    i++;
            }
            //If not found
            if (found)
                return titles[i].Split(';');
            else
            {
                i = 0;
                //Remove ()
                while (!found && i < titles.Length)
                {
                    string a = goName.Substring(0, goName.IndexOf("("));
                    string[] splitted = titles[i].Split(';');
                    for (int j = 0; j < splitted.Length; j++)
                    {
                        found = splitted[j].Equals(a);
                        if (found)
                            break;
                    }
                    if (!found)
                        i++;
                }
            }
            if (found)
                return titles[i].Split(';');

                return null; 
        }
        catch
        {
            return null;
        }

    }

         public string FindDescription(string goName)
         {
             try
             {
                 return descriptionFiles.Find(it => it.name.ToLower().Equals(goName)).text; ;
             }
             catch(Exception)
             {
                 return null;
             }

         }*/


    //------------------------------------------------------------------------------------------------------------------------------//

    private string RemoveSuffix(string str)
    {
        try
        {
            return str.Substring(0, str.IndexOf('.'));
        }
        catch
        {
            return str;
        }
    }

    public void SetTitle(string goName)
    {
        SearchEngine.instance.search = false;
        SearchEngine.instance.mainInputField.text = RemoveSuffix(goName);
        
        //bodyPartName.text = RemoveSuffix(goName);
    }

    public void SetDescription(string description, GameObject go = null)
    {
        bodyPartDescription.text = description;
        scrollView.normalizedPosition = new Vector2(0, 1);

        prevDescGO = go;
    }

    //It hightlights other body parts in the text
    public void HightlightText()
    {
        if (Settings.namesLanguage != Settings.descriptionsLanguage || SelectedObjectsManagement.instance.selectedObjects.Count == 0)
            return;

        previousDescBtn.interactable = visitedDesc.Count > 0;
        bodyPartDescription.ForceMeshUpdate(true);

        string originalDesc = bodyPartDescription.text;
        string actualBodyPart = SearchEngine.instance.mainInputField.text.Replace(" (R)", "").Replace(" (L)", "").ToLower();
        FindAndHighlightLinkMatches(ref originalDesc, actualBodyPart);
        string toLowerDesc = originalDesc.ToLower();
        List<string> matches = bodypartsHyperlinks
            .ConvertAll(d => d.ToLower())
            .Where(it => toLowerDesc.Contains(it.ToLower()) && it.ToLower() != actualBodyPart)
            .Distinct()
            .ToList();

        int indexSpacing = 0;
        //Foreach line in text
        foreach (var lineInfo in bodyPartDescription.textInfo.lineInfo)
        {
            try
            {
                string line = toLowerDesc.Substring(lineInfo.firstCharacterIndex, lineInfo.characterCount);
                List<string> lineMatches = matches.Where(it => line.Contains(it)).ToList();

                //Get bigger match
                foreach (var match in lineMatches.ToList())
                {
                    List<string> duplicates = lineMatches.Where(it => it.Contains(match)).OrderBy(it => it.Length).ToList();

                    //If this match is the smallest one
                    if (duplicates.Count() > 1 && match == duplicates[0])
                    {
                        lineMatches.Remove(match);
                    }
                }

                lineMatches = lineMatches.OrderBy(it => line.IndexOf(it)).ToList();

                int inLineIndex = 0;
                int firstWordIndex = 0;
                int lastWordIndex = 0;

                //Foreach match in this line
                foreach (string match in lineMatches)
                {
                    if (lastWordIndex > line.IndexOf(match))
                        continue;

                    //Get the match index
                    inLineIndex = line.IndexOf(match);
                    if (inLineIndex == 0)
                        firstWordIndex = 0;
                    else
                        firstWordIndex = GetFirstWordIndex(line, inLineIndex) + 1;
                    lastWordIndex = GetLastWordIndex(line, inLineIndex + match.Length);
                    string completeWord = toLowerDesc.Substring(lineInfo.firstCharacterIndex + firstWordIndex, lastWordIndex - firstWordIndex);

                    //Get real index
                    int realIndex = lineInfo.firstCharacterIndex + inLineIndex;

                    //If there's a >2 char difference between matches, don't highlight it
                    if (completeWord.Length - match.Length <= 2)
                    {
                        //Set color
                        originalDesc = originalDesc.Insert(realIndex + indexSpacing, "<link><color=#fc7b03>");
                        indexSpacing += 21;
                        originalDesc = originalDesc.Insert(realIndex + match.Length + indexSpacing, "</link></color>");
                        indexSpacing += 15;
                    }

                    
                }
                
            }
            catch { }
        }

        bodyPartDescription.text = originalDesc;
    }

    private int GetFirstWordIndex(string line, int index)
    {
        char c = line[index];
        bool startOfWord = c == ' ' || c == '.' || c == ',' || c == '\n' || c == '>' || c == ';' || c == '(';

        while (!startOfWord)
        {
            c = line[index];
            startOfWord = c == ' ' || c == '.' || c == ',' || c == '\n' || c == '>' || c == ';' || c == '(';
            if(!startOfWord)
                index--;
        }
        return index;
    }

    private int GetLastWordIndex(string line, int index)
    {
        char c = line[index];
        bool endOfWord = c == ' ' || c == '.' || c == ',' || c == '\n' || c == '<' || c == ';' || c == ')';
        while (!endOfWord)
        {
            c = line[index];
            endOfWord = c == ' ' || c == '.' || c == ',' || c == '\n' || c == '<' || c == ';' || c == ')';
            if(!endOfWord)
                index++;
            if (index == line.Length)
                return line.Length;
        }

        return index;
    }

    private void FindAndHighlightLinkMatches(ref string originalDesc, string actualBodypart)
    {
        bool translateUrl = false;
        int indexSpacing = 0;
        string toLowerDesc = originalDesc.ToLower();

        //Foreach line in text
        foreach (var lineInfo in bodyPartDescription.textInfo.lineInfo)
        {
            try
            {
                string line = toLowerDesc.Substring(lineInfo.firstCharacterIndex, lineInfo.characterCount);
                //If line contains any match
                if (line.Contains("http"))
                {
                    translateUrl = !translateUrl && line.Contains(actualBodypart.Replace(" ", "_"));

                    //Get real index
                    int realIndex = lineInfo.firstCharacterIndex;
                    int lastWordIndex = GetLastWordIndex(originalDesc, realIndex + line.Length - 1 + indexSpacing);

                    //Set color
                    originalDesc = originalDesc.Insert(realIndex + indexSpacing, "<link><color=#fc7b03>");
                    indexSpacing += 21;

                    if (lastWordIndex == originalDesc.Length)
                        originalDesc += "</link></color>";
                    else
                    {
                        originalDesc = originalDesc.Insert(lastWordIndex + indexSpacing, "</link></color>");
                        indexSpacing += 15;
                    }
                }
            }
            catch{}
        }

        if(translateUrl)
        {

            if (Settings.descriptionsLanguage == SystemLanguage.Spanish)
                originalDesc = originalDesc.Replace("//en", "//es");
            else if (Settings.descriptionsLanguage == SystemLanguage.French)
                originalDesc = originalDesc.Replace("//en", "//fr");
            else if (Settings.descriptionsLanguage == SystemLanguage.Portuguese)
                originalDesc = originalDesc.Replace("//en", "//pt");
        }
    }

    public void TextClicked(string clickedObject, bool updateStack)
    {
        if (Settings.namesLanguage != Settings.descriptionsLanguage)
            return;

        List<string> toLowerBodyParts = bodypartsHyperlinks.Select(it => it.ToLower()).ToList();
        
        //If it is a link
        if(clickedObject.Contains("http"))
        {
            Application.OpenURL(clickedObject);
        }
        else
        {
            int index = toLowerBodyParts.IndexOf(clickedObject.Replace(" (R)", ""). Replace(" (L)","").ToLower());
            string name = bodypartsHyperlinks[index].Trim();

            Transform clickedGO = StaticMethods.RecursiveFindChild(MeshManagement.instance.globalParent.transform, name + " (R)");       
            if (clickedGO == null)
                 clickedGO = StaticMethods.RecursiveFindChild(MeshManagement.instance.globalParent.transform, name);
            if (clickedGO == null)
                return;

            if(updateStack)
            {
                visitedDesc.Push(prevDescGO);
                prevDescGO = clickedGO.gameObject;
            }

            previousDescBtn.interactable = visitedDesc.Count > 0;

            StaticMethods.SetActiveParentsRecursively(clickedGO.transform, true);

            SelectedObjectsManagement.instance.DeselectAllObjects();
            ActionControl.someObjectSelected = true;

            BodyPart bodyPartScript = clickedGO.GetComponent<BodyPart>();
            Label labelSript = clickedGO.GetComponent<Label>();

            //If it is a bodypart
            if(bodyPartScript != null)
            {
                //If it is not on scene
                if (!clickedGO.gameObject.activeInHierarchy)
                {
                    //Set active
                    StaticMethods.SetActiveParentsRecursively(bodyPartScript.transform, true);
                }

                //Select it
                SelectedObjectsManagement.instance.SelectObject(clickedGO.gameObject);
                SelectedObjectsManagement.instance.EnableDisableButtons();

                //Focus camera
                cam.target = clickedGO.gameObject;
                cam.cameraCenter = bodyPartScript.center;
                cam.UpdateDistance(bodyPartScript.distanceToCamera);
            }
            //If it is a label
            else if(labelSript != null)
            {
                //Select the label's parent (jump the .labels obj)
                SelectedObjectsManagement.instance.SelectObject(clickedGO.gameObject.transform.parent.parent.gameObject);
                SelectedObjectsManagement.instance.EnableDisableButtons();

                //Isolate it
                MeshManagement.instance.IsolationClick();

                //Get the parent script
                BodyPart parentScript = clickedGO.parent.parent.GetComponent<BodyPart>();

                //Focus camera
                cam.target = clickedGO.parent.parent.gameObject;
                cam.cameraCenter = parentScript.center;
                cam.UpdateDistance(parentScript.distanceToCamera);

                //Then select the label
                SelectedObjectsManagement.instance.SelectObject(clickedGO.gameObject);
            }
            //If it is a global part
            else
            {
                SelectedObjectsManagement.instance.activeObjects.Clear();
                SelectedObjectsManagement.instance.SelectAllChildren(clickedGO.transform);
                SelectedObjectsManagement.instance.EnableDisableButtons();

                cam.CenterView(true);
            }

            NameAndDescription nameScript = clickedGO.GetComponent<NameAndDescription>();
            SetTitle(nameScript.name);
            SetDescription(nameScript.Description(), clickedGO.gameObject);
            HightlightText();
        }
    }

    public void GetPreviousDescriptionBtn()
    {
        GameObject prev = visitedDesc.Pop();
        NameAndDescription nameScript = prev.GetComponent<NameAndDescription>();
        SetDescription(nameScript.Description(), prev);
        TextClicked(nameScript.name, false);
    }

    public void ResetStack()
    {
        visitedDesc.Clear();
        prevDescGO = null;
    }

}
