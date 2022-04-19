using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpPanel : MonoBehaviour
{
    public OpenHelpItem[] items;

    public void CollapseItems()
    {
        foreach (OpenHelpItem item in items)
        {
            if(item.open)
                item.Close();
        }
    }
}
