using UnityEngine;
using UnityEngine.UI;

// Handles the "Music" toggle in the Options panel.
// Uses AudioListener.volume so it affects ALL AudioSources in the scene at once.
public class AudioSettings : MonoBehaviour
{
    [Header("UI")]
    public Toggle musicToggle;

    [Header("Defaults")]
    public bool defaultMusicOn = true;

    public const string KEY_MUSIC = "music";

    void Awake()
    {
        if (musicToggle != null)
            musicToggle.onValueChanged.AddListener(OnMusicToggled);
    }

    void Start()
    {
        // Read saved value, reflect in UI, apply to engine
        bool on = PlayerPrefs.GetInt(KEY_MUSIC, defaultMusicOn ? 1 : 0) == 1;
        if (musicToggle != null)
            musicToggle.isOn = on;
        AudioListener.volume = on ? 1f : 0f;
    }

    private void OnMusicToggled(bool isOn)
    {
        AudioListener.volume = isOn ? 1f : 0f;
        PlayerPrefs.SetInt(KEY_MUSIC, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        if (musicToggle != null)
            musicToggle.onValueChanged.RemoveListener(OnMusicToggled);
    }
}
