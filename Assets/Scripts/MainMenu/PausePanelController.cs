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
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        FindObjectOfType<PlayerMove>()?.RefreshControlMode();
        FindObjectOfType<ControlUIManager>()?.RefreshUI();
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