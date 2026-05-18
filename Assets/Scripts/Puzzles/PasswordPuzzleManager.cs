using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PasswordPuzzleManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject puzzlePanel;
    public TMP_Text timerText;

    [Header("Buttons")]
    public Button weakButton1;   // 123456
    public Button weakButton2;   // admin
    public Button strongButton;  // correct password

    [Header("Settings")]
    public float timeLimit = 5f;

    [Header("References")]
    public DoorController door;
    public DoorScannerTrigger scanner;
    public GameObject doorBlocker;

    private float timer;
    private bool isRunning = false;

    private string correctPassword = "G7#kP!9x@Q";

    void Start()
    {
        // 🔥 Assign button logic in code (NO inspector needed)
        weakButton1.onClick.AddListener(() => SelectPassword("123456"));
        weakButton2.onClick.AddListener(() => SelectPassword("admin"));
        strongButton.onClick.AddListener(() => SelectPassword(correctPassword));
    }

    // ---------------- OPEN PUZZLE ----------------
    public void OpenPuzzle()
    {
        puzzlePanel.SetActive(true);
        Time.timeScale = 0f;

        timer = timeLimit;
        isRunning = true;
    }

    // ---------------- UPDATE TIMER ----------------
    void Update()
    {
        if (!isRunning) return;

        timer -= Time.unscaledDeltaTime;

        if (timerText != null)
            timerText.text = "Time: " + timer.ToString("F1");

        if (timer <= 0f)
        {
            Fail();
        }
    }

    // ---------------- PASSWORD SELECT ----------------
    void SelectPassword(string chosen)
    {
        isRunning = false;

        if (chosen == correctPassword)
            Success();
        else
            Fail();
    }

    // ---------------- SUCCESS ----------------
    void Success()
    {
        Debug.Log("✅ Correct Password");

        puzzlePanel.SetActive(false);
        Time.timeScale = 1f;

        if (door != null)
            door.OpenDoor();

        if (doorBlocker != null)
            doorBlocker.SetActive(false);

        if (scanner != null)
            scanner.MarkCompleted();
    }

    // ---------------- FAIL ----------------
    void Fail()
    {
        Debug.Log("❌ Wrong Password");

        puzzlePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}