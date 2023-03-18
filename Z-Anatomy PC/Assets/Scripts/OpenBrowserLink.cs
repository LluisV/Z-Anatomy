using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBrowserLink : MonoBehaviour
{
    public string link;

    public void Open()
    {
        Application.OpenURL(link);
    }
}
