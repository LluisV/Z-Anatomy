using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class CubeText : MonoBehaviour
{
    public Camera cam;
    private TextMeshPro text;
    public float fontSize;
    private MaterialPropertyBlock _propBlock;
    private Renderer _renderer;
    public Transform cubeCenter;
    public Material labelMaterial;
    private Material mat;
    private Color originalColor;
    public float maxAlpha = 1;
    private CubeBehaviour cubeScript;
    public CubeFace cubeFace;
    public CrossPlanesCube planesScript;
    public bool isPlanesCube;
    private void Start()
    {
        cubeScript = GetComponentInParent<CubeBehaviour>();
        text = gameObject.GetComponent<TextMeshPro>();
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = fontSize * 0.05f;
        text.material = labelMaterial;
        originalColor = labelMaterial.color;
        mat = new Material(text.material);
        _propBlock = new MaterialPropertyBlock();
        _renderer = text.renderer;
    }
    public void ColorChanged(Color color)
    {
        mat.color = color;
    }

    public void SetOriginalColor()
    {
        mat.color = originalColor;
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = cam.transform.rotation;
        UpdateColor();
    }

    void UpdateColor()
    {
        float angle = Vector3.Angle(cubeCenter.position - transform.position, -cam.transform.forward);
        float a = angle * angle * angle * angle * 0.000000025f;
        if (a > maxAlpha)
            a = maxAlpha;
        _renderer.enabled = a > .005f;

        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetColor("_FaceColor", new Color(mat.color.r, mat.color.g, mat.color.b, a));
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }

    private void OnMouseDown()
    {
        //We block the main raycast so we don't click other things
        RaycastObject.instance.raycastBlocked = true;
    }
    private void OnMouseUpAsButton()
    {
        cubeScript.SetCameraRotation(cubeFace);
        if(isPlanesCube)
            planesScript.SetPlane(cubeFace);
        RaycastObject.instance.raycastBlocked = false;
    }
}
