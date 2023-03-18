using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Cutter : MonoBehaviour
{
    private RaycastHit hit;
    int layer_mask1;
    int layer_mask2;
    LayerMask finalmask;

    public MeshRenderer meshRenderer;

    private void Awake()
    {
        layer_mask1 = LayerMask.GetMask("Body");
        layer_mask2 = LayerMask.GetMask("Outline");
        finalmask = layer_mask1 | layer_mask2;
    }

    // Update is called once per frame
    void Update()
    {
        if(Mouse.current.leftButton.isPressed && ActionControl.scalpel)
        {
            Ray raycast = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(raycast, out hit, 100, finalmask))
            {
                MeshCollider meshCollider = hit.collider as MeshCollider;
                MeshFilter meshFilter = hit.collider.GetComponent<MeshFilter>();
                if (meshCollider == null || meshCollider.sharedMesh == null)
                    return;

                Mesh mesh = meshCollider.sharedMesh;
                Vector3[] vertices = mesh.vertices;
                int[] triangles = mesh.triangles;
                Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
                Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
                Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
                Transform hitTransform = hit.collider.transform;
                p0 = hitTransform.TransformPoint(p0);
                p1 = hitTransform.TransformPoint(p1);
                p2 = hitTransform.TransformPoint(p2);
                Debug.DrawLine(p0, p1);
                Debug.DrawLine(p1, p2);
                Debug.DrawLine(p2, p0);

                triangles[hit.triangleIndex * 3 + 0] = 0;
                triangles[hit.triangleIndex * 3 + 1] = 0;
                triangles[hit.triangleIndex * 3 + 2] = 0;

                mesh.triangles = triangles;
                meshFilter.mesh = mesh;
            }
        }
    }
}
