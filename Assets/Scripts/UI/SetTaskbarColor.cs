using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTaskbarColor : MonoBehaviour
{
    Image img;
    private void Awake()
    {
        img = GetComponent<Image>();
        img.color = GlobalVariables.TaskBarColor;
    }
}
