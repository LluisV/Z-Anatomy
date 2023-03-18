using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TranslateObject : MonoBehaviour
{

    public static TranslateObject Instance;

    public Transform translationGizmo;
    public Transform explosionGizmo;
    public float size;
    [HideInInspector]
    public List<TangibleBodyPart> selected;
    private List<Vector3> prevPos;

    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        translationGizmo.localScale = Vector3.one * Camera.main.orthographicSize * size / CameraController.instance.defaulDistance;
    }

    public void SetGizmoCenter()
    {
        if (SelectedObjectsManagement.Instance.selectedObjects.Count == 0)
        {
            Disable();
            return;
        }
        
        selected = new List<TangibleBodyPart>();
        prevPos = new List<Vector3>();

        foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
        {
            TangibleBodyPart script = item.GetComponent<TangibleBodyPart>();
            if(script != null)
            {
                selected.Add(script);
                prevPos.Add(script.transform.position);
                script.UpdateBounds();
            }
        }

        translationGizmo.position = StaticMethods.GetBounds(SelectedObjectsManagement.Instance.selectedObjects).center;
        explosionGizmo.position = translationGizmo.position;
    }

    public void Translate(Vector3 delta)
    {
        foreach (var bodyPart in selected)
            bodyPart.Translate(delta);

        translationGizmo.Translate(delta, Space.World);
    }

    public void UpdateSelected()
    {
        SetGizmoCenter();
        Disable();
    }

    public void Disable()
    {
        translationGizmo.gameObject.SetActive(false);
    }

    public void Enable()
    {
        translationGizmo.gameObject.SetActive(true);
    }

    public void SaveAction()
    {
        List<Vector3> delta = new List<Vector3>();
        for (int i = 0; i < selected.Count; i++)
            delta.Add(selected[i].transform.position - prevPos[i]);

        ActionControl.Instance.AddCommand(new MoveCommand(selected, delta), false);
    }

    public void UpdatePrevPos()
    {
        prevPos = new List<Vector3>();
        foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
            prevPos.Add(item. transform.position); 
    }
}
