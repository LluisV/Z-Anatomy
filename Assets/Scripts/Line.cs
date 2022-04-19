using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [HideInInspector]
    public Material lineMaterial;
    public Camera cam;
    public CameraController camScript;
    private MaterialPropertyBlock _propBlock;
    private LineRenderer _renderer;
    Color lineColor;
    private Transform originPoint;
    private float initialSize;
    private float maxAlpha = .5f;
    private bool onRoutine;
    private void Awake()
    {
        Transform minPoint = transform.Find("minPoint");
        Transform maxPoint = transform.Find("maxPoint");
        _renderer = gameObject.AddComponent<LineRenderer>();
        DrawLine(minPoint.position, maxPoint.position);
        _propBlock = new MaterialPropertyBlock();

        lineColor = lineMaterial.color;
        gameObject.SetActive(true);

        initialSize = 0.0035f;
        try
        {
            /*if (Vector3.Distance(maxPoint.position, transform.parent.position) > Vector3.Distance(minPoint.position, transform.parent.position))
                originPoint = minPoint;
            else
                originPoint = maxPoint;*/
            originPoint = maxPoint;
        }
        catch (System.Exception)
        {
            originPoint = transform.parent;
        }
    }

    // Update is called once per frame
    void Update()
    {
       // if(!onRoutine)
        {
            UpdateColor();
            _renderer.startWidth = initialSize * cam.orthographicSize;
            _renderer.endWidth = initialSize * cam.orthographicSize;
        }
    }

    void UpdateColor()
    {
   //     onRoutine = true;
        //yield return new WaitForSeconds(0.1f);
        float angle = Vector3.Angle(originPoint.position - transform.position, -cam.transform.forward);
        float a = angle * angle * angle * angle * 0.00000005f;
        //text.color = new Color(1, 1, 1, a);
        _renderer.enabled = a > .005f;
        if (a > maxAlpha)
            a = maxAlpha;

        //    lineColor.a = a;

        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetColor("_Color", new Color(lineColor.r, lineColor.g, lineColor.b, a));// lineColor);
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
       // onRoutine = false;
    }

    void DrawLine(Vector3 start, Vector3 end)
    {
      //  transform.position = start;
        _renderer.material = lineMaterial;
        _renderer.SetColors(lineColor, lineColor);
        _renderer.SetWidth(0.005f, 0.005f);
        _renderer.SetPosition(0, start);
        _renderer.SetPosition(1, end);
    }


}
