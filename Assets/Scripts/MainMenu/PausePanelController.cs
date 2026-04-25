using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanelController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject optionsPanel;

    [Header("Control UI (SEPARATE)")]
    public GameObject joystickUI;
    public GameObject buttonsUI;
    public GameObject voiceUI;

    [Header("Other UI")]
    public GameObject pauseButton;

    // ---------------- PAUSE ----------------
    public void PauseGame()
    {
        Debug.Log("[Pause] Game Paused");

        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.transform.SetAsLastSibling();
            pausePanel.SetActive(true);
        }
    }

    // ---------------- RESUME ----------------
    public void ResumeGame()
    {
        Debug.Log("[Pause] Resuming Game");

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;

        // Restore control system
        PlayerMove player = FindObjectOfType<PlayerMove>();
        if (player != null)
            player.RefreshControlMode();

        ControlUIManager ui = FindObjectOfType<ControlUIManager>();
        if (ui != null)
            ui.RefreshUI();

        // Make sure pause button is visible again
        if (pauseButton != null)
            pauseButton.SetActive(true);
    }

    // ---------------- OPEN OPTIONS ----------------
    public void OpenOptions()
    {
        Debug.Log("[Pause] Open Options");

        // 🔥 HIDE PAUSE MENU
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // 🔥 SHOW OPTIONS
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            optionsPanel.transform.SetAsLastSibling();
        }

        // 🔥 HIDE ALL CONTROL SYSTEMS
        if (joystickUI != null) joystickUI.SetActive(false);
        if (buttonsUI != null) buttonsUI.SetActive(false);
        if (voiceUI != null) voiceUI.SetActive(false);

        // 🔥 DISABLE PAUSE BUTTON
        if (pauseButton != null)
            pauseButton.SetActive(false);
    }

    // ---------------- CLOSE OPTIONS ----------------
    public void CloseOptions()
    {
        Debug.Log("[Pause] Close Options");

        // 🔥 HIDE OPTIONS
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        // 🔥 SHOW PAUSE MENU AGAIN
        if (pausePanel != null)
            pausePanel.SetActive(true);

        // 🔥 RESTORE CORRECT CONTROL SYSTEM
        ControlUIManager ui = FindObjectOfType<ControlUIManager>();
        if (ui != null)
            ui.RefreshUI();

        // 🔥 ENABLE PAUSE BUTTON AGAIN
        if (pauseButton != null)
            pauseButton.SetActive(true);
    }

    // ---------------- EXIT ----------------
    public void ExitToMainMenu()
    {
        Debug.Log("[Pause] Exit to Main Menu");

        if (GameplaySessionLogger.Instance != null)
        {
            GameplaySessionLogger.Instance.EndSession(false);
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneNames.MainMenu);
    }
}