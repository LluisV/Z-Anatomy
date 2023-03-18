
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.InputSystem;
using TMPro;
#if UNITY_STANDALONE_WIN
using System.Runtime.InteropServices;
#endif

public class MinimumWindowSize : MonoBehaviour
{
    // The minimum width and height for the window
    public int minWidth = 800;
    public int minHeight = 600;
    private float lastWidth;
    public RectTransform canvasRT;

    private void Start()
    {
        lastWidth = canvasRT.GetWidth();
    }

#if UNITY_STANDALONE_WIN
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern IntPtr FindWindow(string className, string windowName);
    public static IEnumerator SetWindowPosition(int x, int y)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        SetWindowPos(FindWindow(null, Application.productName), 0, x, y, 0, 0, 5);
    }
    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);

#endif

    IEnumerator resizeCoroutine;

    public void OnResize()
    {

        float delta = canvasRT.GetWidth() - lastWidth;
        if(PanelsManagement.instance != null)
            PanelsManagement.instance.CollpasePanels(delta);
        lastWidth = canvasRT.GetWidth();
#if !UNITY_WEBGL
        if (resizeCoroutine != null)
            StopCoroutine(resizeCoroutine);
        resizeCoroutine = ClampWindow();
        StartCoroutine(resizeCoroutine);
#endif
    }

    private IEnumerator ClampWindow()
    {
        Vector2Int screenPosition = Screen.mainWindowPosition;
        DisplayInfo screenDisplay = Screen.mainWindowDisplayInfo;

        int width = Screen.width;
        int height = Screen.height;
#if UNITY_STANDALONE_WIN
        //Wait while the LMB is pressed
        yield return new WaitWhile(() => (GetAsyncKeyState(0x01) & 0x8000) != 0);
#else
        yield return null;
#endif
        HierarchyBar.Instance.Set();
        if (width < minWidth)
                Screen.SetResolution(minWidth, height, false);
        if (height < minHeight)
                Screen.SetResolution(width, minHeight, false);


#if UNITY_STANDALONE_WIN
        Screen.MoveMainWindowTo(screenDisplay, screenPosition);
#endif

    }

}
