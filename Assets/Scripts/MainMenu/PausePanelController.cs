using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanelController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;

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

        // Hide pause UI
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Resume time
        Time.timeScale = 1f;

        // 🔥 DEBUG current control mode
        int uiType = PlayerPrefs.GetInt("UIType", -1);
        Debug.Log("[Pause] UIType after resume = " + uiType);

        // 🔥 FIX 1: Restore player control mode
        PlayerMove player = FindObjectOfType<PlayerMove>();
        if (player != null)
        {
            player.RefreshControlMode();
        }
        else
        {
            Debug.LogWarning("[Pause] PlayerMove not found!");
        }

        // 🔥 FIX 2: Restore correct UI (THIS FIXES JOYSTICK BUG)
        ControlUIManager uiManager = FindObjectOfType<ControlUIManager>();
        if (uiManager != null)
        {
            uiManager.RefreshUI();
        }
        else
        {
            Debug.LogWarning("[Pause] ControlUIManager not found!");
        }
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