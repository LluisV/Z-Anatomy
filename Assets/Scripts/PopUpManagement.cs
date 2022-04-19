using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManagement : MonoBehaviour
{
    public static PopUpManagement instance;
    public GameObject canvas;
    public GameObject popUpPrefab;

    private void Awake()
    {
        instance = this;
    }

    public void Show(string message)
    {
        PopUpMessage popUp = Instantiate(popUpPrefab, canvas.transform).GetComponent<PopUpMessage>();
        popUp.Show(message, 3);
    }

    public void Show(string message, float length)
    {
        PopUpMessage popUp = Instantiate(popUpPrefab, canvas.transform).GetComponent<PopUpMessage>();
        popUp.Show(message, length);
    }
}
