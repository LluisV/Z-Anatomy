// =============================================================================
//  MobilePerformanceManager.cs
//  Z-Anatomy — Android Performance Tier Manager
//
//  Automatically adjusts rendering quality based on the device tier.
//  Works alongside the existing "Adaptive Performance" package already
//  in the Z-Anatomy Mobile-Version branch.
//
//  This script is ADDITIVE — it does NOT modify any existing script.
//  Attach it to a persistent GameObject in the main scene.
// =============================================================================

using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class MobilePerformanceManager : MonoBehaviour
{
    // ── Configuration ────────────────────────────────────────────────────────
    [Header("Frame Rate Targets")]
    [Tooltip("Frame rate cap for high-end devices (e.g. flagship phones).")]
    [SerializeField] private int highTierTargetFPS = 60;

    [Tooltip("Frame rate cap for mid-range devices.")]
    [SerializeField] private int midTierTargetFPS  = 45;

    [Tooltip("Frame rate cap for low-end devices.")]
    [SerializeField] private int lowTierTargetFPS  = 30;

    [Header("Quality Settings")]
    [Tooltip("Quality level index for high-end devices (see Project Settings > Quality).")]
    [SerializeField] private int highTierQualityLevel = 5;

    [Tooltip("Quality level index for mid-range devices.")]
    [SerializeField] private int midTierQualityLevel  = 3;

    [Tooltip("Quality level index for low-end devices.")]
    [SerializeField] private int lowTierQualityLevel  = 1;

    [Header("Memory Thresholds (MB)")]
    [Tooltip("Devices with >= this RAM are classified as high-end.")]
    [SerializeField] private int highTierRAM = 6144;  // 6 GB

    [Tooltip("Devices with >= this RAM are classified as mid-range.")]
    [SerializeField] private int midTierRAM  = 3072;  // 3 GB
    // ─────────────────────────────────────────────────────────────────────────

    public enum DeviceTier { Low, Mid, High }
    public static DeviceTier CurrentTier { get; private set; } = DeviceTier.Mid;

    // ── Events ────────────────────────────────────────────────────────────────
    public static System.Action<DeviceTier> OnTierDetected;
    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        DetectDeviceTier();
        ApplyPerformanceSettings();
    }

    private void DetectDeviceTier()
    {
        var systemMemoryMB = SystemInfo.systemMemorySize;
        var gpu            = SystemInfo.graphicsDeviceName ?? "";

        Debug.Log($"[MobilePerformance] Device: {SystemInfo.deviceModel}");
        Debug.Log($"[MobilePerformance] GPU   : {gpu}");
        Debug.Log($"[MobilePerformance] RAM   : {systemMemoryMB} MB");
        Debug.Log($"[MobilePerformance] OS    : {SystemInfo.operatingSystem}");

        // Primary classification by RAM
        if (systemMemoryMB >= highTierRAM)
            CurrentTier = DeviceTier.High;
        else if (systemMemoryMB >= midTierRAM)
            CurrentTier = DeviceTier.Mid;
        else
            CurrentTier = DeviceTier.Low;

        // GPU-based override: Adreno 6xx / 7xx = high-end, Mali G71 and below = low
        if (IsHighEndGPU(gpu) && CurrentTier == DeviceTier.Mid)
            CurrentTier = DeviceTier.High;
        else if (IsLowEndGPU(gpu) && CurrentTier != DeviceTier.Low)
            CurrentTier = DeviceTier.Low;

        Debug.Log($"[MobilePerformance] Classified as: {CurrentTier} tier");
        OnTierDetected?.Invoke(CurrentTier);
    }

    private void ApplyPerformanceSettings()
    {
        int targetFPS;
        int qualityLevel;

        switch (CurrentTier)
        {
            case DeviceTier.High:
                targetFPS    = highTierTargetFPS;
                qualityLevel = highTierQualityLevel;
                break;
            case DeviceTier.Low:
                targetFPS    = lowTierTargetFPS;
                qualityLevel = lowTierQualityLevel;
                break;
            default: // Mid
                targetFPS    = midTierTargetFPS;
                qualityLevel = midTierQualityLevel;
                break;
        }

        // Cap quality level to valid range
        qualityLevel = Mathf.Clamp(
            qualityLevel, 0, QualitySettings.names.Length - 1);

        Application.targetFrameRate = targetFPS;
        QualitySettings.SetQualityLevel(qualityLevel, applyExpensiveChanges: true);

        // GPU skinning: enable on mid+, disable on low (save CPU/GPU bandwidth)
        QualitySettings.skinWeights = CurrentTier == DeviceTier.Low
            ? SkinWeights.TwoBones
            : SkinWeights.FourBones;

        // Reduce shadow quality on low-end for anatomy performance
        if (CurrentTier == DeviceTier.Low)
        {
            QualitySettings.shadows      = ShadowQuality.Disable;
            QualitySettings.lodBias      = 0.5f;
        }
        else if (CurrentTier == DeviceTier.Mid)
        {
            QualitySettings.shadows      = ShadowQuality.HardOnly;
            QualitySettings.lodBias      = 1.0f;
        }
        else
        {
            QualitySettings.shadows      = ShadowQuality.All;
            QualitySettings.lodBias      = 1.5f;
        }

        Debug.Log($"[MobilePerformance] Applied: FPS={targetFPS}, Quality={QualitySettings.names[qualityLevel]}, Shadows={QualitySettings.shadows}");
    }

    // ── GPU Classification Helpers ────────────────────────────────────────────

    private static bool IsHighEndGPU(string gpuName)
    {
        if (string.IsNullOrEmpty(gpuName)) return false;
        var g = gpuName.ToLowerInvariant();
        return g.Contains("adreno 6")   // Snapdragon 855+
            || g.Contains("adreno 7")   // Snapdragon 8 Gen 1+
            || g.Contains("adreno 8")   // Snapdragon 8 Gen 3+
            || g.Contains("mali-g76")
            || g.Contains("mali-g710")
            || g.Contains("mali-g78")
            || g.Contains("apple gpu") // iOS (in case of iOS builds)
            || g.Contains("xclipse");  // Samsung Exynos 2200+
    }

    private static bool IsLowEndGPU(string gpuName)
    {
        if (string.IsNullOrEmpty(gpuName)) return false;
        var g = gpuName.ToLowerInvariant();
        return g.Contains("adreno 3")
            || g.Contains("adreno 4")
            || g.Contains("mali-t")
            || g.Contains("mali-400")
            || g.Contains("mali-450")
            || g.Contains("powervr");
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Force a specific device tier (e.g., from a settings menu).</summary>
    public static void OverrideTier(DeviceTier tier)
    {
        CurrentTier = tier;
        if (Instance != null)
        {
            Instance.ApplyPerformanceSettings();
        }
    }

    // Singleton accessor (lazy, no DontDestroyOnLoad needed as Awake handles it)
    private static MobilePerformanceManager _instance;
    public static MobilePerformanceManager Instance => _instance;

    private void OnEnable()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void OnDisable()
    {
        if (_instance == this) _instance = null;
    }
}
