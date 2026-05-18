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

    [Header("Security Systems")]
    public GameObject[] securityCameras;
    public GameObject[] bugRobots;

    private string correctKey = "CYBERLAND_KEY";

    // 🔥 OPEN TERMINAL
    public void OpenPC()
    {
        pcPanel.SetActive(true);

        // Pause gameplay
        Time.timeScale = 0f;

        HideAllScreens();

        // Unlock Key Vault
        if (keyVaultTrigger != null)
        {
            keyVaultTrigger.SetActive(true);
        }

        // Show input screen
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

            // 🔥 Disable all cameras
            foreach (GameObject cam in securityCameras)
            {
                if (cam != null)
                {
                    cam.SetActive(false);
                }
            }

            // 🔥 Disable all bug robots
            foreach (GameObject robot in bugRobots)
            {
                if (robot != null)
                {
                    robot.SetActive(false);
                }
            }

            // Open server room door
            if (serverDoor != null)
            {
                serverDoor.OpenDoor();
            }

            // Disable interaction forever
            if (interactionTrigger != null)
            {
                interactionTrigger.MarkCompleted();
            }

            Debug.Log("✅ MAINFRAME HACKED");
        }
        else
        {
            // ❌ Wrong key
            accessDeniedScreen.SetActive(true);
            invalidKeyScreen.SetActive(true);

            Debug.Log("❌ Invalid Key");
        }
    }

    // 🔥 CLOSE PANEL
    public void ClosePanel()
    {
        pcPanel.SetActive(false);

        // Resume gameplay
        Time.timeScale = 1f;

        HideAllScreens();
    }

    // 🔥 HIDE ALL SCREENS
    void HideAllScreens()
    {
        if (enterKeyScreen != null)
            enterKeyScreen.SetActive(false);

        if (accessDeniedScreen != null)
            accessDeniedScreen.SetActive(false);

        if (invalidKeyScreen != null)
            invalidKeyScreen.SetActive(false);

        if (successScreen != null)
            successScreen.SetActive(false);
    }
}