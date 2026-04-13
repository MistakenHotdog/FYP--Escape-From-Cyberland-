using UnityEngine;

public class ControlUIManager : MonoBehaviour
{
    [Tooltip("The parent GameObject containing the Joystick UI.")]
    public GameObject joystickUI;
    [Tooltip("The parent GameObject containing the Button UI (Up/Down/Left/Right buttons).")]
    public GameObject buttonUI;

    void Awake()
    {
        // Subscribe early so we never miss an event, even if this object gets briefly deactivated.
        ControlSettings.OnControlChanged -= Apply;
        ControlSettings.OnControlChanged += Apply;
    }

    void Start()
    {
        Apply(PlayerPrefs.GetInt("ControlMode", 0));
    }

    void OnDestroy()
    {
        ControlSettings.OnControlChanged -= Apply;
    }

    void Apply(int mode)
    {
        if (joystickUI != null) joystickUI.SetActive(mode == 0);
        if (buttonUI   != null) buttonUI.SetActive(mode == 1);
    }
}
