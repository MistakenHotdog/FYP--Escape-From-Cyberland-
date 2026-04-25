using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject levelSelectionPanel;
    public GameObject uiSelectionPanel;

    private const string UI_TYPE_KEY = "UIType";
    private const string LEVEL_KEY = "SelectedLevel";

    void Start()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (levelSelectionPanel != null) levelSelectionPanel.SetActive(false);
        if (uiSelectionPanel != null) uiSelectionPanel.SetActive(false);
    }

    // ---------------- PLAY ----------------
    public void PlayGame()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (levelSelectionPanel != null) levelSelectionPanel.SetActive(true);
    }

    // ---------------- LEVEL SELECT ----------------
    public void SelectLevel1()
    {
        PlayerPrefs.SetString(LEVEL_KEY, "GameScene1");
        PlayerPrefs.Save();

        OpenUISelection();
    }

    public void SelectLevel2()
    {
        PlayerPrefs.SetString(LEVEL_KEY, "Level2");
        PlayerPrefs.Save();

        OpenUISelection();
    }

    void OpenUISelection()
    {
        if (levelSelectionPanel != null) levelSelectionPanel.SetActive(false);
        if (uiSelectionPanel != null) uiSelectionPanel.SetActive(true);
    }

    // ---------------- CONTROL SELECT ----------------
    public void SelectJoystickUI()
    {
        LoadGameWithUI(1);
    }

    public void SelectButtonsUI()
    {
        LoadGameWithUI(2);
    }

    public void SelectVoiceUI()
    {
        LoadGameWithUI(3);
    }

    void LoadGameWithUI(int uiType)
    {
        PlayerPrefs.SetInt(UI_TYPE_KEY, uiType);
        PlayerPrefs.Save();

        // 🔥 ALWAYS go to loading scene
        SceneManager.LoadScene(SceneNames.LoadingScene);
    }

    // ---------------- BACK ----------------
    public void BackToMainMenu()
    {
        if (levelSelectionPanel != null) levelSelectionPanel.SetActive(false);
        if (uiSelectionPanel != null) uiSelectionPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void BackToLevelSelect()
    {
        if (uiSelectionPanel != null) uiSelectionPanel.SetActive(false);
        if (levelSelectionPanel != null) levelSelectionPanel.SetActive(true);
    }

    // ---------------- OTHER ----------------
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