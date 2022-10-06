using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;

public class BorderlessWindow
{
    public static bool framed = true;

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll")]
    private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);
    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hwnd, out WinRect lpRect);

    private struct WinRect { public int left, top, right, bottom; }

    private const int GWL_STYLE = -16;

    private const int SW_MINIMIZE = 6;
    private const int SW_MAXIMIZE = 3;
    private const int SW_RESTORE = 9;

    private const uint WS_VISIBLE = 0x10000000;    
    private const uint WS_POPUP = 0x80000000;
    private const uint WS_BORDER = 0x00800000;
    private const uint WS_OVERLAPPED = 0x00000000;
    private const uint WS_CAPTION = 0x00C00000;
    private const uint WS_SYSMENU = 0x00080000;
    private const uint WS_THICKFRAME = 0x00040000; // WS_SIZEBOX
    private const uint WS_MINIMIZEBOX = 0x00020000;
    private const uint WS_MAXIMIZEBOX = 0x00010000;
    private const uint WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;

    // The enum flag for DwmSetWindowAttribute's second parameter, which tells the function what attribute to set.
    public enum DWMWINDOWATTRIBUTE
    {
        DWMWA_WINDOW_CORNER_PREFERENCE = 33
    }

    // The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
    // what value of the enum to set.
    public enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    // Import dwmapi.dll and define DwmSetWindowAttribute in C# corresponding to the native function.
    [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern long DwmSetWindowAttribute(IntPtr hwnd,
                                                     DWMWINDOWATTRIBUTE attribute,
                                                     ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
                                                     uint cbAttribute);


    // This attribute will make the method execute on game launch, after the Unity Logo Splash Screen.
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeOnLoad()
    {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN   // Dont do this while on Unity Editor!
            SetFramelessWindow(true);
            SetFramelessWindow(true);
#endif
    }

    public static void SetFramelessWindow(bool roundCorners)
    {
        var hwnd = GetActiveWindow();
        if(roundCorners)
        {
            var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
            var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
            DwmSetWindowAttribute(GetActiveWindow(), attribute, ref preference, sizeof(uint));
        }
        else
        {
            var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
            var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND;
            DwmSetWindowAttribute(GetActiveWindow(), attribute, ref preference, sizeof(uint));
        }
        SetWindowLong(hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE | WS_BORDER);
        framed = false;
    }

    public static void SetFramedWindow()
    {
        var hwnd = GetActiveWindow();
        SetWindowLong(hwnd, GWL_STYLE, WS_OVERLAPPEDWINDOW | WS_VISIBLE);
        framed = true;
    }

    public static void MinimizeWindow()
    {
        var hwnd = GetActiveWindow();
        ShowWindow(hwnd, SW_MINIMIZE);
    }

    public static void MaximizeWindow()
    {
        var hwnd = GetActiveWindow();
        ShowWindow(hwnd, SW_MAXIMIZE);
    }

    public static void RestoreWindow()
    {
        var hwnd = GetActiveWindow();
        ShowWindow(hwnd, SW_RESTORE);
    }

    public static void MoveWindowPos(Vector2 posDelta, int newWidth, int newHeight)
    {
        var hwnd = GetActiveWindow();

        var windowRect = GetWindowRect(hwnd, out WinRect winRect);

        var x = winRect.left + (int)posDelta.x;
        var y = winRect.top - (int)posDelta.y;
        MoveWindow(hwnd, x, y, newWidth, newHeight, false);
    }
}

