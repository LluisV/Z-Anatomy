using Assets.Scripts.Commands;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ContextualMenu : MonoBehaviour
{
    public static ContextualMenu Instance;

    private RectTransform rt;

    public Button copyBtn;
    public Button isolateBtn;
    public Button partiallyIsolateBtn;
    public Button hideBtn;
    public Button resetPosition;
    public Button movePosition;
    public Button showKeyColors;
    public Button hideKeyColors;
    public Button addNoteBtn;
    public Button structuresBtn;
    public Button showLabelsBtn;
    public Button hideLabelsBtn;
    public Button showBoneInsertionsBtn;
    public Button hideBoneInsertionsBtn;
    public Button showMuscleNervesBtn;
    public Button hideMuscleNervesBtn;
    public Button showNerveMusclesBtn;
    public Button hideNerveMusclesBtn;

    public Button showMusclesBtn;


    public RectTransform bonusCollectionPanel;

    private bool someCollectionFound;

    [HideInInspector]
    public GameObject contextObject;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
        rt = GetComponent<RectTransform>();
        addNoteBtn.transform.parent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
            Close();
    }

    public void UpdateEnabledButtons()
    {
        foreach (Transform item in bonusCollectionPanel.transform)
            Destroy(item.gameObject);

        SelectedObjectsManagement.Instance.GetActiveObjects();
        List<string> actualTags = new List<string>();
        foreach (var item in SelectedObjectsManagement.Instance.activeObjects)
        {
            if (!actualTags.Contains(item.tag))
            {
                actualTags.Add(item.tag);
            }
        }

        copyBtn.transform.parent.gameObject.SetActive(false);

        isolateBtn.transform.parent.gameObject.SetActive(true);
        hideBtn.transform.parent.gameObject.SetActive(true);

        structuresBtn.transform.parent.gameObject.SetActive(false);
        structuresBtn.transform.parent.gameObject.SetActive(ShowStructuresOn());

        partiallyIsolateBtn.transform.parent.gameObject.SetActive(actualTags.Count > 1);

        movePosition.transform.parent.gameObject.SetActive(MovePositionOn());
        resetPosition.transform.parent.gameObject.SetActive(ResetPositionOn());

        showLabelsBtn.transform.parent.gameObject.SetActive(ShowLabelsOn());
        hideLabelsBtn.transform.parent.gameObject.SetActive(HideLabelsOn());

        showBoneInsertionsBtn.transform.parent.gameObject.SetActive(ShowInsertionsOn());
        hideBoneInsertionsBtn.transform.parent.gameObject.SetActive(HideInsertionsOn());

        showMusclesBtn.transform.parent.gameObject.SetActive(ShowInsertionMuscleOn());

        showNerveMusclesBtn.transform.parent.gameObject.SetActive(ShowNerveMusclesOn());
        showMuscleNervesBtn.transform.parent.gameObject.SetActive(ShowMuscleNervesOn());

        showKeyColors.transform.parent.gameObject.SetActive(ShowKeyColorsOn());
        hideKeyColors.transform.parent.gameObject.SetActive(HideKeyColorsOn());
    }

    public void ShowCopyBtn()
    {
        copyBtn.transform.parent.gameObject.SetActive(true);

        isolateBtn.transform.parent.gameObject.SetActive(false);
        hideBtn.transform.parent.gameObject.SetActive(false);
        structuresBtn.transform.parent.gameObject.SetActive(false);
        structuresBtn.transform.parent.gameObject.SetActive(false);
        partiallyIsolateBtn.transform.parent.gameObject.SetActive(false);
        movePosition.transform.parent.gameObject.SetActive(false);
        resetPosition.transform.parent.gameObject.SetActive(false);
        showLabelsBtn.transform.parent.gameObject.SetActive(false);
        hideLabelsBtn.transform.parent.gameObject.SetActive(false);
        showBoneInsertionsBtn.transform.parent.gameObject.SetActive(false);
        hideBoneInsertionsBtn.transform.parent.gameObject.SetActive(false);
        showMusclesBtn.transform.parent.gameObject.SetActive(false);
        showNerveMusclesBtn.transform.parent.gameObject.SetActive(false);
        showMuscleNervesBtn.transform.parent.gameObject.SetActive(false);
        showKeyColors.transform.parent.gameObject.SetActive(false);
        hideKeyColors.transform.parent.gameObject.SetActive(false);
    }

    public void CopyClick()
    {
        GUIUtility.systemCopyBuffer = DescriptionClick.Instance.selectedText.RemoveRichTextTags();
        PopUpManagement.Instance.Show("Text copied!");
    }

    public void IsolateClick()
    {
        bool wasSelected = contextObject != null && SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject);

        if (!wasSelected && contextObject != null)
        {
            SelectedObjectsManagement.Instance.DeselectAllObjects();
            SelectedObjectsManagement.Instance.SelectObject(contextObject);
            if (contextObject.IsGroup())
                SelectedObjectsManagement.Instance.SelectAllChildren(contextObject.transform);
        }

        MeshManagement.Instance.IsolationClick();

         if(!wasSelected && contextObject != null && SelectedObjectsManagement.Instance.selectedObjects.Count > 1)
         {
             SelectedObjectsManagement.Instance.DeselectObject(contextObject);
             if (contextObject.IsGroup())
                 SelectedObjectsManagement.Instance.DeselectAllChildren(contextObject.transform);
         }

        ActionControl.Instance.UpdateButtons();
    }

    public void PartiallyIsolateClick()
    {
        bool wasSelected = contextObject != null && SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject);

        if (!wasSelected && contextObject != null)
        {
            SelectedObjectsManagement.Instance.DeselectAllObjects();
            SelectedObjectsManagement.Instance.SelectObject(contextObject);
            if (contextObject.IsGroup())
                SelectedObjectsManagement.Instance.SelectAllChildren(contextObject.transform);
        }
        MeshManagement.Instance.PartialIsolationClick();

         if (!wasSelected && contextObject != null)
         {
             SelectedObjectsManagement.Instance.DeselectObject(contextObject);
             if (contextObject.IsGroup())
                 SelectedObjectsManagement.Instance.DeselectAllChildren(contextObject.transform);
         }
        ActionControl.Instance.UpdateButtons();

    }

    public void MovePositionClick()
    {
        bool wasSelected = contextObject != null && SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject);

        if (!wasSelected && contextObject != null)
        {
            SelectedObjectsManagement.Instance.DeselectAllObjects();
            SelectedObjectsManagement.Instance.SelectObject(contextObject);
            if (contextObject.IsGroup())
                SelectedObjectsManagement.Instance.SelectAllChildren(contextObject.transform);
        }

        TranslateObject.Instance.UpdateSelected();
        TranslateObject.Instance.Enable();
    }

    public void ResetPositionClick()
    {
        bool wasSelected = contextObject != null && SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject);

        if (!wasSelected && contextObject != null)
        {
            SelectedObjectsManagement.Instance.DeselectAllObjects();
            SelectedObjectsManagement.Instance.SelectObject(contextObject);
            if (contextObject.IsGroup())
                SelectedObjectsManagement.Instance.SelectAllChildren(contextObject.transform);
        }

        foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
        {
            TangibleBodyPart script = item.GetComponent<TangibleBodyPart>();
            if (script != null)
                script.ResetPosition();
        }

        if (!wasSelected && contextObject != null)
        {
            SelectedObjectsManagement.Instance.DeselectObject(contextObject);
            if (contextObject.IsGroup())
                SelectedObjectsManagement.Instance.DeselectAllChildren(contextObject.transform);
        }

        TranslateObject.Instance.SetGizmoCenter();
    }

    public void HideClick()
    {
        bool wasSelected = contextObject != null && SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject);

        if (!wasSelected && contextObject != null)
        {
            SelectedObjectsManagement.Instance.DeselectAllObjects();
            SelectedObjectsManagement.Instance.SelectObject(contextObject);
            if (contextObject.IsGroup())
                SelectedObjectsManagement.Instance.SelectAllChildren(contextObject.transform);
        }

        SelectedObjectsManagement.Instance.DeleteSelected();

        if (!wasSelected && contextObject != null)
        {
            SelectedObjectsManagement.Instance.DeselectObject(contextObject);
            if (contextObject.IsGroup())
                SelectedObjectsManagement.Instance.DeselectAllChildren(contextObject.transform);
        }
        ActionControl.Instance.UpdateButtons();

    }

    public void ShowLabelsClick()
    {
        if (contextObject != null && !SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
        {
            contextObject.GetComponent<BodyPartVisibility>().ShowLabels(true);

            HierarchyBar.Instance.Set(contextObject.transform);
            Lexicon.Instance.ExpandRecursively();

            contextObject.gameObject.SetActive(true);
        }

        if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
        {
            foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
            {
                item.GetComponent<BodyPartVisibility>().ShowLabels(true);
            }
        }
        Lexicon.Instance.UpdateTreeViewCheckboxes();

    }

    public void HideLabelsClick()
    {
        if (contextObject != null && !SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            contextObject.GetComponent<BodyPartVisibility>().HideLabels(true);

        if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
        {
            foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
            {
                item.GetComponent<BodyPartVisibility>().HideLabels(true);
            }
        }
        Lexicon.Instance.UpdateTreeViewCheckboxes();

    }

    public void ShowInsertionsClick()
    { 
        List<GameObject> shown = new List<GameObject>();

        if(contextObject != null && !SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            shown = contextObject.GetComponent<BodyPartVisibility>().ShowInsertions();

        if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
        {
            foreach (var item in SelectedObjectsManagement.Instance.selectedObjects.ToList())
            {
                shown.AddRange(item.GetComponent<BodyPartVisibility>().ShowInsertions());
            }
        }
        Lexicon.Instance.UpdateTreeViewCheckboxes();

        if (shown.Count > 0)
            ActionControl.Instance.AddCommand(new ShowCommand(shown), false);
    }

    public void HideInsertionsClick()
    {
        List<GameObject> hidden = new List<GameObject>(); ;

        if (contextObject != null && !SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            hidden = contextObject.GetComponent<BodyPartVisibility>().HideInsertions();

        if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
        {
            foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
            {
                hidden.AddRange(item.GetComponent<BodyPartVisibility>().HideInsertions());
            }
        }
        Lexicon.Instance.UpdateTreeViewCheckboxes();

        if (hidden.Count > 0)
            ActionControl.Instance.AddCommand(new DeleteCommand(hidden), false);
    }

    public void AddNoteClick()
    {
        
    }

    public void ShowMuscleClick()
    {
        List<GameObject> shown = new List<GameObject>(); ;
        if (contextObject != null && !SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            shown = contextObject.GetComponent<BodyPartVisibility>().ShowInsertionMuscles();

        if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
        {
            foreach (var item in SelectedObjectsManagement.Instance.selectedObjects.ToList())
            {
                shown.AddRange(item.GetComponent<BodyPartVisibility>().ShowInsertionMuscles());
            }
        }

        Lexicon.Instance.UpdateTreeViewCheckboxes();

        if (shown.Count > 0)
            ActionControl.Instance.AddCommand(new ShowCommand(shown), false);
    }

    public void ShowMuscleNervesClick()
    {

        List<GameObject> shown = new List<GameObject>(); ;

        if (contextObject != null && !SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            shown = contextObject.GetComponent<BodyPartVisibility>().ShowMuscleNerves();

        if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
        {
            foreach (var item in SelectedObjectsManagement.Instance.selectedObjects.ToList())
            {
                shown.AddRange(item.GetComponent<BodyPartVisibility>().ShowMuscleNerves());
            }
        }
        Lexicon.Instance.UpdateTreeViewCheckboxes();

        if (shown.Count > 0)
            ActionControl.Instance.AddCommand(new ShowCommand(shown), false);

    }

    public void ShowNerveMusclesClick()
    {
        List<GameObject> shown = new List<GameObject>(); ;

        if (contextObject != null && !SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            shown = contextObject.GetComponent<BodyPartVisibility>().ShowNerveMuscles();

        if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
        {
            foreach (var item in SelectedObjectsManagement.Instance.selectedObjects.ToList())
            {
                shown.AddRange(item.GetComponent<BodyPartVisibility>().ShowNerveMuscles());
            }
        }
        Lexicon.Instance.UpdateTreeViewCheckboxes();

        if (shown.Count > 0)
            ActionControl.Instance.AddCommand(new ShowCommand(shown), false);

    }

    public void ShowKeyColorsClick()
    {
        if (contextObject != null && !SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            KeyColors.Instance.EnableByTag(contextObject.tag);

        if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
        {
            KeyColors.Instance.EnableByTag(SelectedObjectsManagement.Instance.selectedObjects[0].tag);
        }
    }

    public void HideKeyColorsClick()
    {
        if (contextObject != null && !SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            KeyColors.Instance.EnableByTag(contextObject.tag);

        if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
        {
            KeyColors.Instance.EnableByTag(SelectedObjectsManagement.Instance.selectedObjects[0].tag);
        }
    }

    public void Show()
    {
        if((contextObject != null && (contextObject.IsBodyPart() || contextObject.IsGroup())) || SelectedObjectsManagement.Instance.selectedObjects.Count != 0)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            gameObject.SetActive(true);
            transform.position = new Vector2(
                Mathf.Clamp(mousePos.x + rt.GetWidth() / 2, rt.GetWidth() * 0.75f, Screen.width - rt.GetWidth() * 0.75f), 
                Mathf.Clamp(mousePos.y - rt.GetHeight() / 2, rt.GetHeight() * 0.75f, Screen.height - rt.GetHeight() * 0.75f)
                );
            bonusCollectionPanel.position = new Vector2(
                bonusCollectionPanel.position.x,
                Mathf.Clamp(bonusCollectionPanel.parent.position.y + 25, bonusCollectionPanel.GetHeight(), Screen.height)
                );

            Lexicon.Instance.UpdateTreeViewCheckboxes();
            UpdateEnabledButtons();
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private bool ShowStructuresOn()
    {

        if (SelectedObjectsManagement.Instance.selectedObjects.Count > 1)
            return false;

        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 0)
        {
            NameAndDescription objName = contextObject.GetComponent<NameAndDescription>();
            someCollectionFound = BonusCollections.Instance.SetCollections(objName.originalName, objName.transform);
            return someCollectionFound;
        }

        else
        {
            if (contextObject != null && SelectedObjectsManagement.Instance.selectedObjects[0] != contextObject)
            {
                NameAndDescription objName = contextObject.GetComponent<NameAndDescription>();
                someCollectionFound = BonusCollections.Instance.SetCollections(objName.originalName, objName.transform);
                return someCollectionFound;
            }
            else
            {
                NameAndDescription objName = SelectedObjectsManagement.Instance.selectedObjects[0].GetComponent<NameAndDescription>();
                someCollectionFound = BonusCollections.Instance.SetCollections(objName.originalName, objName.transform);
                return someCollectionFound;
            }
        }
    }

    private bool ShowLabelsOn()
    {
        if (SelectedObjectsManagement.Instance.selectedObjects.Count > 1)
            return false;

        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 0)
        {
            var visibility = contextObject.GetComponent<BodyPartVisibility>();
            return visibility.HasLabels() && !visibility.labelsOn;
        }

        else
        {
            if(contextObject != null && SelectedObjectsManagement.Instance.selectedObjects[0] != contextObject)
            {
                var visibility = contextObject.GetComponent<BodyPartVisibility>();
                return visibility.HasLabels() && !visibility.labelsOn;
            }
            else
            {
                var visibility = SelectedObjectsManagement.Instance.selectedObjects[0].GetComponent<BodyPartVisibility>();
                return visibility.HasLabels() && !visibility.labelsOn;
            }
        }
    }

    private bool HideLabelsOn()
    {
        if (SelectedObjectsManagement.Instance.selectedObjects.Count > 1)
            return false;

        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 0)
        {
            var visibility = contextObject.GetComponent<BodyPartVisibility>();
            return visibility.HasLabels() && visibility.labelsOn;
        }

        else
        {
            if (contextObject != null && SelectedObjectsManagement.Instance.selectedObjects[0] != contextObject)
            {
                var visibility = contextObject.GetComponent<BodyPartVisibility>();
                return visibility.HasLabels() && visibility.labelsOn;
            }
            else
            {
                var visibility = SelectedObjectsManagement.Instance.selectedObjects[0].GetComponent<BodyPartVisibility>();
                return visibility.HasLabels() && visibility.labelsOn;
            }

        }
    }

    private bool MovePositionOn()
    {
        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 0)
            return !contextObject.CompareTag("References") && !contextObject.CompareTag("Insertions");
        else
        {
            if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
                return !SelectedObjectsManagement.Instance.selectedObjects.Find(it => it.CompareTag("References"));
            else
                return !contextObject.CompareTag("References") && !contextObject.CompareTag("Insertions");
        }
    }

    private bool ResetPositionOn()
    {
        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 0)
        {
            var bodyPart = contextObject.GetComponent<TangibleBodyPart>();
            return bodyPart != null && bodyPart.displaced;
        }

        else
        {
            if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            {
                foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
                {
                    var bodyPart = item.GetComponent<TangibleBodyPart>();
                    if (bodyPart != null && bodyPart.displaced)
                        return true;
                }
                return false;
            }
            else
            {
                var bodyPart = contextObject.GetComponent<TangibleBodyPart>();
                return bodyPart != null && bodyPart.displaced;
            }
        }
    }

    private bool ShowInsertionsOn()
    {
        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 0)
        {
            var visibility = contextObject.GetComponent<BodyPartVisibility>();
            if (contextObject.CompareTag("Muscles"))
                return visibility.HasInsertions();
            else
                return visibility.HasInsertions() && visibility.AtLeastOneInsertionOff();
        }

        else
        {
            if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            {
                if(SelectedObjectsManagement.Instance.selectedObjects.Any(it => !it.CompareTag("Skeleton") && !it.CompareTag("Insertions") && !it.CompareTag("Muscles")))
                    return false;

                foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
                {
                    var visibility = item.GetComponent<BodyPartVisibility>();

                    if (visibility.HasInsertions() && visibility.CompareTag("Muscles"))
                        return visibility.HasInsertions();

                    if (visibility.HasInsertions() && visibility.AtLeastOneInsertionOff() && visibility.CompareTag("Skeleton"))
                        return true;
                }
                return false;
            }
            else
            {
                var visibility = contextObject.GetComponent<BodyPartVisibility>();
                if (contextObject.CompareTag("Muscles"))
                    return visibility.HasInsertions();
                else
                    return visibility.HasInsertions() && visibility.AtLeastOneInsertionOff();
            }
        }
    }

    private bool HideInsertionsOn()
    {
        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 0)
        {
            var visibility = contextObject.GetComponent<BodyPartVisibility>();
            return visibility.HasInsertions() && visibility.AtLeastOneInsertionOn();
        }

        else
        {
            if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            {
                if (SelectedObjectsManagement.Instance.selectedObjects.Any(it => !it.CompareTag("Skeleton") && !it.CompareTag("Insertions") && !it.CompareTag("Muscles")))
                    return false;

                foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
                {
                    var visibility = item.GetComponent<BodyPartVisibility>();
                    if (visibility.HasInsertions() && visibility.AtLeastOneInsertionOn())
                        return true;
                }
                return false;
            }
            else
            {
                var visibility = contextObject.GetComponent<BodyPartVisibility>();
                return visibility.HasInsertions() && visibility.AtLeastOneInsertionOn();
            }
        }
    }

    private bool ShowInsertionMuscleOn()
    {
        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 0)
        {
            var bodyPart = contextObject.GetComponent<BodyPartVisibility>();
            return bodyPart != null && bodyPart.HasInsertionMuscles();
        }

        else
        {
            if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            {
                foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
                {
                    var bodyPart = item.GetComponent<BodyPartVisibility>();
                    if (bodyPart == null)
                        return false;
                    if (!bodyPart.HasInsertionMuscles())
                        return false;
                }
                return true;
            }
            else
            {
                var bodyPart = contextObject.GetComponent<BodyPartVisibility>();
                return bodyPart != null && bodyPart.HasInsertionMuscles();
            }
        }
    }

    private bool ShowMuscleNervesOn()
    {
        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 0)
        {
            var bodyPart = contextObject.GetComponent<BodyPartVisibility>();
            return bodyPart != null && bodyPart.HasMuscleNerves();
        }

        else
        {
            if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            {
                foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
                {
                    var bodyPart = item.GetComponent<BodyPartVisibility>();
                    if (bodyPart == null)
                        return false;
                    if (!bodyPart.HasMuscleNerves())
                        return false;
                }
                return true;
            }
            else
            {
                var bodyPart = contextObject.GetComponent<BodyPartVisibility>();
                return bodyPart != null && bodyPart.HasMuscleNerves();
            }
        }
    }

    private bool HideMuscleNervesOn()
    {
        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 0)
        {
            var bodyPart = contextObject.GetComponent<BodyPartVisibility>();
            return bodyPart != null && bodyPart.HasMuscleNerves() && bodyPart.AtLeastOneMuscleNervesOn();
        }

        else
        {
            if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            {
                foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
                {
                    var bodyPart = item.GetComponent<BodyPartVisibility>();
                    if (bodyPart == null)
                        return false;
                    if (!bodyPart.HasMuscleNerves())
                        return false;
                    if (bodyPart.AtLeastOneMuscleNervesOn())
                        return true;
                }
                return true;
            }
            else
            {
                var bodyPart = contextObject.GetComponent<BodyPartVisibility>();
                return bodyPart != null && bodyPart.HasMuscleNerves() && bodyPart.AtLeastOneMuscleNervesOn();
            }
        }
    }

    private bool ShowNerveMusclesOn()
    {
        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 0)
        {
            var bodyPart = contextObject.GetComponent<BodyPartVisibility>();
            return bodyPart != null && bodyPart.HasNerveMuscles();
        }

        else
        {
            if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            {
                foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
                {
                    var bodyPart = item.GetComponent<BodyPartVisibility>();
                    if (bodyPart == null)
                        return false;
                    if (!bodyPart.HasNerveMuscles())
                        return false;
                }
                return true;
            }
            else
            {
                var bodyPart = contextObject.GetComponent<BodyPartVisibility>();
                return bodyPart != null && bodyPart.HasNerveMuscles();
            }
        }
    }

    private bool HideNerveMusclesOn()
    {
        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 0)
        {
            var bodyPart = contextObject.GetComponent<BodyPartVisibility>();
            return bodyPart != null && bodyPart.HasNerveMuscles() && bodyPart.AtLeastOneNerveMusclesOn();
        }

        else
        {
            if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            {
                foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
                {
                    var bodyPart = item.GetComponent<BodyPartVisibility>();
                    if (bodyPart == null)
                        return false;
                    if (!bodyPart.HasNerveMuscles())
                        return false;
                    if (bodyPart.AtLeastOneNerveMusclesOn())
                        return true;
                }
                return true;
            }
            else
            {
                var bodyPart = contextObject.GetComponent<BodyPartVisibility>();
                return bodyPart != null && bodyPart.HasNerveMuscles() && bodyPart.AtLeastOneNerveMusclesOn();
            }
        }
    }

    public bool ShowKeyColorsOn()
    {
        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 0)
            return KeyColors.Instance.HasKeyColor(contextObject.tag) && !KeyColors.Instance.IsActiveByTag(contextObject.tag);

        else
        {
            if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            {
                string prevTag = SelectedObjectsManagement.Instance.selectedObjects[0].tag;
                foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
                {
                    if (!KeyColors.Instance.HasKeyColor(item.tag) || item.tag != prevTag || KeyColors.Instance.IsActiveByTag(item.tag))
                        return false;
                    prevTag = item.tag;
                }
                return true;
            }
            else
                return KeyColors.Instance.HasKeyColor(contextObject.tag) && !KeyColors.Instance.IsActiveByTag(contextObject.tag);
        }
    }

    public bool HideKeyColorsOn()
    {
        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 0)
            return contextObject != null && KeyColors.Instance.HasKeyColor(contextObject.tag) && KeyColors.Instance.IsActiveByTag(contextObject.tag);

        else
        {
            if (contextObject == null || SelectedObjectsManagement.Instance.selectedObjects.Contains(contextObject))
            {
                string prevTag = SelectedObjectsManagement.Instance.selectedObjects[0].tag;
                foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
                {
                    if (!KeyColors.Instance.HasKeyColor(item.tag) || item.tag != prevTag || !KeyColors.Instance.IsActiveByTag(item.tag))
                        return false;
                    prevTag = item.tag;
                }
                return true;
            }
            else
                return KeyColors.Instance.HasKeyColor(contextObject.tag) && KeyColors.Instance.IsActiveByTag(contextObject.tag);
        }
    }

}
