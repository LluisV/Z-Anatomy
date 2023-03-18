using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{

    private Camera cam;
    private MaterialPropertyBlock _propBlock;
    private Renderer _renderer;
    public Transform center;
    private Material mat;
    private Color originalColor;
    public float maxAlpha;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Start()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<MeshRenderer>();
        mat = _renderer.material;
        originalColor = mat.color;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateColor();
    }

    public void ColorChanged(Color color)
    {
        mat.color = color;
    }

    public void SetOriginalColor()
    {
        mat.color = originalColor;
    }

    void UpdateColor()
    {
        float angle = Vector3.Angle(center.position - transform.position, -cam.transform.forward);
        float a = angle * angle * angle * angle * 0.000000025f;
        if (a > maxAlpha)
            a = maxAlpha;
        _renderer.enabled = a > .005f;

        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetColor("_Color", new Color(mat.color.r, mat.color.g, mat.color.b, a));
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }
}
