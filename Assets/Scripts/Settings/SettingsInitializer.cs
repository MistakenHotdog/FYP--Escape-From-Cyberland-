using UnityEngine;

// Apply all saved settings at scene load, regardless of whether the Options panel is open.
// Attach this to an always-active GameObject in every scene (Main Menu AND Game scene).
public class SettingsInitializer : MonoBehaviour
{
    [Header("Post-Processing Volume GameObject (this scene)")]
    [Tooltip("Optional: drag the PostProcessLayer GameObject of this scene. If left empty, it is looked up by name.")]
    public GameObject postProcessingVolume;

    // Same keys used by GraphicsMenu and AudioSettings
    const string KEY_QUALITY = "quality";
    const string KEY_SHADOWS = "shadows";
    const string KEY_RENDERSCALE = "renderscale";
    const string KEY_POST = "postprocessing";
    const string KEY_MUSIC = "music";

    void Awake()
    {
        ApplyGraphics();
        ApplyAudio();
    }

    void ApplyGraphics()
    {
        // Quality level (0=Low, 1=Medium, 2=High)
        int q = PlayerPrefs.GetInt(KEY_QUALITY, 2);
        QualitySettings.SetQualityLevel(q);

        // Shadows
        bool shadowsOn = PlayerPrefs.GetInt(KEY_SHADOWS, 1) == 1;
        QualitySettings.shadows = shadowsOn ? ShadowQuality.All : ShadowQuality.Disable;

        // Render scale
        float scale = PlayerPrefs.GetFloat(KEY_RENDERSCALE, 1f);
        ScalableBufferManager.ResizeBuffers(scale, scale);

        // Post-processing volume
        bool ppOn = PlayerPrefs.GetInt(KEY_POST, 1) == 1;
        GameObject pp = postProcessingVolume != null ? postProcessingVolume : GameObject.Find("PostProcessLayer");
        if (pp != null)
            pp.SetActive(ppOn);

        Debug.Log($"[SettingsInitializer] Applied graphics — quality={q}, shadows={shadowsOn}, renderScale={scale:F2}, postProcessing={ppOn}");
    }

    void ApplyAudio()
    {
        bool musicOn = PlayerPrefs.GetInt(KEY_MUSIC, 1) == 1;
        AudioListener.volume = musicOn ? 1f : 0f;
        Debug.Log($"[SettingsInitializer] Applied audio — music={musicOn}");
    }
}
