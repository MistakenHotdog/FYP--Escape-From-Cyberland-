using UnityEngine;
using TMPro;

public class ServerRoomPC : MonoBehaviour
{
    public GameObject pcPanel;
    public TMP_Text messageText;
    public TMP_InputField keyInput;

    public DoorController serverDoor;

    public ServerPCInteraction interactionTrigger;

    private string correctKey = "CYBERLAND_KEY";

    public void OpenPC()
    {
        pcPanel.SetActive(true);

        if (!PlayerInventory.Instance.hasEncryptionKey)
        {
            messageText.text =
                "MAINFRAME TERMINAL\n\n" +
                "ACCESS DENIED ❌\n\n" +
                "Encryption Key Required: CYBERLAND_MASTER_KEY\n\n" +
                "Retrieve key from:\nKEY VAULT TERMINAL (Sector B)";
        }
        else
        {
            messageText.text =
                "MAINFRAME TERMINAL\n\n" +
                "Enter Encryption Key:";
        }
    }

    public void SubmitKey()
    {
        if (keyInput.text == correctKey)
        {
            messageText.text =
                "KEY ACCEPTED ✅\n\n" +
                "MAINFRAME ACCESS GRANTED\n" +
                "Server Room Door Unlocked...";

            serverDoor.OpenDoor();

            if (interactionTrigger != null)
                interactionTrigger.MarkCompleted();
        }
        else
        {
            messageText.text = "❌ INVALID KEY";
        }
    }
}