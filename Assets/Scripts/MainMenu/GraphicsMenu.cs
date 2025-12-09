using UnityEngine;
using UnityEngine.UI;

public class GraphicsMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public Dropdown qualityDropdown;     // 0 = Low, 1 = Medium, 2 = High
    public Toggle shadowsToggle;
    public Slider renderScaleSlider;     // 0.5 - 1.0
    public Toggle postProcessToggle;

    public Button applyButton;
    public Button resetButton;

    [Header("Post Processing Volume (Built-in)")]
    public GameObject postProcessingVolume; // assign your post process volume object or leave null

    [Header("Behavior")]
    public bool applyImmediately = true; // if true, selecting a preset applies settings immediately

    // Defaults (High)
    int defaultQuality = 2; // High
    bool defaultShadows = true;
    float defaultRenderScale = 1f;
    bool defaultPostProcessing = true;

    // PlayerPrefs keys
    const string KEY_QUALITY = "quality";
    const string KEY_SHADOWS = "shadows";
    const string KEY_RENDERSCALE = "renderscale";
    const string KEY_POST = "postprocessing";

    void Awake()
    {
        // Add listeners
        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);

        if (applyButton != null)
            applyButton.onClick.AddListener(ApplySettings);

        if (resetButton != null)
            resetButton.onClick.AddListener(ResetToDefaults);
    }

    void Start()
    {
        LoadSettings();
    }

    // Called when the Quality dropdown changes (0 = Low, 1 = Medium, 2 = High).
    // Sets the UI controls according to the selected preset.
    public void OnQualityChanged(int index)
    {
        switch (index)
        {
            case 0: // Low
                if (shadowsToggle != null) shadowsToggle.isOn = false;
                if (renderScaleSlider != null) renderScaleSlider.value = 0.6f;
                if (postProcessToggle != null) postProcessToggle.isOn = false;
                break;

            case 1: // Medium
                if (shadowsToggle != null) shadowsToggle.isOn = true;
                if (renderScaleSlider != null) renderScaleSlider.value = 0.8f;
                if (postProcessToggle != null) postProcessToggle.isOn = false;
                break;

            case 2: // High
            default:
                if (shadowsToggle != null) shadowsToggle.isOn = true;
                if (renderScaleSlider != null) renderScaleSlider.value = 1.0f;
                if (postProcessToggle != null) postProcessToggle.isOn = true;
                break;
        }

        if (applyImmediately)
            ApplySettings();
    }

    // Apply current UI values to engine and save to PlayerPrefs.
    public void ApplySettings()
    {
        int q = qualityDropdown != null ? qualityDropdown.value : defaultQuality;
        QualitySettings.SetQualityLevel(q);
        PlayerPrefs.SetInt(KEY_QUALITY, q);

        bool shadowsOn = shadowsToggle != null ? shadowsToggle.isOn : defaultShadows;
        QualitySettings.shadows = shadowsOn ? ShadowQuality.All : ShadowQuality.Disable;
        PlayerPrefs.SetInt(KEY_SHADOWS, shadowsOn ? 1 : 0);

        float scale = renderScaleSlider != null ? Mathf.Clamp(renderScaleSlider.value, 0.5f, 1f) : defaultRenderScale;
        ScalableBufferManager.ResizeBuffers(scale, scale);
        PlayerPrefs.SetFloat(KEY_RENDERSCALE, scale);

        bool ppOn = postProcessToggle != null ? postProcessToggle.isOn : defaultPostProcessing;
        if (postProcessingVolume != null)
            postProcessingVolume.SetActive(ppOn);
        PlayerPrefs.SetInt(KEY_POST, ppOn ? 1 : 0);

        PlayerPrefs.Save();
    }

    // Load saved settings (or defaults) and apply them.
    public void LoadSettings()
    {
        int q = PlayerPrefs.GetInt(KEY_QUALITY, defaultQuality);
        if (qualityDropdown != null)
            qualityDropdown.value = Mathf.Clamp(q, 0, Mathf.Max(0, qualityDropdown.options.Count - 1));
        QualitySettings.SetQualityLevel(q);

        bool shadowsOn = PlayerPrefs.GetInt(KEY_SHADOWS, defaultShadows ? 1 : 0) == 1;
        if (shadowsToggle != null) shadowsToggle.isOn = shadowsOn;
        QualitySettings.shadows = shadowsOn ? ShadowQuality.All : ShadowQuality.Disable;

        float scale = PlayerPrefs.GetFloat(KEY_RENDERSCALE, defaultRenderScale);
        if (renderScaleSlider != null) renderScaleSlider.value = Mathf.Clamp(scale, renderScaleSlider.minValue, renderScaleSlider.maxValue);
        ScalableBufferManager.ResizeBuffers(scale, scale);

        bool ppOn = PlayerPrefs.GetInt(KEY_POST, defaultPostProcessing ? 1 : 0) == 1;
        if (postProcessToggle != null) postProcessToggle.isOn = ppOn;
        if (postProcessingVolume != null) postProcessingVolume.SetActive(ppOn);
    }

    // Reset UI controls to defaults and apply.
    public void ResetToDefaults()
    {
        if (qualityDropdown != null) qualityDropdown.value = defaultQuality;
        if (shadowsToggle != null) shadowsToggle.isOn = defaultShadows;
        if (renderScaleSlider != null) renderScaleSlider.value = defaultRenderScale;
        if (postProcessToggle != null) postProcessToggle.isOn = defaultPostProcessing;

        ApplySettings();
    }

    void OnDestroy()
    {
        // Remove listeners
        if (qualityDropdown != null) qualityDropdown.onValueChanged.RemoveListener(OnQualityChanged);
        if (applyButton != null) applyButton.onClick.RemoveListener(ApplySettings);
        if (resetButton != null) resetButton.onClick.RemoveListener(ResetToDefaults);
    }
}