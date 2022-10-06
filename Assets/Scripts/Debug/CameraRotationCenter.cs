using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationCenter : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.position = CameraController.instance.pivot;
        float scale = CameraController.instance.distance / 35;
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
