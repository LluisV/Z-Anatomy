// =============================================================================
//  AndroidBuildScript.cs
//  Z-Anatomy — Unity Editor Build Automation Script
//
//  Provides:
//    1. Menu item: Z-Anatomy > Build Android APK
//    2. Static method for batch-mode:
//         -executeMethod AndroidBuildScript.BuildAPK
//
//  This file belongs in: Assets/Editor/AndroidBuildScript.cs
//  It is EDITOR-ONLY and is NOT included in the Android build itself.
// =============================================================================

#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class AndroidBuildScript
{
    // ── Configuration ────────────────────────────────────────────────────────
    private const string AppName         = "ZAnatomy";
    private const string PackageName     = "com.zanatomy.android";
    private const string OutputDir       = "Build/Android";
    private const string ApkName         = "ZAnatomy.apk";
    private const int    BundleVersion   = 1;
    private const string AppVersion      = "1.0.0";
    private const AndroidSdkVersions MinSdk    = AndroidSdkVersions.AndroidApiLevel26;
    private const AndroidSdkVersions TargetSdk = AndroidSdkVersions.AndroidApiLevel34;
    // ─────────────────────────────────────────────────────────────────────────

    // ── Menu item (Unity Editor GUI) ─────────────────────────────────────────
    [MenuItem("Z-Anatomy/Build Android APK")]
    public static void BuildAPKFromMenu()
    {
        BuildAPK();
    }

    [MenuItem("Z-Anatomy/Build Android AAB (Play Store)")]
    public static void BuildAABFromMenu()
    {
        BuildAAB();
    }
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Entry point for Unity batch-mode builds:
    ///   unity.exe -batchmode -executeMethod AndroidBuildScript.BuildAPK -quit
    /// </summary>
    public static void BuildAPK()
    {
        ConfigureAndroidSettings();

        PlayerSettings.Android.splitApplicationBinary = false;

        var outputPath = Path.Combine(OutputDir, ApkName);
        Directory.CreateDirectory(OutputDir);

        Debug.Log($"[ZAnatomy Build] Starting APK build → {outputPath}");

        var report = BuildPipeline.BuildPlayer(GetBuildPlayerOptions(outputPath, BuildOptions.None));
        HandleBuildReport(report);
    }

    /// <summary>
    /// Builds an Android App Bundle (.aab) suitable for Google Play submission.
    ///   unity.exe -batchmode -executeMethod AndroidBuildScript.BuildAAB -quit
    /// </summary>
    public static void BuildAAB()
    {
        ConfigureAndroidSettings();

        EditorUserBuildSettings.buildAppBundle = true;
        var aabName    = ApkName.Replace(".apk", ".aab");
        var outputPath = Path.Combine(OutputDir, aabName);
        Directory.CreateDirectory(OutputDir);

        Debug.Log($"[ZAnatomy Build] Starting AAB build → {outputPath}");

        var report = BuildPipeline.BuildPlayer(GetBuildPlayerOptions(outputPath, BuildOptions.None));
        HandleBuildReport(report);

        // Reset
        EditorUserBuildSettings.buildAppBundle = false;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void ConfigureAndroidSettings()
    {
        Debug.Log("[ZAnatomy Build] Configuring Android Player Settings...");

        // Identity
        PlayerSettings.applicationIdentifier        = PackageName;
        PlayerSettings.productName                  = "Z-Anatomy";
        PlayerSettings.companyName                  = "Z-Anatomy Open Source";
        PlayerSettings.bundleVersion                = AppVersion;
        PlayerSettings.Android.bundleVersionCode    = BundleVersion;

        // API levels
        PlayerSettings.Android.minSdkVersion        = MinSdk;
        PlayerSettings.Android.targetSdkVersion     = TargetSdk;

        // Architecture: ARM64 + ARMv7 for broad device support
        PlayerSettings.Android.targetArchitectures  =
            AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

        // Screen orientation: landscape (better for anatomy viewing)
        PlayerSettings.defaultInterfaceOrientation  = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToLandscapeLeft  = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.allowedAutorotateToPortrait        = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

        // Graphics
        PlayerSettings.gpuSkinning                  = true;
        PlayerSettings.stripEngineCode              = false; 
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false); // CRITICAL: Must disable auto APIs first
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 }); 
        
        // Disable Mesh Component Stripping & Vertex Compression (prevents invisible meshes and destroyed text UVs!)
        PlayerSettings.stripUnusedMeshComponents = false;

        // Android-specific
        PlayerSettings.Android.useCustomKeystore         = false; // Sign with default debug key
        PlayerSettings.Android.forceInternetPermission  = false; // manifest handles this
        PlayerSettings.Android.forceSDCardPermission    = false;
        PlayerSettings.Android.blitType                 = AndroidBlitType.Auto;

        // Point to custom OpenJDK & Android SDK
        string androidDir = @"C:\Android";
        string jdkPath = System.IO.Path.Combine(androidDir, "OpenJDK");
        string sdkPath = System.IO.Path.Combine(androidDir, "SDK");
        string ndkPath = System.IO.Path.Combine(androidDir, "NDK");

        if (System.IO.Directory.Exists(jdkPath))
        {
            EditorPrefs.SetBool("JdkUseEmbedded", false);
            EditorPrefs.SetString("JdkPath", jdkPath);
            UnityEditor.Android.AndroidExternalToolsSettings.jdkRootPath = jdkPath;
            Debug.Log($"[ZAnatomy Build] Set JDK Path: {jdkPath}");
        }

        if (System.IO.Directory.Exists(sdkPath))
        {
            EditorPrefs.SetBool("SdkUseEmbedded", false);
            EditorPrefs.SetString("AndroidSdkRoot", sdkPath);
            UnityEditor.Android.AndroidExternalToolsSettings.sdkRootPath = sdkPath;
            Debug.Log($"[ZAnatomy Build] Set Android SDK Path: {sdkPath}");
        }

        if (System.IO.Directory.Exists(ndkPath))
        {
            EditorPrefs.SetBool("NdkUseEmbedded", false);
            EditorPrefs.SetString("AndroidNdkRoot", ndkPath);
            UnityEditor.Android.AndroidExternalToolsSettings.ndkRootPath = ndkPath;
            Debug.Log($"[ZAnatomy Build] Set Android NDK Path: {ndkPath}");
        }

        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
        Debug.Log("[ZAnatomy Build] Using IL2CPP backend (ARM64+ARMv7).");

        // Use .NET Standard 2.1
        PlayerSettings.SetApiCompatibilityLevel(
            UnityEditor.Build.NamedBuildTarget.Android, ApiCompatibilityLevel.NET_Standard_2_0);

        Debug.Log("[ZAnatomy Build] Android settings configured.");
    }

    private static BuildPlayerOptions GetBuildPlayerOptions(string locationPathName, BuildOptions options)
    {
        // Collect all scenes from Build Settings
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            // Fallback: auto-discover all scenes in the project
            scenes = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();
            Debug.LogWarning($"[ZAnatomy Build] No scenes in Build Settings. Auto-discovered {scenes.Length} scenes.");
        }

        Debug.Log($"[ZAnatomy Build] Building {scenes.Length} scene(s):");
        foreach (var s in scenes) Debug.Log($"  - {s}");

        return new BuildPlayerOptions
        {
            scenes            = scenes,
            locationPathName  = locationPathName,
            target            = BuildTarget.Android,
            options           = options
        };
    }

    private static void HandleBuildReport(BuildReport report)
    {
        var summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            var sizeKB = summary.totalSize / 1024;
            Debug.Log($"[ZAnatomy Build] ✓ BUILD SUCCEEDED");
            Debug.Log($"[ZAnatomy Build]   Output : {summary.outputPath}");
            Debug.Log($"[ZAnatomy Build]   Size   : {sizeKB:N0} KB");
            Debug.Log($"[ZAnatomy Build]   Time   : {summary.totalTime.TotalSeconds:F1}s");
        }
        else
        {
            Debug.LogError($"[ZAnatomy Build] ✗ BUILD FAILED: {summary.result}");
            Debug.LogError($"[ZAnatomy Build]   Errors  : {summary.totalErrors}");
            Debug.LogError($"[ZAnatomy Build]   Warnings: {summary.totalWarnings}");
            // Exit with error code so CI systems detect failure
            EditorApplication.Exit(1);
        }
    }
}

#endif // UNITY_EDITOR
