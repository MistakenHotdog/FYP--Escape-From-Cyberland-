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

    void Awake()
    {
        // 🔥 IMPORTANT: Force everything OFF at start
        ForceDisableAll();
    }

    void Start()
    {
        RefreshUI();
    }

    // 🔥 MAIN METHOD (used everywhere)
    public void RefreshUI()
    {
        int uiType = PlayerPrefs.GetInt(UI_TYPE_KEY, 1);

        Debug.Log("[ControlUIManager] RefreshUI → UIType = " + uiType);

        Apply(uiType);
    }

    // 🔥 CORE FIX
    void Apply(int uiType)
    {
        // 🚨 STEP 1: FORCE DISABLE EVERYTHING
        ForceDisableAll();

        // 🚨 STEP 2: ENABLE ONLY THE CORRECT ONE
        switch (uiType)
        {
            case 1: // Joystick
                if (joystickUI != null)
                {
                    joystickUI.SetActive(true);
                    Debug.Log("Joystick UI ENABLED");
                }
                break;

            case 2: // Buttons
                if (buttonUI != null)
                {
                    buttonUI.SetActive(true);
                    Debug.Log("Button UI ENABLED");
                }
                break;

            case 3: // Voice
                if (voiceUI != null)
                {
                    voiceUI.SetActive(true);
                    Debug.Log("Voice UI ENABLED");
                }
                break;

            default:
                Debug.LogWarning("Unknown UIType: " + uiType);
                break;
        }
    }

    // 🔥 HARD RESET (VERY IMPORTANT)
    void ForceDisableAll()
    {
        if (joystickUI != null)
            joystickUI.SetActive(false);

        if (buttonUI != null)
            buttonUI.SetActive(false);

        if (voiceUI != null)
            voiceUI.SetActive(false);
    }
}