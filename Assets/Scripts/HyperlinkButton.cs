using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperlinkButton : MonoBehaviour
{
    public string link;
    public void OpenLink()
    {
        Application.OpenURL(link);
    }
}
