using UnityEngine;
using TMPro;

public class KeyVaultPC : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;

    public TMP_Text outputText;
    public TMP_InputField shiftInput;
    public TMP_Text resultText;

    public GameObject keyObtainedUI;

    [Header("Interaction")]
    public KeyVaultInteraction interactionTrigger;

    // 🔥 OPEN VAULT
    public void OpenVault()
    {
        panel.SetActive(true);

        // Pause gameplay
        Time.timeScale = 0f;

        outputText.text =
            "CYBERLAND KEY VAULT SYSTEM\n\n" +
            "Authorized Access Only\n" +
            "Encryption Storage: ACTIVE\n" +
            "Key File: MASTERKEY.enc\n" +
            "Status: ENCRYPTED 🔒\n\n" +
            "Encrypted Output:\nNJMJWPQJ_XLJ\n\n" +
            "Decryption Type: ROT Cipher\n\n" +
            "Enter Shift Value:";
    }

    // 🔥 TRY DECRYPT
    public void TryDecrypt()
    {
        if (shiftInput.text == "11")
        {
            resultText.text =
                "Decryption Successful ✅\n\n" +
                "Key Extracted: CYBERLAND_KEY\n\n" +
                "Downloading key to inventory...";

            PlayerInventory.Instance.hasEncryptionKey = true;

            if (keyObtainedUI != null)
                keyObtainedUI.SetActive(true);

            // Disable interaction forever
            if (interactionTrigger != null)
                interactionTrigger.MarkCompleted();
        }
        else
        {
            resultText.text = "❌ Incorrect Shift Value";
        }
    }

    // 🔥 CLOSE PANEL
    public void ClosePanel()
    {
        panel.SetActive(false);

        // Resume gameplay
        Time.timeScale = 1f;
    }
}