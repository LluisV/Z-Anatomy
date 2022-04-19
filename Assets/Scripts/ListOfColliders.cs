using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListOfColliders : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> currentCollisions = new List<GameObject>();
    [HideInInspector]
    public SelectedObjectsManagement selectedObjecstMng;
    private void OnTriggerEnter(Collider other)
    {
        // Add the GameObject collided with to the list.
        if(other.gameObject.layer == 6)
        {
            BodyPart script = other.gameObject.GetComponent<BodyPart>();
            if(script != null)
            {
                script.isSelected = true;
                selectedObjecstMng.SelectObject(other.gameObject);
                currentCollisions.Add(other.gameObject);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {

        // Remove the GameObject collided with from the list.
       /* BodyPart script = other.gameObject.GetComponent<BodyPart>();
        if (script != null)
        {
            script.isSelected = false;
            selectedObjecstMng.DeselectObjet(other.gameObject);
            currentCollisions.Remove(other.gameObject);
        }*/
    }
}
