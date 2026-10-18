using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ANDROID FIX: TextMeshPro SDF shaders require TexCoord1 (UV1) to render correctly.
/// Without it, every character renders as a solid white right-angled triangle.
///
/// Unity's Canvas strips UV1 by default unless m_AdditionalShaderChannelsFlag includes it.
/// This script patches ALL Canvases — including dynamically created ones — at runtime
/// by hooking into Canvas.willRenderCanvases.
/// 
/// Required flags:
///   TexCoord1 = 8   (UV1 for TMP SDF)
///   Normal    = 1
///   Tangent   = 16
///   Combined  = 25
/// </summary>
[DefaultExecutionOrder(-2000)]
public class CanvasShaderChannelFix : MonoBehaviour
{
    private const AdditionalCanvasShaderChannels RequiredChannels =
        AdditionalCanvasShaderChannels.TexCoord1 |
        AdditionalCanvasShaderChannels.Normal |
        AdditionalCanvasShaderChannels.Tangent;

    void Awake()
    {
        // Fix all canvases already in the scene
        FixAllCanvases();
        // Hook into the render event to catch any new canvases created later
        Canvas.willRenderCanvases += OnWillRenderCanvases;
        Debug.Log("[CanvasShaderChannelFix] Initialized — patching all current and future Canvases.");
    }

    void OnDestroy()
    {
        Canvas.willRenderCanvases -= OnWillRenderCanvases;
    }

    private float lastFixTime = -1f;

    private void OnWillRenderCanvases()
    {
        // Avoid heavy search every frame - check at most every 0.5s
        if (Time.unscaledTime - lastFixTime < 0.5f) return;
        lastFixTime = Time.unscaledTime;
        FixAllCanvases();
    }

    private static void FixAllCanvases()
    {
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include);
        foreach (Canvas canvas in canvases)
        {
            if ((canvas.additionalShaderChannels & RequiredChannels) != RequiredChannels)
            {
                canvas.additionalShaderChannels |= RequiredChannels;
            }
        }
    }
}
