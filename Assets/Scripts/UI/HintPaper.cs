using UnityEngine;

public class HintPaper : MonoBehaviour
{
    [Header("UI")]
    public GameObject readButton;
    public GameObject hintPanel;
    public GameObject closeButton;

    private bool playerInside = false;

    private void Start()
    {
        // Hide UI at start
        if (readButton != null)
            readButton.SetActive(false);

        if (hintPanel != null)
            hintPanel.SetActive(false);

        if (closeButton != null)
            closeButton.SetActive(false);
    }

    // 🔥 PLAYER ENTERS TRIGGER
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        if (readButton != null)
            readButton.SetActive(true);
    }

    // 🔥 PLAYER EXITS TRIGGER
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        if (readButton != null)
            readButton.SetActive(false);
    }

    // 🔥 OPEN HINT PANEL
    public void OpenHint()
    {
        if (!playerInside) return;

        if (hintPanel != null)
            hintPanel.SetActive(true);

        if (closeButton != null)
            closeButton.SetActive(true);

        if (readButton != null)
            readButton.SetActive(false);

        // Pause gameplay
        Time.timeScale = 0f;
    }

    // 🔥 CLOSE HINT PANEL
    public void CloseHint()
    {
        if (hintPanel != null)
            hintPanel.SetActive(false);

        if (closeButton != null)
            closeButton.SetActive(false);

        // Resume gameplay
        Time.timeScale = 1f;

        // Show read button again if player still nearby
        if (playerInside && readButton != null)
            readButton.SetActive(true);
    }
}