using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCenter : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.position = CameraController.instance.cameraCenter.position;
    }
}
