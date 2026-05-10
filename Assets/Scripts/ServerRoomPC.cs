using UnityEngine;
using TMPro;

public class ServerRoomPC : MonoBehaviour
{
    [Header("UI")]
    public GameObject pcPanel;
    public TMP_Text messageText;
    public TMP_InputField keyInput;

    [Header("Door")]
    public DoorController serverDoor;

    [Header("Interaction")]
    public ServerPCInteraction interactionTrigger;

    private string correctKey = "CYBERLAND_KEY";

    // 🔥 OPEN PANEL
    public void OpenPC()
    {
        pcPanel.SetActive(true);

        // Pause gameplay
        Time.timeScale = 0f;

        if (!PlayerInventory.Instance.hasEncryptionKey)
        {
            messageText.text =
                "MAINFRAME TERMINAL\n\n" +
                "ACCESS DENIED ❌\n\n" +
                "Encryption Key Required: CYBERLAND_MASTER_KEY\n" +
                "Key not found in local storage.\n\n" +
                "Retrieve key from:\nKEY VAULT TERMINAL (Sector B)";
        }
        else
        {
            messageText.text =
                "MAINFRAME TERMINAL\n\n" +
                "Enter Encryption Key:";
        }
    }

    // 🔥 SUBMIT KEY
    public void SubmitKey()
    {
        if (keyInput.text == correctKey)
        {
            messageText.text =
                "KEY ACCEPTED ✅\n\n" +
                "MAINFRAME ACCESS GRANTED\n" +
                "Server Room Door Unlocked...";

            if (serverDoor != null)
                serverDoor.OpenDoor();

            // Disable interaction forever
            if (interactionTrigger != null)
                interactionTrigger.MarkCompleted();
        }
        else
        {
            messageText.text = "❌ INVALID KEY";
        }
    }

    // 🔥 CLOSE PANEL
    public void ClosePanel()
    {
        pcPanel.SetActive(false);

        // Resume gameplay
        Time.timeScale = 1f;
    }
}