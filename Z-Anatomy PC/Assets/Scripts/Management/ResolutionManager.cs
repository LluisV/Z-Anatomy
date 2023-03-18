using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ResolutionManager : MonoBehaviour
{
    static public ResolutionManager Instance;

    // Fixed aspect ratio parameters
    static public bool FixedAspectRatio = true;
    static public float TargetAspectRatio = 16 / 9f;

    // Windowed aspect ratio when FixedAspectRatio is false
    static public float WindowedAspectRatio = 16 / 9f;

    // List of horizontal resolutions to include
    int[] resolutions = new int[] { 1024, 1152, 1280, 1408, 1536, 1664, 1792, 1920, 2432, 2560 };

    public Resolution DisplayResolution;
    public List<Vector2> WindowedResolutions, FullscreenResolutions;

    [HideInInspector]
    public int currWindowedRes, currFullscreenRes;

    public Image maximizeIcon;
    public Sprite restoreSprite;
    public Sprite maximizeSprite;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            StartCoroutine(StartRoutine());
    }

    private void printResolution()
    {
        Debug.Log("Current res: " + Screen.currentResolution.width + "x" + Screen.currentResolution.height);
    }

    private IEnumerator StartRoutine()
    {
        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            DisplayResolution = Screen.currentResolution;
        }
        else
        {
            if (Screen.fullScreen)
            {
                Resolution r = Screen.currentResolution;
                Screen.fullScreen = false;

                yield return null;
                yield return null;

                DisplayResolution = Screen.currentResolution;

                Screen.SetResolution(r.width, r.height, true);

                yield return null;
            }
            else
            {
                DisplayResolution = Screen.currentResolution;
            }
        }

        TargetAspectRatio = WindowedAspectRatio = (float)DisplayResolution.width / (float)DisplayResolution.height;


        InitResolutions();
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
                BorderlessWindow.SetFramelessWindow(true);
#endif
    }

    private void InitResolutions()
    {
        float screenAspect = (float)DisplayResolution.width / DisplayResolution.height;

        WindowedResolutions = new List<Vector2>();
        FullscreenResolutions = new List<Vector2>();

        foreach (int w in resolutions)
        {
            if (w < DisplayResolution.width)
            {
                // Adding resolution only if it's 5% smaller than the screen
                if (w < DisplayResolution.width * 0.95f)
                {
                    Vector2 windowedResolution = new Vector2(w, Mathf.Round(w / (FixedAspectRatio ? TargetAspectRatio : WindowedAspectRatio)));
                    if (windowedResolution.y < DisplayResolution.height * 0.95f)
                        WindowedResolutions.Add(windowedResolution);

                    FullscreenResolutions.Add(new Vector2(w, Mathf.Round(w / screenAspect)));
                }
            }
        }

        // Adding fullscreen native resolution
        FullscreenResolutions.Add(new Vector2(DisplayResolution.width, DisplayResolution.height));

        // Adding half fullscreen native resolution
        Vector2 halfNative = new Vector2(DisplayResolution.width * 0.5f, DisplayResolution.height * 0.5f);
        if (halfNative.x > resolutions[0] && FullscreenResolutions.IndexOf(halfNative) == -1)
            FullscreenResolutions.Add(halfNative);

        FullscreenResolutions = FullscreenResolutions.OrderBy(resolution => resolution.x).ToList();

        bool found = false;
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        currWindowedRes = WindowedResolutions.Count - 1;
        currFullscreenRes = FullscreenResolutions.Count - 1;

        if (fullscreen)
        {
            for (int i = 0; i < FullscreenResolutions.Count; i++)
            {
                if (FullscreenResolutions[i].x == Screen.width && FullscreenResolutions[i].y == Screen.height)
                {
                    currFullscreenRes = i;
                    found = true;
                    break;
                }
            }

            if (!found)
                SetResolution(FullscreenResolutions.Count - 1, true);
        }
        else
        {
            for (int i = 0; i < WindowedResolutions.Count; i++)
            {
                if (WindowedResolutions[i].x == Screen.width && WindowedResolutions[i].y == Screen.height)
                {
                    found = true;
                    currWindowedRes = i;
                    break;
                }
            }

            if (!found)
                SetResolution(WindowedResolutions.Count - 1, false);
        }
    }

    public void SetResolution(int index, bool fullscreen)
    {
        if (FullscreenResolutions.Count == 0)
            return;

        Vector2 r = new Vector2();
        if (fullscreen)
        {
            currFullscreenRes = index;
            r = FullscreenResolutions[currFullscreenRes];
        }
        else
        {
            currWindowedRes = index;
            r = WindowedResolutions[currWindowedRes];
        }

        bool fullscreen2windowed = Screen.fullScreen & !fullscreen;

        Debug.Log("Setting resolution to " + (int)r.x + "x" + (int)r.y);
        Screen.SetResolution((int)r.x, (int)r.y, fullscreen);

        // On OSX the application will pass from fullscreen to windowed with an animated transition of a couple of seconds.
        // After this transition, the first time you exit fullscreen you have to call SetResolution again to ensure that the window is resized correctly.
        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            // Ensure that there is no SetResolutionAfterResize coroutine running and waiting for screen size changes
            StopAllCoroutines();

            // Resize the window again after the end of the resize transition
            if (fullscreen2windowed) StartCoroutine(SetResolutionAfterResize(r));
        }

        StartCoroutine(WaitToScreenChange());

        IEnumerator WaitToScreenChange()
        {
            yield return null;
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN   // Dont do this while on Unity Editor!
                    BorderlessWindow.SetFramelessWindow(true);
#endif
            //BorderlessWindow.MoveWindowPos(Vector2Int.zero, Screen.width - borderSize.x, Screen.height - borderSize.y);
            BorderlessWindow.MoveWindowPos(Vector2Int.zero, Screen.width, Screen.height);
        }

    }

    private IEnumerator SetResolutionAfterResize(Vector2 r)
    {
        int maxTime = 5; // Max wait for the end of the resize transition
        float time = Time.time;

        // Skipping a couple of frames during which the screen size will change
        yield return null;
        yield return null;

        int lastW = Screen.width;
        int lastH = Screen.height;

        // Waiting for another screen size change at the end of the transition animation
        while (Time.time - time < maxTime)
        {
            if (lastW != Screen.width || lastH != Screen.height)
            {
                Debug.Log("Resize! " + Screen.width + "x" + Screen.height);

                Screen.SetResolution((int)r.x, (int)r.y, Screen.fullScreen);
                yield break;
            }

            yield return null;
        }

        Debug.Log("End waiting");
    }

    public void ToggleFullscreen(bool fromSettings)
    {
        if (!fromSettings)
            Settings.Instance.fullscreenToggle.SetIsOnWithoutNotify(!Settings.Instance.fullscreenToggle.isOn);

        if (Settings.Instance.fullscreenToggle.isOn)
            maximizeIcon.sprite = restoreSprite;
        else
            maximizeIcon.sprite = maximizeSprite;

        //SetResolution(
        //    Settings.Instance.fullscreenToggle.isOn ? currFullscreenRes : currWindowedRes,
        //    Settings.Instance.fullscreenToggle.isOn);

        PlayerPrefs.SetInt("Fullscreen", Settings.Instance.fullscreenToggle.isOn ? 1 : 0);
        if (!Settings.Instance.fullscreenToggle.isOn)
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.Windowed);
        else
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
    }
}