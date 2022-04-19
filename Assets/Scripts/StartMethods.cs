using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMethods : MonoBehaviour
{
    Label label;
    NameAndDescription nameScript;
    IsVisible isVisibleScript;
    private void Awake()
    {
        label = GetComponent<Label>();
        nameScript = GetComponent<NameAndDescription>();
        isVisibleScript = GetComponent<IsVisible>();
    }

    private void Start()
    {
        /*label.originalName = nameScript.originalName;
        isVisibleScript.SetLabelVisibility();
        label.GetOriginPoint();
        nameScript.SetTranslatedName();
        label.Initialize();*/
    }
}
