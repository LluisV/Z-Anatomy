using Assets.Scripts.Commands;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class manages the visibility of objects in the scene. It contains methods to find insertions, nerves, and converging muscles, and add them to appropriate lists. It also has functions to show or hide labels associated with the object.
/// </summary>
public class BodyPartVisibility : MonoBehaviour
{
    public bool isVisible;
    public bool isSelected;
    [HideInInspector]
    public NameAndDescription nameScript;
    Label[] labels;
    [HideInInspector]
    public List<TangibleBodyPart> insertions = new List<TangibleBodyPart>();
    [HideInInspector]
    public List<TangibleBodyPart> insertionMuscles = new List<TangibleBodyPart>();
    [HideInInspector]
    public List<TangibleBodyPart> nerveMuscles = new List<TangibleBodyPart>();
    [HideInInspector]
    public List<TangibleBodyPart> muscleNerves = new List<TangibleBodyPart>();
    BodyPartVisibility[] childs;

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
        childs = GetComponentsInChildren<BodyPartVisibility>(true);

        if (CompareTag("Skeleton"))
            isVisible = true;
        if (GetComponent<Label>() != null)
            isVisible = false;
        if (CompareTag("Insertions"))
            isVisible = false;
    }

    private void Start()
    {
        // Check the tag of the game object and execute appropriate code for each case
        if (CompareTag("Skeleton"))
        {
            AddChildInsertions();
        }
        else if (CompareTag("Muscles"))
        {
            FindInsertionsAndNerves();
        }
        else if(CompareTag("Insertions"))
        {
            FindInsertionMuscleAndConvergingMuscles();
        }
        else if(CompareTag("Nervous"))
        {
            // Get the nerve muscles for this nerve
            NerveGroups.Instance.GetNerveMuscles(this);
        }
    }

    ///<summary>
    /// Finds the insertions and nerves for the muscle.
    ///</summary>
    public void FindInsertionsAndNerves()
    {
        // If the muscle is for the right side, find all right insertions with the same name and add them to the insertions list
        if (nameScript.originalName.IsRight())
        {
            var rightInsertions = GlobalVariables.Instance.insertions.FindAll(it => it.nameScript.originalName.IsRight() && it.nameScript.originalName.RemoveSuffix() + ".r" == nameScript.originalName);
            insertions.AddRange(rightInsertions);
        }
        // If the muscle is for the left side, find all left insertions with the same name and add them to the insertions list
        else if (nameScript.originalName.IsLeft())
        {
            var leftInsertions = GlobalVariables.Instance.insertions.FindAll(it => it.nameScript.originalName.IsLeft() && it.nameScript.originalName.RemoveSuffix() + ".l" == nameScript.originalName);
            insertions.AddRange(leftInsertions);
        }

        // Get the muscle group insertions and muscle nerves for this muscle
        MuscleGroups.Instance.GetGroupInsertions(this);
        NerveGroups.Instance.GetMuscleNerves(this);
    }

    /// <summary>
    /// This function finds the muscle with the same name as the current insertion's name, adds it to the insertion muscles list if found and
    /// gets the converging muscles for this insertion. It checks whether the insertion is for the left or right side and finds the corresponding muscle with the same name.
    /// </summary>
    public void FindInsertionMuscleAndConvergingMuscles()
    {
        if (nameScript.originalName.IsRight())
        {
            // If the insertion is for the right side, find the right muscle with the same name and add it to the insertion muscles list
            var insertionMuscle = GlobalVariables.Instance.muscles.Find(it => it.nameScript.originalName == nameScript.originalName.RemoveSuffix() + ".r");
            if (insertionMuscle != null)
                insertionMuscles.Add(insertionMuscle);
        }
        else
        {
            // If the insertion is for the left side, find the left muscle with the same name and add it to the insertion muscles list
            var insertionMuscle = GlobalVariables.Instance.muscles.Find(it => it.nameScript.originalName == nameScript.originalName.RemoveSuffix() + ".l");
            if (insertionMuscle != null)
                insertionMuscles.Add(insertionMuscle);
        }
        // Get the converging muscles for this insertion
        MuscleGroups.Instance.GetConvergingMuscles(this);
    }


    /// <summary>
    /// Adds all child body parts containing ".o" or ".e" in their name to the insertions list.
    /// </summary>
    private void AddChildInsertions()
    {
        var childInsertions = GetComponentsInChildren<TangibleBodyPart>(true).Where(it => it.name.Contains(".o") || it.name.Contains(".e")).ToList();
        insertions.AddRange(childInsertions);
    }

    /// <summary>
    /// Shows labels associated with this object (if it has labels)
    /// </summary>
    /// <param name="saveInHistory">Optional parameter. Whether to save the command in history or not</param>
    public void ShowLabels(bool saveInHistory = false)
    {
        if (labels == null)
            return;

        List<GameObject> shown = new List<GameObject>();

        foreach (var label in labels)
        {
            shown.Add(label.gameObject);
            label.gameObject.SetActive(true);
            label.GetComponent<BodyPartVisibility>().isVisible = true;
            if (label.line != null)
                label.line.gameObject.SetActive(true);
        }

        if(saveInHistory)
            ActionControl.Instance.AddCommand(new ShowCommand(shown), false);

        labelsOn = true;
    }

    /// <summary>
    /// Hides all labels and their connecting lines.
    /// </summary>
    /// <param name="saveInHistory">Optional parameter. If set to true, a record of the labels hidden will be added to the action history.</param>
    public void HideLabels(bool saveInHistory = false)
    {
        if (labels == null)
            return;

        List<GameObject> hidden = new List<GameObject>();

        foreach (var label in labels)
        {
            hidden.Add(label.gameObject);
            label.gameObject.SetActive(false);
            label.GetComponent<BodyPartVisibility>().isVisible = false;
            if(label.line != null)
                label.line.gameObject.SetActive(false);
        }

        if(saveInHistory)
            ActionControl.Instance.AddCommand(new DeleteCommand(hidden), false);

        labelsOn = false;
    }

    /// <summary>
    /// Shows all insertions for a skeleton or muscle object.
    /// </summary>
    /// <returns>A list of all gameobjects that were shown.</returns>
    public List<GameObject> ShowInsertions()
    {
        if (insertions == null)
            return null;

        List<GameObject> shown = new List<GameObject>();

        if(CompareTag("Skeleton"))
        {
            shown = ShowBonesInsertions();
        }
        else if(CompareTag("Muscles"))
        {
            shown = ShowMuscleInsertions();
        }

        return shown;
    }

    /// <summary>
    /// Shows all bone insertions.
    /// </summary>
    /// <returns>A list of all gameobjects that were shown.</returns>
    private List<GameObject> ShowBonesInsertions()
    {
        List<GameObject> shown = new List<GameObject>();

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

        return shown;
    }

    /// <summary>
    /// Shows all muscle insertions and hides the muscles that are part of the same muscle group as the current insertion.
    /// </summary>
    /// <returns>A list of all gameobjects that were shown.</returns>
    private List<GameObject> ShowMuscleInsertions()
    {
        List<GameObject> shown = new List<GameObject>();
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
            muscle.GetComponent<BodyPartVisibility>().isVisible = false;
            SelectedObjectsManagement.Instance.DeselectObject(gameObject);
        }

        ActionControl.Instance.AddCommand(new DeleteCommand(musclesToHide), false);

        isVisible = false;
        SelectedObjectsManagement.Instance.DeselectObject(gameObject);
        insertionsOn = true;
        gameObject.SetActive(false);

        return shown;
    }

    /// <summary>
    /// Hides the insertions for the current object.
    /// </summary>
    /// <returns>A list of hidden insertion game objects.</returns>
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
            insertion.GetComponent<BodyPartVisibility>().isVisible = false;
        }
        foreach (var child in childs)
            child.insertionsOn = false;
        insertionsOn = false;

        return hidden;
    }

    /// <summary>
    /// Shows the nerves for the muscle
    /// </summary>
    /// <returns>List of gameobjects that were shown</returns>
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

    ///<summary>Shows the nerves connected to the muscle</summary>
    ///<returns>List of game objects that have been shown</returns>
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

    /// <summary>
    /// Shows the muscles connected to the insertion points of this game object.
    /// </summary>
    /// <returns>A list of game objects that were shown.</returns>
    public List<GameObject> ShowInsertionMuscles()
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

    /// <summary>
    /// Checks if the game object has any labels associated with it.
    /// </summary>
    /// <returns>True if the game object has labels, false otherwise.</returns>
    public bool HasLabels()
    {
        return labels != null && labels.Length > 0;
    }

    /// <summary>
    /// Checks if the game object has any insertions associated with it.
    /// </summary>
    /// <returns>True if the game object has insertions, false otherwise.</returns>
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
