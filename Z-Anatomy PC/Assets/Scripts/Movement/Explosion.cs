using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public static Explosion Instance;

    public Transform gizmo;
    public float size;
    public float velocity;

    private bool isEnabled = false;

    [HideInInspector]
    public float minDistance;
    [HideInInspector]
    public float maxDistance;

    private void Awake()
    {
        Instance = this;
    }

    public void Disable()
    {
        isEnabled = false;
        gizmo.gameObject.SetActive(false);
    }

    public void OnExplosionClick()
    {
        isEnabled = !isEnabled;
        gizmo.gameObject.SetActive(isEnabled);
    }

    public void Explode(float magnitude)
    {
        foreach (var bodyPart in TranslateObject.Instance.selected.Where(item => !item.GetComponentsInParent<TangibleBodyPart>().Any(it => it != item && TranslateObject.Instance.selected.Contains(it))))
        {

            Vector3 direction = (gizmo.transform.position - bodyPart.originalPos);
            Vector3 delta = direction * magnitude * velocity;

            if (Vector3.Distance(bodyPart.transform.position + delta, gizmo.position) > Vector3.Distance(bodyPart.originalPos, gizmo.position))
                bodyPart.Translate(delta);
            else
                bodyPart.transform.position = bodyPart.originalPos;

        }
    }

    public Vector3 NearestVertexTo(Mesh mesh, Vector3 point)
    {
        // convert point to local space
        point = transform.InverseTransformPoint(point);


        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;
        // scan all vertices to find nearest
        foreach (Vector3 vertex in mesh.vertices)
        {
            Vector3 diff = point - vertex;
            float distSqr = diff.sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                nearestVertex = vertex;
            }
        }
        // convert nearest vertex back to world space
        return transform.TransformPoint(nearestVertex);

    }
}
