using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    [HideInInspector]
    public static GlobalVariables instance;
    public bool mobile;
    public static Vector2 actualScreenSize;
    public static ScreenOrientation screenOrientation;

    private void Awake()
    {
        instance = this;
        screenOrientation = Screen.orientation;
        actualScreenSize = new Vector2(Screen.width, Screen.height);
        mobile = SystemInfo.deviceType == DeviceType.Handheld;
    }

}
