using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Commands
{
    public class ShowCommand : ICommand
    {

        List<GameObject> shown = new List<GameObject>();

        public ShowCommand(List<GameObject> shown)
        {
            this.shown = new List<GameObject>(shown);
        }

        public ShowCommand(GameObject shown)
        {
            this.shown.Add(shown);
        }

        public bool Equals(ICommand command)
        {
            return command.GetType() == GetType() && (command as ShowCommand).shown.SequenceEqual(shown);
        }

        public void Execute()
        {
            if (shown.Count > 0 && shown.All(it => it.IsLabel()))
                shown[0].transform.parent.GetComponent<BodyPartVisibility>().labelsOn = true;

            foreach (var item in shown)
            {
                BodyPartVisibility isVisibleScript = item.GetComponent<BodyPartVisibility>();
                if(isVisibleScript != null)
                    isVisibleScript.isVisible = true;
                item.SetActive(true);

                Label label = item.GetComponent<Label>();
                if(label != null && label.line != null)
                    label.line.gameObject.SetActive(true);

                SelectedObjectsManagement.Instance.activeObjects.Add(item);
            }
        }

        public void Undo()
        {
            if(shown.Count > 0 && shown.All(it => it.IsLabel()))
                shown[0].transform.parent.GetComponent<BodyPartVisibility>().labelsOn = false;

            foreach (var item in shown)
            {
                BodyPartVisibility isVisibleScript = item.GetComponent<BodyPartVisibility>();
                if (isVisibleScript != null)
                    isVisibleScript.isVisible = false;
                item.SetActive(false);

                Label label = item.GetComponent<Label>();
                if (label != null && label.line != null)
                    label.line.gameObject.SetActive(false);
                SelectedObjectsManagement.Instance.activeObjects.Remove(item);
            }
        }

        public bool IsEmpty()
        {
            return shown == null || shown.Count == 0;
        }
    }
}