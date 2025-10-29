using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("LoadingScene"); // your gameplay scene name
    }

    public void OpenSettings()
    {
        // For now, just a log or open panel
        Debug.Log("Settings Opened");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
