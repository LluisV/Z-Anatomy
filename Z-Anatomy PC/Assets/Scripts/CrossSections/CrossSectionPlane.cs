using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSectionPlane : MonoBehaviour
{
    public MeshManagement meshManagement;
    public string planePos;
    public string planeNormal;

    // Update is called once per frame
    void Update()
    {
        if (transform.hasChanged)
        {
            transform.hasChanged = false;
            SendPositionToShader();
        }
    }

    public void SendPositionToShader()
    {
        for (int i = 0; i < GlobalVariables.Instance.allBodyPartRenderers.Count; i++)
        {
            foreach (Material material in GlobalVariables.Instance.allBodyPartRenderers[i].materials)
            {
                material.SetVector(planePos, transform.position);
                material.SetVector(planeNormal, transform.up);
            }
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < GlobalVariables.Instance.allBodyPartRenderers.Count; i++)
        {
            foreach (Material material in GlobalVariables.Instance.allBodyPartRenderers[i].materials)
            {
                material.SetVector(planePos, Vector3.zero);
                material.SetVector(planeNormal, Vector3.zero);
            }
        }
    }

    private void OnEnable()
    {
        SendPositionToShader();
    }

}
