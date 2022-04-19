using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorsIconOrientation : MonoBehaviour
{
    public Transform cubeCenter;
    public Transform target;
    private Quaternion initialRotation;
    private Camera cam;
    public CubeFace cubeFace;
    public CrossPlanesCube planesScript;
    private CubeBehaviour cubeScript;

    private void Awake()
    {
        initialRotation = transform.rotation;
        cubeScript = GetComponentInParent<CubeBehaviour>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = cam.transform.rotation * initialRotation;
    }

    private void OnMouseDown()
    {
        //We block the main raycast so we don't click other things
        RaycastObject.instance.raycastBlocked = true;
    }

    private void OnMouseUpAsButton()
    {
        planesScript.SetPlane(cubeFace);
        cubeScript.SetCameraRotation(cubeFace);
        RaycastObject.instance.raycastBlocked = false;
    }
}
