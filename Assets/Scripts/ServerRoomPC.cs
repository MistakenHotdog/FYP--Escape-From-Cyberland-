using UnityEngine;
using TMPro;

public class ServerRoomPC : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject pcPanel;

    [Header("Screens")]
    public GameObject enterKeyScreen;
    public GameObject accessDeniedScreen;
    public GameObject invalidKeyScreen;
    public GameObject successScreen;

    [Header("Input")]
    public TMP_InputField keyInput;

    [Header("Door")]
    public DoorController serverDoor;

    [Header("Interaction")]
    public ServerPCInteraction interactionTrigger;

    [Header("Unlock")]
    public GameObject keyVaultTrigger;

    private string correctKey = "CYBERLAND_KEY";

    // 🔥 OPEN TERMINAL
    public void OpenPC()
    {
        pcPanel.SetActive(true);

        Time.timeScale = 0f;

        HideAllScreens();

        // Unlock Key Vault
        if (keyVaultTrigger != null)
        {
            keyVaultTrigger.SetActive(true);
        }

        // Show enter key screen only
        enterKeyScreen.SetActive(true);
    }

    // 🔥 SUBMIT KEY
    public void SubmitKey()
    {
        HideAllScreens();

        // ✅ Correct key
        if (keyInput.text.ToUpper() == correctKey)
        {
            successScreen.SetActive(true);

            if (serverDoor != null)
            {
                serverDoor.OpenDoor();
            }

            // Disable future interaction
            if (interactionTrigger != null)
            {
                interactionTrigger.MarkCompleted();
            }
        }
        else
        {
            // ❌ Wrong key
            accessDeniedScreen.SetActive(true);
            invalidKeyScreen.SetActive(true);
        }
    }

    // 🔥 CLOSE PANEL
    public void ClosePanel()
    {
        pcPanel.SetActive(false);

        Time.timeScale = 1f;

        HideAllScreens();
    }

    // 🔥 HIDE ALL SCREENS
    void HideAllScreens()
    {
        enterKeyScreen.SetActive(false);
        accessDeniedScreen.SetActive(false);
        invalidKeyScreen.SetActive(false);
        successScreen.SetActive(false);
    }
}