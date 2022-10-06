using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeleteCommand : ICommand
{
    List<GameObject> deleted = new List<GameObject>();
    List<GameObject> selected = new List<GameObject>();
    bool showLabels;

    public DeleteCommand(List<GameObject> deleted, List<GameObject> selected = null, bool showLabels = true)
    {
        this.deleted = new List<GameObject>(deleted);
        if(selected != null)
            this.selected = new List<GameObject>(selected);
        this.showLabels = showLabels;
    }

    public DeleteCommand(GameObject deleted, bool showLabels = true)
    {
        this.deleted.Add(deleted);
        this.showLabels = showLabels;
    }

    public bool Equals(ICommand command)
    {
        return command.GetType() == GetType() && (command as DeleteCommand).deleted.SequenceEqual(deleted);
    }

    public void Execute()
    {
        SelectedObjectsManagement.Instance.DeleteList(deleted);
        if(SelectedObjectsManagement.Instance.activeObjects.Count == 1 && showLabels)
            SelectedObjectsManagement.Instance.activeObjects[0].GetComponent<Visibility>().ShowLabels();

        if(selected != null)
        {
            foreach (var item in selected)
            {
                item.transform.SetActiveParentsRecursively(true);
            }
        }
    }

    public void Undo()
    {
        foreach (var item in SelectedObjectsManagement.Instance.selectedObjects)
            item.GetComponent<Visibility>().HideLabels();

        foreach (var item in deleted)
        {
            Visibility isVisibleScript = item.GetComponent<Visibility>();
            if (isVisibleScript != null)
                isVisibleScript.isVisible = true;
            item.SetActive(true);
            Label label = item.GetComponent<Label>();
            if (label != null && label.line != null)
                label.line.gameObject.SetActive(true);
            SelectedObjectsManagement.Instance.activeObjects.Add(item);
        }

        ActionControl.Instance.UpdateButtons();
    }

    public bool IsEmpty()
    {
        return deleted == null || deleted.Count == 0;
    }
}
