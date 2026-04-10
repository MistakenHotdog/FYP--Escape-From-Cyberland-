using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TerminationUI : MonoBehaviour
{
    public GameObject uiPanel;
    public Text resultText;

    private bool isShowing = false;

    void Start()
    {
        // Don't hide the panel if ShowGameOver/ShowVictory was already called.
        // This handles the case where the TerminationUI starts inactive in the scene
        // and Start() runs for the first time AFTER ShowGameOver activated it.
        if (!isShowing && uiPanel != null)
            uiPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        isShowing = true;

        if (resultText != null)
        {
            resultText.text = "GAME OVER";
            resultText.color = Color.red;
        }

        if (uiPanel != null)
        {
            uiPanel.SetActive(true);

            Canvas canvas = uiPanel.GetComponent<Canvas>();
            if (canvas == null)
                canvas = uiPanel.GetComponentInParent<Canvas>();
            if (canvas != null)
                canvas.sortingOrder = 999;
        }

        Time.timeScale = 0f;
    }

    public void ShowVictory()
    {
        isShowing = true;

        if (resultText != null)
        {
            resultText.text = "VICTORY!";
            resultText.color = Color.green;
        }

        if (uiPanel != null)
        {
            uiPanel.SetActive(true);

            Canvas canvas = uiPanel.GetComponent<Canvas>();
            if (canvas == null)
                canvas = uiPanel.GetComponentInParent<Canvas>();
            if (canvas != null)
                canvas.sortingOrder = 999;
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneNames.MainMenu);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
