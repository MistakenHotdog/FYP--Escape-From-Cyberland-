using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject uiSelectionPanel;

    [Header("Next Scene")]
    public string nextSceneName = SceneNames.LoadingScene;

    private const string UI_TYPE_KEY = "UIType";

    void Start()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (uiSelectionPanel != null) uiSelectionPanel.SetActive(false);
    }

    public void PlayGame()
    {
        // Instead of loading scene immediately, open UI selection screen
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (uiSelectionPanel != null) uiSelectionPanel.SetActive(true);
    }

    public void SelectJoystickUI()
    {
        PlayerPrefs.SetInt(UI_TYPE_KEY, 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(nextSceneName);
    }

    public void SelectButtonsUI()
    {
        PlayerPrefs.SetInt(UI_TYPE_KEY, 2);
        PlayerPrefs.Save();
        SceneManager.LoadScene(nextSceneName);
    }

    public void SelectVoiceUI()
    {
        PlayerPrefs.SetInt(UI_TYPE_KEY, 3);
        PlayerPrefs.Save();
        SceneManager.LoadScene(nextSceneName);
    }

    public void BackToMainMenu()
    {
        if (uiSelectionPanel != null) uiSelectionPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void OpenSettings()
    {
        Debug.Log("Settings Opened");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}