using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Represents a body part in a 3D model. 
/// Provides methods for handling selection, highlighting, and adding notes to the body part. 
/// Contains variables for storing information such as the distance to the camera, center, bounds, materials, and child objects.
/// </summary>
public class TangibleBodyPart : MonoBehaviour
{
    CameraController cam;
    [HideInInspector]
    public float distanceToCamera;
    [HideInInspector]
    public Vector3 center;
    [HideInInspector]
    public Bounds bounds;
    [HideInInspector]
    public NameAndDescription nameScript;
    [HideInInspector]
    public BodyPartVisibility visibilityScript;
    [HideInInspector]
    public Vector3 originalPos;
    [HideInInspector]
    public MeshCollider meshCollider;
    [HideInInspector]
    public List<TangibleBodyPart> childs;

    private Vector3 actualPos;
    private MeshRenderer mr;

    private List<Note> notes = new List<Note>();

    private Material[] secondaryMaterials;
    private Material[] primaryMaterials;

    private Label[] labels;

    [HideInInspector]
    public bool displaced;

    private float secondaryColorWeight;

    private void Awake()
    {
        meshCollider = GetComponent<MeshCollider>();
        mr = GetComponent<MeshRenderer>();
        nameScript = GetComponent<NameAndDescription>();
        visibilityScript = GetComponent<BodyPartVisibility>();
        cam = Camera.main.GetComponent<CameraController>();
        labels = GetComponentsInChildren<Label>(true);
        childs = GetComponentsInChildren<TangibleBodyPart>(true).ToList();
        childs.Remove(this);

        primaryMaterials = mr.sharedMaterials;
        distanceToCamera = meshCollider.bounds.extents.magnitude * 1.4f;
        center = meshCollider.bounds.center;
        bounds = meshCollider.bounds;
        originalPos = transform.position;
        actualPos = originalPos;
    }

    private void Start()
    {
        InitializeSecondaryMaterials();
    }

    /// <summary>
    /// Handles the selection/deselection of the game object when clicked.
    /// </summary>
    public void ObjectClicked()
    {
        //If the multiple selection is DISABLED
        if (!ActionControl.multipleSelection)
        {
            //If it is NOT the current camera target, we want to SELECT it
            if (cam.target != gameObject)
            {
                //Deselect the last selected
                SelectedObjectsManagement.Instance.DeselectAllObjects();
                //Select this object
                SelectedObjectsManagement.Instance.SelectObject(gameObject);
                SelectedObjectsManagement.Instance.ShowBodyPartInfo(gameObject);
                SelectedObjectsManagement.Instance.lastParentSelected = null;

                SearchEngine.Instance.ClearSearch();
                //Update the camera position and distance
                cam.SetTarget(gameObject);
                if(ActionControl.zoomSelected)
                    cam.CenterView(true);

                //ShowNotes();
            }
            //If it WAS the camera target, we want to DESELECT it
            else
            {
                //Now the camera has no target
                cam.SetTarget(null);
                //Deselect it
                SelectedObjectsManagement.Instance.DeselectObject(gameObject);
                Lexicon.Instance.SetHighlighted(null);

            }
        }
        //If the multiple selection is ENABLED
        else
        {
            //If it wasn't already selected, we want to SELECT it
            if (!visibilityScript.isSelected)
                SelectedObjectsManagement.Instance.SelectObject(gameObject);
            else
                SelectedObjectsManagement.Instance.DeselectObject(gameObject);

            SelectedObjectsManagement.Instance.ShowBodyPartInfo(gameObject);
        }

        ActionControl.Instance.AddCommand(new SelectCommand(SelectedObjectsManagement.Instance.selectedObjects), false);
        ActionControl.Instance.UpdateButtons();
        TranslateObject.Instance.UpdateSelected();
        Lexicon.Instance.UpdateSelected();
    }

    /// <summary>
    /// Returns a boolean indicating whether the object is currently selected or not.
    /// </summary>
    public bool IsSelected()
    {
        return visibilityScript.isSelected;
    }

    /// <summary>
    /// Sets the selection state of the object.
    /// </summary>
    public void SetSelectionState(bool state)
    {
        if(visibilityScript != null)
            visibilityScript.isSelected = state;
    }

    /// <summary>
    /// Sets the highlighted outline to the object.
    /// </summary>
    public void Highlight()
    {
        //OUTLINE MASK
        gameObject.layer = 7;
    }

    /// <summary>
    /// Sets the selected outline to the object.
    /// </summary>
    public void Select()
    {
        //OUTLINE MASK
        gameObject.layer = 7;
        SetSelectionState(true);
    }

    /// <summary>
    /// Deselects the object by changing its selection state and hiding its labels and notes.
    /// </summary>
    /// <param name="hideLabels">Optional. Whether to hide the labels or not. Default is true.</param>
    public void Deselect(bool hideLabels = true)
    {
        BodyPartVisibility isVisible = GetComponent<BodyPartVisibility>();
        SetSelectionState(false);
        gameObject.layer = 6;
        if(hideLabels)
            isVisible.HideLabels(false);
        //HideNotes();
    }

    /// <summary>
    /// If not selected, sets the highlighted outline to the object.
    /// </summary>
    public void MouseEnter()
    {
        //OUTLINE MASK
        if(gameObject.layer != 7)
            gameObject.layer = 13;
    }

    /// <summary>
    /// If highlighted, removes the highlighted outline of the object.
    /// </summary>
    public void MouseExit()
    {
        if (gameObject.layer == 13)
            gameObject.layer = 6;
    }

    public void AddNote(Note note)
    {
        notes.Add(note);
        note.bodyPart = this;
    }

    /// <summary>
    /// Sets the primary materials to the object.
    /// </summary>
    /// <param name="planeEnabled">A boolean to enable or disable the cross sections shader.</param>
    public void SetPrimaryMaterial(bool planeEnabled)
    {
        mr.sharedMaterials = primaryMaterials;
        foreach (Material material in mr.materials)
            material.SetFloat("_PlaneEnabled", planeEnabled ? 1f : 0f);
    }

    /// <summary>
    /// Sets the secondary materials to the object.
    /// </summary>
    /// <param name="planeEnabled">A boolean to enable or disable the cross sections shader.</param>
    public void SetSecondaryMaterial(bool planeEnabled)
    {
        mr.sharedMaterials = secondaryMaterials;
        foreach (Material material in mr.materials)
            material.SetFloat("_PlaneEnabled", planeEnabled ? 1f : 0f);
    }

    /// <summary>
    /// Moves the object in the specified direction by modifying its position. Also updates the position of the labels attached to the object and the position of its child objects that are not tagged as "Insertions".
    /// </summary>
    /// <param name="dir">The direction to move the object in.</param>
    public void Translate(Vector3 dir)
    {
        actualPos += dir;
        transform.position = actualPos;
        displaced = transform.position != originalPos;
        foreach (var label in labels)
            label.UpdatePos();
        foreach (var child in childs)
        {
            if(!child.CompareTag("Insertions"))
                child.Translate(Vector3.zero);
        }
    }

    /// <summary>
    /// Updates the bounds of the mesh collider to match the bounds of the mesh. 
    /// This function is useful when the mesh has changed and the collider must be updated accordingly.
    /// </summary>
    public void UpdateBounds()
    {
        //Refresh mesh
        var mesh = meshCollider.sharedMesh;
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;

        center = meshCollider.bounds.center;
        bounds = meshCollider.bounds;
    }

    /// <summary>
    /// Resets the object to its original position.
    /// </summary>
    public void ResetPosition()
    {
        transform.position = originalPos;
        actualPos = originalPos;
        displaced = false;
        foreach (var label in labels)
            label.UpdatePos();
        foreach (var child in childs)
            child.Translate(Vector3.zero);
        UpdateBounds();
        
    }

    private void InitializeSecondaryMaterials()
    {
        secondaryColorWeight = KeyColors.Instance.GetWeightByTag(tag);
        secondaryMaterials = new Material[primaryMaterials.Length];
        for (int i = 0; i < primaryMaterials.Length; i++)
        {
            var mat = KeyColors.Instance.GetSecondaryColor(tag, mr.sharedMaterials[i]);
            if (mat == null)
                continue;
            secondaryMaterials[i] = new Material(mat);
            secondaryMaterials[i].SetColor("_BaseColor", Color.Lerp(primaryMaterials[i].GetColor("_BaseColor"), secondaryMaterials[i].GetColor("_BaseColor"), secondaryColorWeight));
        }
    }

    private void HideNotes()
    {
        foreach (var note in notes)
        {
            note.Collapse();
            note.gizmo.gameObject.SetActive(false);
        }
    }

    private void ShowNotes()
    {
        foreach (var note in notes)
        {
            note.gizmo.gameObject.SetActive(true);
        }
    }


}
