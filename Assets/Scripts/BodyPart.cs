using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BodyPart : MonoBehaviour
{
    public bool selectable = true;
    public GameObject[] otherVersion;
    CameraController cam;
    private int selectedVersion = 0;
    [HideInInspector]
    public float distanceToCamera;
    [HideInInspector]
    public bool isSelected;
    [HideInInspector]
    public Vector3 center;
    [HideInInspector]
    public Bounds bounds;
    [HideInInspector]
    public NameAndDescription nameScript;

    private void Awake()
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        distanceToCamera = meshCollider.bounds.extents.magnitude * 1.75f;
        center = meshCollider.bounds.center;
        bounds = meshCollider.bounds;
    }

    private void Start()
    {

        cam = Camera.main.GetComponent<CameraController>();
        if (otherVersion.Length > 0)
            EnableDisableChildrenRenderers(otherVersion[selectedVersion], false);
        nameScript = GetComponent<NameAndDescription>();
        // GetComponent<Outline>().enabled = false;
        //SetTranslatedName();
    }

    public void ObjectClicked()
    {
        if (!selectable)
            return;

        //Reset visited objects stack
        NamesManagement.instance.ResetStack();

        //If the delete tool is enabled, just delete it
        if (ActionControl.canErase)
        {
            Delete();
            return;
        }

        //If the multiple selection is DISABLED
        if (!ActionControl.multipleSelection)
        {
            //If it is NOT the current camera target, we want to SELECT it
            if (cam.target != gameObject)
            {
                //Deselect the last selected
                SelectedObjectsManagement.instance.DeselectAllObjects();
                //Select this one
                SelectedObjectsManagement.instance.SelectObject(gameObject);
                SelectedObjectsManagement.instance.EnableDisableButtons();
                //Set the name
                NamesManagement.instance.SetTitle(gameObject.name);
                NamesManagement.instance.SetDescription(nameScript.Description(), gameObject);
                //If the title is not on screen, show it.
                // else
                // NamesManagement.instance.ShowTitle();
                //Update the camera position and distance
                cam.target = gameObject;
                cam.cameraCenter = center;
                cam.lastDistance = cam.distance;
                cam.UpdateDistance(distanceToCamera);

                ActionControl.someObjectSelected = true;
            }
            //If it WAS the camera target, we want to DESELECT it
            else
            {
                ActionControl.someObjectSelected = false;
                //Now the camera has no target
                cam.target = null;
                //We update the camera to it's last distance
                // cam.UpdateDistance(cam.lastDistance);
                //Finally deselect it
                SelectedObjectsManagement.instance.DeselectObjet(gameObject);
                SelectedObjectsManagement.instance.DeselectAllChildren(gameObject.transform);
                SelectedObjectsManagement.instance.EnableDisableButtons();
            }
        }
        //If the multiple selection is ENABLED
        else
        {
            //If it wasn't already selected, we want to SELECT it
            if (!isSelected)
            {
                SelectedObjectsManagement.instance.SelectObject(gameObject);
                SelectedObjectsManagement.instance.EnableDisableButtons();
            }
            //If it was already selected, we want to DESELECT it
            else
            {
                //Then whe deselect it
                SelectedObjectsManagement.instance.DeselectObjet(gameObject);
                SelectedObjectsManagement.instance.DeselectAllChildren(gameObject.transform);
                SelectedObjectsManagement.instance.EnableDisableButtons();
            }
            ActionControl.someObjectSelected = SelectedObjectsManagement.instance.selectedObjects.Count > 0;
        }

    }

    private void Delete()
    {
        if (isSelected)
            SelectedObjectsManagement.instance.DeleteSelected();
        else
            SelectedObjectsManagement.instance.DeleteClicked(gameObject);
    }

    void EnableDisableChildrenRenderers(GameObject go, bool state)
    {
        foreach (var item in go.GetComponentsInChildren<Renderer>())
        {
            item.enabled = state;
        }
    }
}
