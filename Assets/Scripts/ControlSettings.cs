using UnityEngine;

public class ControlSettings : MonoBehaviour
{
    public static int mode = 0;

    public delegate void OnControlChanged(int newMode);
    public static event OnControlChanged ControlChanged;

    public void SetJoystick()
    {
        mode = 0;
        PlayerPrefs.SetInt("ControlMode", mode);
        PlayerPrefs.Save();

        ControlChanged?.Invoke(mode); // 🔥 notify
    }

    public void SetButtons()
    {
        mode = 1;
        PlayerPrefs.SetInt("ControlMode", mode);
        PlayerPrefs.Save();

        ControlChanged?.Invoke(mode); // 🔥 notify
    }

    void Awake()
    {
        mode = PlayerPrefs.GetInt("ControlMode", 0);
    }
}