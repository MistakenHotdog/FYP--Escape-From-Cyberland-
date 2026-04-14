using UnityEngine;

public class ControlUIManager : MonoBehaviour
{
    [Tooltip("The parent GameObject containing the Joystick UI.")]
    public GameObject joystickUI;

    [Tooltip("The parent GameObject containing the Button UI.")]
    public GameObject buttonUI;

    [Tooltip("The parent GameObject containing the Voice Command UI.")]
    public GameObject voiceUI;

    private const string UI_TYPE_KEY = "UIType";

    void Start()
    {
        Apply(PlayerPrefs.GetInt(UI_TYPE_KEY, 1));
    }

    void Apply(int uiType)
    {
        if (joystickUI != null) joystickUI.SetActive(uiType == 1);
        if (buttonUI != null) buttonUI.SetActive(uiType == 2);
        if (voiceUI != null) voiceUI.SetActive(uiType == 3);
    }
}