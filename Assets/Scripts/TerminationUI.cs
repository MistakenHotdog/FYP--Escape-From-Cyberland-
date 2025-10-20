using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TerminationUI : MonoBehaviour
{
    public GameObject uiPanel;      // The main panel with text & buttons
    public Text resultText;         // Legacy text (Game Over / Victory)
    private bool isActive = false;

    void Start()
    {
        uiPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        resultText.text = "GAME OVER";
        resultText.color = Color.red;
        uiPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        isActive = true;
    }

    public void ShowVictory()
    {
        resultText.text = "VICTORY!";
        resultText.color = Color.green;
        uiPanel.SetActive(true);
        Time.timeScale = 0f;
        isActive = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
