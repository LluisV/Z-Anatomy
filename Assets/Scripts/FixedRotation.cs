using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedRotation : MonoBehaviour
{
    Quaternion initialRotation;
    Vector3 initialScale;
    private void Awake()
    {
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
       /* transform.rotation = initialRotation;
        transform.localScale = transform.parent.lossyScale.x * initialScale;*/
    }
}
