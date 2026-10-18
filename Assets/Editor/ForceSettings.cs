using UnityEngine;
using UnityEditor;

public class ForceSettings
{
    public static void Apply()
    {
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });
        PlayerSettings.stripUnusedMeshComponents = false;
        AssetDatabase.SaveAssets();
        Debug.Log("[ForceSettings] Successfully forced GLES3 and saved Project Settings!");
    }
}
