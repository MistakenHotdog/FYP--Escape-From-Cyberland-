using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanelController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;  // Assign your Pause Panel in inspector

    // Called by the on-screen Pause button
    public void PauseGame()
    {
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // Called by Exit button
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;          // Always unpause before switching scene
        SceneManager.LoadScene(SceneNames.MainMenu);
    }
}
