using UnityEngine;

public class ControlUIManager : MonoBehaviour
{
    public GameObject joystickUI;
    public GameObject buttonUI;

    void Start()
    {
        ApplyMode(PlayerPrefs.GetInt("ControlMode", 0));
    }

    void OnEnable()
    {
        ControlSettings.ControlChanged += ApplyMode;
    }

    void OnDisable()
    {
        ControlSettings.ControlChanged -= ApplyMode;
    }

    void ApplyMode(int mode)
    {
        Debug.Log("Switching UI to mode: " + mode);

        joystickUI.SetActive(mode == 0);
        buttonUI.SetActive(mode == 1);
    }
}