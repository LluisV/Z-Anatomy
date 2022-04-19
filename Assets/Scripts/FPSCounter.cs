using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI FPStext;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GetFPS", 0.25f, 0.25f);
    }

    private void GetFPS()
    {
        FPStext.text = "FPS: " + (int)(1f / Time.unscaledDeltaTime);
    }
}
