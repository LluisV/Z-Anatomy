using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{

    public float rotationRate = 3.0f;

    void Update()
    {
        // get the user touch inpun
        foreach (Touch touch in Input.touches)
        {
            Debug.Log("Touching at: " + touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("Touch phase began at: " + touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Debug.Log("Touch phase Moved");
                transform.Rotate(touch.deltaPosition.y * rotationRate,
                                 -touch.deltaPosition.x * rotationRate, 0, Space.World);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Debug.Log("Touch phase Ended");
            }
        }
    }
}

