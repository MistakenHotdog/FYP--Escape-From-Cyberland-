using UnityEngine;

public class ControlSettings : MonoBehaviour
{
    public static int mode;

    public delegate void ControlChange(int m);
    public static event ControlChange OnControlChanged;

    void Awake()
    {
        // Load control mode ONLY from Main Menu selection
        // (We now use UIType as the single source of truth)
        int uiType = PlayerPrefs.GetInt("UIType", 1);

        // Convert UIType → movement mode
        // 1 = Joystick, 2 = Buttons, 3 = Voice
        mode = (uiType == 1) ? 0 : 1;

        Debug.Log("[ControlSettings] Loaded mode: " + mode);

        // Notify any listeners ONCE at start
        OnControlChanged?.Invoke(mode);
    }
}