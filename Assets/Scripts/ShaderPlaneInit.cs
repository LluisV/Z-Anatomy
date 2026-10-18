using UnityEngine;

/// <summary>
/// Initializes global shader plane properties on startup so the cross-section
/// plane does NOT clip any models by default.
/// 
/// The ClippableShader uses: dot(fragWorldPos - _PlanePosition, _PlaneNormal)
/// and alpha-clips fragments where the result < 0.5.
/// 
/// When _PlaneNormal = (0,0,0) (default), dot product = 0 < 0.5 → ALL fragments
/// are clipped → invisible models on Android.
/// 
/// Fix: set _PlaneNormal to (0,1,0) and _PlanePosition far BELOW the scene
/// so the dot product is always POSITIVE (> 0.5) for all visible fragments.
/// </summary>
[DefaultExecutionOrder(-1000)]
public class ShaderPlaneInit : MonoBehaviour
{
    // Position far below the world so the plane never clips anything
    private static readonly Vector4 SafePlanePosition = new Vector4(0f, -10000f, 0f, 0f);
    // Normal pointing up - standard orientation for "nothing is clipped above -10000"
    private static readonly Vector4 SafePlaneNormal = new Vector4(0f, 1f, 0f, 0f);

    void Awake()
    {
        ApplySafePlane();
    }

    void Start()
    {
        // Re-apply in Start in case other scripts reset it
        ApplySafePlane();
    }

    private void ApplySafePlane()
    {
        Shader.SetGlobalVector("_PlanePosition", SafePlanePosition);
        Shader.SetGlobalVector("_PlaneNormal", SafePlaneNormal);
        Debug.Log("[ShaderPlaneInit] Initialized cross-section plane to safe defaults (no clipping).");
    }
}
