using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSectionPlane : MonoBehaviour
{
    public MeshManagement meshManagement;

    // Update is called once per frame
    void Update()
    {
        if (transform.hasChanged)
        {
            transform.hasChanged = false;
            SendPositionToShader();
        }
    }

    private void SendPositionToShader()
    {
        for (int i = 0; i < meshManagement.rendererMaterials.Count; i++)
        {
            foreach (Material material in meshManagement.rendererMaterials[i])
            {
                material.SetVector("_PlanePosition", transform.position);
                material.SetVector("_PlaneNormal", transform.up);
            }
        }

      /*  foreach (Material material in meshManagement.colorMaterials)
        {
            material.SetVector("_PlanePosition", transform.position);
            material.SetVector("_PlaneNormal", transform.up);
        }*/
    }

    private void OnDisable()
    {
        for (int i = 0; i < meshManagement.rendererMaterials.Count; i++)
        {
            foreach (Material material in meshManagement.rendererMaterials[i])
            {
                material.SetVector("_PlanePosition", Vector3.zero);
                material.SetVector("_PlaneNormal", Vector3.zero);
            }
        }
     /*   foreach (Material material in meshManagement.colorMaterials)
        {
            material.SetVector("_PlanePosition", Vector3.zero);
            material.SetVector("_PlaneNormal", Vector3.zero);
        }*/
    }

    private void OnEnable()
    {
        SendPositionToShader();
    }

}
