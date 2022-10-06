using Assets.Scripts.Commands;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Visibility : MonoBehaviour
{
    public bool isVisible;
    public bool isSelected;
    [HideInInspector]
    public NameAndDescription nameScript;
    Label[] labels;
    [HideInInspector]
    public List<BodyPart> insertions = new List<BodyPart>();
    [HideInInspector]
    public List<BodyPart> insertionMuscles = new List<BodyPart>();
    [HideInInspector]
    public List<BodyPart> nerveMuscles = new List<BodyPart>();
    [HideInInspector]
    public List<BodyPart> muscleNerves = new List<BodyPart>();
    Visibility[] childs;

    [HideInInspector]
    public bool labelsOn;
    [HideInInspector]
    public bool insertionsOn;
    [HideInInspector]
    public LexiconElement lexiconElement;

    private void Awake()
    {
        nameScript = GetComponent<NameAndDescription>();
        labels = transform.GetComponentsInDirectChildren<Label>();
        childs = GetComponentsInChildren<Visibility>(true);

        if (CompareTag("Skeleton"))
            isVisible = true;
        if (GetComponent<Label>() != null)
            isVisible = false;
        if (CompareTag("Insertions"))
            isVisible = false;



    }

    private void Start()
    {
        if (CompareTag("Skeleton"))
        {
            var childInsertions = GetComponentsInChildren<BodyPart>(true).Where(it => it.name.Contains(".o") || it.name.Contains(".e")).ToList();
            insertions.AddRange(childInsertions);
        }
        else if (CompareTag("Muscles"))
        {
            if (nameScript.originalName.IsRight())
            {
                var rightInsertions = GlobalVariables.Instance.insertions.FindAll(it => it.nameScript.originalName.IsRight() && it.nameScript.originalName.RemoveSuffix() + ".r" == nameScript.originalName);
                insertions.AddRange(rightInsertions);
            }
            else if (nameScript.originalName.IsLeft())
            {
                var leftInsertions = GlobalVariables.Instance.insertions.FindAll(it => it.nameScript.originalName.IsLeft() && it.nameScript.originalName.RemoveSuffix() + ".l" == nameScript.originalName);
                insertions.AddRange(leftInsertions);
            }

            MuscleGroups.Instance.GetGroupInsertions(this);
            NerveGroups.Instance.GetMuscleNerves(this);
        }
        else if(CompareTag("Insertions"))
        {

            if (nameScript.originalName.IsRight())
            {
                var insertionMuscle = GlobalVariables.Instance.muscles.Find(it => it.nameScript.originalName == nameScript.originalName.RemoveSuffix() + ".r");
                if (insertionMuscle != null)
                    insertionMuscles.Add(insertionMuscle);
            }
            else
            {
                var insertionMuscle = GlobalVariables.Instance.muscles.Find(it => it.nameScript.originalName == nameScript.originalName.RemoveSuffix() + ".l");
                if (insertionMuscle != null)
                    insertionMuscles.Add(insertionMuscle);
            }

            MuscleGroups.Instance.GetConvergingMuscles(this);
        }
        else if(CompareTag("Nervous"))
        {
            NerveGroups.Instance.GetNerveMuscles(this);
        }
    }

    public void ShowLabels(bool saveInHistory = false)
    {
        if (labels == null)
            return;

        List<GameObject> shown = new List<GameObject>();

        foreach (var label in labels)
        {
            shown.Add(label.gameObject);
            label.gameObject.SetActive(true);
            label.GetComponent<Visibility>().isVisible = true;
            if (label.line != null)
                label.line.gameObject.SetActive(true);
        }

        if(saveInHistory)
            ActionControl.Instance.AddCommand(new ShowCommand(shown), false);

        labelsOn = true;
    }

    public void HideLabels(bool saveInHistory = false)
    {
        if (labels == null)
            return;

        List<GameObject> hidden = new List<GameObject>();

        foreach (var label in labels)
        {
            hidden.Add(label.gameObject);
            label.gameObject.SetActive(false);
            label.GetComponent<Visibility>().isVisible = false;
            if(label.line != null)
                label.line.gameObject.SetActive(false);
        }

        if(saveInHistory)
            ActionControl.Instance.AddCommand(new DeleteCommand(hidden), false);

        labelsOn = false;
    }

    public List<GameObject> ShowInsertions()
    {
        if (insertions == null)
            return null;

        List<GameObject> shown = new List<GameObject>();

        if(CompareTag("Skeleton"))
        {
            foreach (var insertion in insertions)
            {
                if (!insertion.visibilityScript.isVisible)
                    shown.Add(insertion.gameObject);
                insertion.visibilityScript.isVisible = true;
                insertion.gameObject.SetActive(true);
            }
            foreach (var child in childs)
                child.insertionsOn = true;
            insertionsOn = true;
        }
        else if(CompareTag("Muscles"))
        {
            List<GameObject> musclesToHide = new List<GameObject>();

            foreach (var insertion in insertions)
            {
                if (!insertion.visibilityScript.isVisible)
                    shown.Add(insertion.gameObject);
                insertion.visibilityScript.isVisible = true;
                insertion.gameObject.SetActive(true);
                insertion.transform.SetActiveParentsRecursively(true, shown);
                SelectedObjectsManagement.Instance.SelectObject(insertion.gameObject);

                foreach (var muscle in MuscleGroups.Instance.GetGroupMuscles(this))
                    musclesToHide.Add(muscle.gameObject);
            }

            foreach (var muscle in musclesToHide)
            {
                muscle.SetActive(false);
                muscle.GetComponent<Visibility>().isVisible = false;
                SelectedObjectsManagement.Instance.DeselectObject(gameObject);
            }

            ActionControl.Instance.AddCommand(new DeleteCommand(musclesToHide), false);

            isVisible = false;
            SelectedObjectsManagement.Instance.DeselectObject(gameObject);
            insertionsOn = true;
            gameObject.SetActive(false);
        }

        return shown;
    }

    public List<GameObject> HideInsertions()
    {
        if (insertions == null)
            return null;

        List<GameObject> hidden = new List<GameObject>();

        foreach (var insertion in insertions)
        {
            if (insertion.visibilityScript.isVisible)
                hidden.Add(insertion.gameObject);

            insertion.gameObject.SetActive(false);
            insertion.GetComponent<Visibility>().isVisible = false;
        }
        foreach (var child in childs)
            child.insertionsOn = false;
        insertionsOn = false;

        return hidden;
    }

    public List<GameObject> ShowMuscleNerves()
    {
        List<GameObject> shown = new List<GameObject>();

        foreach (var nerve in muscleNerves)
        {
            if (!nerve.visibilityScript.isVisible)
                shown.Add(nerve.gameObject);

            nerve.visibilityScript.isVisible = true;
            nerve.gameObject.SetActive(true);
            nerve.transform.SetActiveParentsRecursively(true, shown);
            SelectedObjectsManagement.Instance.SelectObject(nerve.gameObject);
        }

        SelectedObjectsManagement.Instance.DeselectObject(gameObject);

        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 1)
            HierarchyBar.Instance.Set(SelectedObjectsManagement.Instance.selectedObjects[0].transform);


        return shown;
    }

    public List<GameObject> ShowNerveMuscles()
    {
        List<GameObject> shown = new List<GameObject>();

        foreach (var muscle in nerveMuscles)
        {
            if (!muscle.visibilityScript.isVisible)
                shown.Add(muscle.gameObject);

            muscle.visibilityScript.isVisible = true;
            muscle.gameObject.SetActive(true);
            muscle.transform.SetActiveParentsRecursively(true, shown);
            SelectedObjectsManagement.Instance.SelectObject(muscle.gameObject);
        }

        SelectedObjectsManagement.Instance.DeselectObject(gameObject);
        return shown;
    }

    public List<GameObject> ShowMuscle()
    {
        List<GameObject> shown = new List<GameObject>();

        foreach (var muscle in insertionMuscles)
        {
            if (!muscle.visibilityScript.isVisible)
                shown.Add(muscle.gameObject);

            muscle.visibilityScript.isVisible = true;
            muscle.gameObject.SetActive(true);
            muscle.transform.SetActiveParentsRecursively(true, shown);
            SelectedObjectsManagement.Instance.SelectObject(muscle.gameObject);
        }

        SelectedObjectsManagement.Instance.DeselectObject(gameObject);
        return shown;
    }

    public bool HasLabels()
    {
        return labels != null && labels.Length > 0;
    }

    public bool HasInsertions()
    {
        return insertions != null && insertions.Count > 0;
    }

    public bool AtLeastOneInsertionOn()
    {
        return insertions.Any(it => it.visibilityScript.isVisible);
    }

    public bool AtLeastOneInsertionOff()
    {
        return insertions.Any(it => !it.visibilityScript.isVisible);

    }

    public bool AtLeastOneNerveMusclesOn()
    {
        return nerveMuscles.Any(it => it.visibilityScript.isVisible);
    }

    public bool AtLeastOneNerveMusclesOff()
    {
        return nerveMuscles.Any(it => !it.visibilityScript.isVisible);
    }

    public bool AtLeastOneMuscleNervesOn()
    {
        return muscleNerves.Any(it => it.visibilityScript.isVisible);
    }

    public bool AtLeastOneMuscleNervesOff()
    {
        return muscleNerves.Any(it => !it.visibilityScript.isVisible);
    }

    public bool HasInsertionMuscles()
    {
        return insertionMuscles != null && insertionMuscles.Count > 0;
    }

    public bool HasNerveMuscles()
    {
        return nerveMuscles != null && nerveMuscles.Count > 0;

    }

    public bool HasMuscleNerves()
    {
        return muscleNerves != null && muscleNerves.Count > 0;

    }
}
