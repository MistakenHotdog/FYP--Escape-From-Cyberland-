using UnityEngine;

public class ControlUIManager : MonoBehaviour
{
    public GameObject joystickUI;
    public GameObject buttonUI;

    void Start()
    {
        Apply(PlayerPrefs.GetInt("ControlMode", 0));
    }

    void OnEnable()
    {
        ControlSettings.OnControlChanged += Apply;
    }

    void OnDisable()
    {
        ControlSettings.OnControlChanged -= Apply;
    }

    void Apply(int mode)
    {
        if (joystickUI != null) joystickUI.SetActive(mode == 0);
        if (buttonUI != null) buttonUI.SetActive(mode == 1);
    }
}