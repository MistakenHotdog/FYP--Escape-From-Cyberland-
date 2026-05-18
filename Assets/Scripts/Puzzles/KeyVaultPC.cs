using UnityEngine;
using TMPro;

public class KeyVaultPC : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject panel;

    [Header("Screens")]
    public GameObject vaultMainScreen;
    public GameObject decryptFailedScreen;
    public GameObject decryptSuccessScreen;

    [Header("Input")]
    public TMP_InputField shiftInput;

    [Header("Interaction")]
    public KeyVaultInteraction interactionTrigger;

    // 🔐 Correct ROT shift
    private string correctShift = "11";

    // 🔥 OPEN VAULT PANEL
    public void OpenVault()
    {
        panel.SetActive(true);

        // Pause gameplay
        Time.timeScale = 0f;

        // Hide all screens first
        HideAllScreens();

        // Show main vault terminal
        vaultMainScreen.SetActive(true);
    }

    // 🔥 TRY DECRYPT
    public void TryDecrypt()
    {
        // Hide previous screens
        HideAllScreens();

        // ✅ Correct shift value
        if (shiftInput.text == correctShift)
        {
            decryptSuccessScreen.SetActive(true);

            // Give player encryption key
            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.hasEncryptionKey = true;
            }

            // Disable future interaction
            if (interactionTrigger != null)
            {
                interactionTrigger.MarkCompleted();
            }

            Debug.Log("✅ Encryption Key Obtained");
        }
        else
        {
            // ❌ Wrong shift value
            decryptFailedScreen.SetActive(true);

            Debug.Log("❌ Wrong Shift Value");
        }
    }

    // 🔥 CLOSE PANEL
    public void ClosePanel()
    {
        panel.SetActive(false);

        // Resume gameplay
        Time.timeScale = 1f;

        HideAllScreens();
    }

    // 🔥 HIDE ALL SCREENS
    void HideAllScreens()
    {
        if (vaultMainScreen != null)
            vaultMainScreen.SetActive(false);

        if (decryptFailedScreen != null)
            decryptFailedScreen.SetActive(false);

        if (decryptSuccessScreen != null)
            decryptSuccessScreen.SetActive(false);
    }
}