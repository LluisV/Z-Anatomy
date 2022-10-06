using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SelectCommand : ICommand
{
    List<GameObject> selected;

    public SelectCommand(List<GameObject> selected)
    {
        this.selected = new List<GameObject>(selected);
    }

    public bool IsEmpty()
    {
        return selected == null || selected.Count == 0;
    }

    public bool Equals(ICommand command)
    {
        return command.GetType() == GetType() && (command as SelectCommand).selected.SequenceEqual(selected);
    }

    public void Execute()
    {
        SelectedObjectsManagement.Instance.DeselectAllObjects();
        bool hasLabels = selected.Any(it => it.IsLabel());
        foreach (GameObject item in selected.ToList())
        {
            SelectedObjectsManagement.Instance.SelectObject(item);
            item.transform.SetActiveParentsRecursively(true);
        }

        if(selected.Count > 0 && ActionControl.zoomSelected)
            CameraController.instance.CenterView(true);
        if (selected.Count == 1)
            SelectedObjectsManagement.Instance.ShowBodyPartInfo(selected[0]);

        ActionControl.Instance.UpdateButtons();
    }

    public void Undo()
    {
        foreach (GameObject item in selected.ToList())
            SelectedObjectsManagement.Instance.DeselectObject(item);
    }
}
