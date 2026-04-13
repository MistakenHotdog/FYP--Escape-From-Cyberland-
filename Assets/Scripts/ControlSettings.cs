using UnityEngine;

public class ControlSettings : MonoBehaviour
{
    public static int mode;

    public delegate void ControlChange(int m);
    public static event ControlChange OnControlChanged;

    [Header("Auto-close panels on selection (optional)")]
    [Tooltip("Drag here any panels you want to close when a control mode is picked (e.g. OptionsPanel, PauseMenu).")]
    public GameObject[] panelsToClose;

    [Header("Resume game on selection")]
    [Tooltip("Set Time.timeScale back to 1 after selecting a mode. Keep enabled when this is used from a pause menu.")]
    public bool resumeOnSelection = true;

    void Awake()
    {
        mode = PlayerPrefs.GetInt("ControlMode", 0);
    }

    // Hook these to your UI buttons' OnClick
    public void SetJoystick() { ApplyMode(0); }
    public void SetButtons()  { ApplyMode(1); }

    private void ApplyMode(int newMode)
    {
        mode = newMode;
        PlayerPrefs.SetInt("ControlMode", newMode);
        PlayerPrefs.Save();

        // Notify all listeners (PlayerMove, ControlUIManager) BEFORE deactivating anything
        OnControlChanged?.Invoke(newMode);

        // Close specified panels
        if (panelsToClose != null)
        {
            foreach (var p in panelsToClose)
                if (p != null) p.SetActive(false);
        }

        // Unfreeze time so the game doesn't get stuck
        if (resumeOnSelection)
            Time.timeScale = 1f;
    }
}
