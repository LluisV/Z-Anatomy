using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System.Reflection;

public class FixZAnatomyAndroid
{
    [MenuItem("Z-Anatomy/Fix Android Glitches")]
    public static void FixGlitches()
    {
        Debug.Log("[FixZAnatomy] Starting fixes...");

        string[] scenes = new[] { "Assets/Scenes/SkeletonScene.unity", "Assets/Scenes/SampleScene.unity" };
        foreach (var scenePath in scenes)
        {
            if (System.IO.File.Exists(scenePath))
            {
                var scene = EditorSceneManager.OpenScene(scenePath);
                var canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                int fixedCount = 0;
                foreach (var canvas in canvases)
                {
                    var current = canvas.additionalShaderChannels;
                    var required = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
                    if ((current & required) != required)
                    {
                        canvas.additionalShaderChannels = current | required;
                        EditorUtility.SetDirty(canvas);
                        fixedCount++;
                    }
                }
                
                var cameras = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach(var cam in cameras) {
                    var ucam = cam.GetComponent<UniversalAdditionalCameraData>();
                    if (ucam != null && ucam.renderPostProcessing) {
                        ucam.renderPostProcessing = false;
                        EditorUtility.SetDirty(ucam);
                    }
                    if (cam.allowMSAA) {
                        cam.allowMSAA = false;
                        EditorUtility.SetDirty(cam);
                    }
                }

                if (fixedCount > 0)
                {
                    EditorSceneManager.SaveScene(scene);
                    Debug.Log($"[FixZAnatomy] Fixed {fixedCount} Canvases in {scenePath}");
                }
            }
        }

        string[] renderers = AssetDatabase.FindAssets("t:ScriptableRendererData");
        foreach (var guid in renderers)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var renderer = AssetDatabase.LoadAssetAtPath<ScriptableRendererData>(path);
            if (renderer != null)
            {
                bool changed = false;
                foreach (var feature in renderer.rendererFeatures)
                {
                    if (feature != null && feature.isActive)
                    {
                        if (feature.name.Contains("Ambient Occlusion") || feature.name.Contains("SSAO"))
                        {
                            feature.SetActive(false);
                            changed = true;
                            Debug.Log($"[FixZAnatomy] Disabled {feature.name} in {path}");
                        }
                    }
                }
                if (changed)
                {
                    EditorUtility.SetDirty(renderer);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        Debug.Log("[FixZAnatomy] Finished applying Android fixes.");
    }
}
