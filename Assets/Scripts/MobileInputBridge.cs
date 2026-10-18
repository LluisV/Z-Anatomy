// =============================================================================
//  MobileInputBridge.cs
//  Z-Anatomy — Mobile Touch Input Handler
//
//  Bridges Unity's touch input system to work alongside the existing
//  Z-Anatomy mouse/keyboard input system. This script is ADDITIVE —
//  it does NOT modify any existing script. Attach it to a persistent
//  GameObject in the main scene (e.g., a "MobileManager" empty object).
//
//  Gestures handled:
//    • Single finger drag  → Rotate model (maps to mouse drag)
//    • Two-finger pinch    → Zoom in/out (maps to scroll wheel)
//    • Two-finger drag     → Pan / translate the view
//    • Double tap          → Reset view / re-center
//    • Long press          → Context menu (label info)
// =============================================================================

using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInputBridge : MonoBehaviour
{
    // ── Configuration (tune in Inspector) ────────────────────────────────────
    [Header("Rotation")]
    [Tooltip("Sensitivity multiplier for single-finger rotate drag.")]
    [SerializeField] private float rotateSensitivity = 0.3f;

    [Header("Zoom (Pinch)")]
    [Tooltip("Sensitivity multiplier for two-finger pinch zoom.")]
    [SerializeField] private float zoomSensitivity = 0.02f;
    [SerializeField] private float minZoomDistance = 0.5f;
    [SerializeField] private float maxZoomDistance = 20f;

    [Header("Pan")]
    [Tooltip("Sensitivity multiplier for two-finger pan drag.")]
    [SerializeField] private float panSensitivity = 0.003f;

    [Header("Double Tap")]
    [Tooltip("Maximum time in seconds between taps to count as double-tap.")]
    [SerializeField] private float doubleTapThreshold = 0.3f;
    [SerializeField] private float doubleTapMaxDistance = 50f; // pixels

    [Header("Long Press")]
    [Tooltip("Duration (seconds) a finger must be held to trigger long press.")]
    [SerializeField] private float longPressDuration = 0.7f;
    // ─────────────────────────────────────────────────────────────────────────

    // ── Events (subscribe from other scripts without coupling) ───────────────
    public static System.Action<Vector2> OnRotateDelta;
    public static System.Action<float>   OnZoomDelta;
    public static System.Action<Vector2> OnPanDelta;
    public static System.Action          OnDoubleTap;
    public static System.Action<Vector2> OnLongPress;
    // ─────────────────────────────────────────────────────────────────────────

    // ── Private state ─────────────────────────────────────────────────────────
    private float  _lastTapTime;
    private Vector2 _lastTapPosition;
    private bool   _isLongPressTriggered;
    private float  _touchHoldTime;
    private Vector2 _touchStartPos;
    private float  _prevPinchDistance;
    private bool   _pinchInitialized;
    // ─────────────────────────────────────────────────────────────────────────

    private void OnEnable()
    {
        // Only activate on Android / iOS; on PC let the original mouse input work
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        Debug.Log("[MobileInputBridge] Mobile touch input enabled.");
#else
        Debug.Log("[MobileInputBridge] Running in Editor/PC mode — touch bridge passive.");
#endif
    }

    private void Update()
    {
        // Skip if pointer is over UI elements
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.touchCount == 0)
        {
            _pinchInitialized = false;
            return;
        }

        if (Input.touchCount == 1)
            HandleSingleTouch(Input.GetTouch(0));
        else if (Input.touchCount == 2)
            HandleTwoFingerGesture(Input.GetTouch(0), Input.GetTouch(1));
    }

    // ── Single Touch: Rotate + DoubleTap + LongPress ─────────────────────────
    private void HandleSingleTouch(Touch touch)
    {
        _pinchInitialized = false;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                _touchStartPos        = touch.position;
                _touchHoldTime        = 0f;
                _isLongPressTriggered = false;
                break;

            case TouchPhase.Stationary:
                _touchHoldTime += Time.deltaTime;
                if (!_isLongPressTriggered && _touchHoldTime >= longPressDuration)
                {
                    _isLongPressTriggered = true;
                    OnLongPress?.Invoke(touch.position);
                }
                break;

            case TouchPhase.Moved:
                if (!_isLongPressTriggered)
                {
                    // Map finger drag → rotate delta
                    var delta = touch.deltaPosition * rotateSensitivity;
                    OnRotateDelta?.Invoke(delta);
                }
                break;

            case TouchPhase.Ended:
                // Detect double tap
                if (!_isLongPressTriggered)
                {
                    var now      = Time.time;
                    var dist     = Vector2.Distance(touch.position, _lastTapPosition);
                    var timeDiff = now - _lastTapTime;

                    if (timeDiff <= doubleTapThreshold && dist <= doubleTapMaxDistance)
                    {
                        OnDoubleTap?.Invoke();
                        _lastTapTime = 0f; // reset
                    }
                    else
                    {
                        _lastTapTime     = now;
                        _lastTapPosition = touch.position;
                    }
                }
                break;
        }
    }

    // ── Two Finger: Pinch Zoom + Pan ─────────────────────────────────────────
    private void HandleTwoFingerGesture(Touch t0, Touch t1)
    {
        // ── Pinch zoom ──
        var currentDist = Vector2.Distance(t0.position, t1.position);

        if (!_pinchInitialized)
        {
            _prevPinchDistance = currentDist;
            _pinchInitialized  = true;
            return;
        }

        var pinchDelta = currentDist - _prevPinchDistance;
        _prevPinchDistance = currentDist;

        if (Mathf.Abs(pinchDelta) > 0.01f)
        {
            OnZoomDelta?.Invoke(-pinchDelta * zoomSensitivity);
        }

        // ── Two-finger pan ──
        var t0PrevPos  = t0.position - t0.deltaPosition;
        var t1PrevPos  = t1.position - t1.deltaPosition;
        var prevMid    = (t0PrevPos + t1PrevPos) * 0.5f;
        var currMid    = (t0.position + t1.position) * 0.5f;
        var panDelta   = (currMid - prevMid) * panSensitivity;

        if (panDelta.sqrMagnitude > 0.00001f)
        {
            OnPanDelta?.Invoke(panDelta);
        }
    }
}
