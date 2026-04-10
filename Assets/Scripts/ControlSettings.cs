using UnityEngine;

public class ControlSettings : MonoBehaviour
{
    public static int mode;

    public delegate void ControlChange(int m);
    public static event ControlChange OnControlChanged;

    public void SetJoystick()
    {
        mode = 0;
        PlayerPrefs.SetInt("ControlMode", 0);
        PlayerPrefs.Save();
        OnControlChanged?.Invoke(mode);
    }

    public void SetButtons()
    {
        mode = 1;
        PlayerPrefs.SetInt("ControlMode", 1);
        PlayerPrefs.Save();
        OnControlChanged?.Invoke(mode);
    }

    void Awake()
    {
        mode = PlayerPrefs.GetInt("ControlMode", 0);
    }
}