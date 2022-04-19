using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static bool draggingUI;

    public void DragStart()
    {
        draggingUI = true;
    }

    public void DragEnd()
    {
        draggingUI = false;

    }
}
