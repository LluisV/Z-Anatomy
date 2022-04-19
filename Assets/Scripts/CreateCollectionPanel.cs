using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateCollectionPanel : MonoBehaviour
{
    public GameObject[] buttons;
    public Image backgroundImage;

    public void DeleteOutlines()
    {
        foreach (var item in buttons)
        {
            item.GetComponent<Outline>().enabled = false;
        }
    }

    public void SetColor(Color color)
    {
        backgroundImage.color = color;
    }
}
