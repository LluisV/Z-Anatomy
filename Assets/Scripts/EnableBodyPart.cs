using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnableBodyPart : MonoBehaviour
{
    public MeshManagement meshMng;
    public GameObject bodyPart;
    public bool bodyPartEnabled = false;
    bool bodyPartColorEnabled = false;
    public Color selectedColor;
    public Material colorEnabledMaterial;
    public Image img;
    public void enableBodyPart()
    {
        img.material = null;
        //Body Part ON
        if (!bodyPartEnabled)
        {
            img.color = selectedColor;
            bodyPartEnabled = true;
        }
        //Body part ON COLOR
        else if (bodyPartEnabled && !bodyPartColorEnabled)
        {
           // meshMng.SetColors(bodyPart.tag);
            img.color = Color.white;
            img.material = colorEnabledMaterial;
            bodyPartColorEnabled = true;
        }
        //Body part OFF
        else if(bodyPartColorEnabled && bodyPartColorEnabled)
        {
            img.color = Color.white;
            bodyPartEnabled = false;
            bodyPartColorEnabled = false;
           // meshMng.SetNormalColorByTag(bodyPart.tag);
        }
        SetActiveRecursively(bodyPart.transform, bodyPartEnabled);
        TreeViewCanvas.instance.UpdateTreeViewCheckboxes();
    }

    private void SetActiveRecursively(Transform parent, bool state)
    {
        IsVisible parentScript = parent.GetComponent<IsVisible>();
        if (parentScript != null)
        {
            parent.gameObject.SetActive(state);
            parentScript.isVisible = state;
        }

        foreach (Transform child in parent)
        {
            IsVisible script = child.GetComponent<IsVisible>();
            if (script != null)
            {
                child.gameObject.SetActive(state);
                script.isVisible = state;
            }
            if (child.childCount > 0)
                SetActiveRecursively(child, state);

        }
    }
}
