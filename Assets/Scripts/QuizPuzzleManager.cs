using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizPuzzleManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TMP_Text questionText;
    public TMP_Text progressText;

    public Button[] answerButtons;
    public TMP_Text[] answerTexts;

    public Button nextButton;
    public TMP_Text nextButtonText;

    [Header("Door")]
    public DoorController door;

    private int currentQuestion = 0;
    private int score = 0;
    private int selectedAnswer = -1;

    // 🔥 QUESTIONS
    private string[] questions =
    {
        "What is two-factor authentication (2FA)?",
        "What is a VPN used for?",
        "What is a secure network?"
    };

    // 🔥 ANSWERS
    private string[,] answers =
    {
        { "Using two methods to verify identity", "Using two passwords", "Logging in twice" },
        { "Encrypting your internet connection", "Speeding up your device", "Blocking viruses" },
        { "A protected and encrypted connection", "Public Wi-Fi", "Any internet connection" }
    };

    // 🔥 CORRECT ANSWERS (index of correct option)
    private int[] correctAnswers = { 0, 0, 0 };

    // ---------------- OPEN ----------------
    public void OpenPuzzle()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;

        currentQuestion = 0;
        score = 0;

        LoadQuestion();
    }

    // ---------------- LOAD QUESTION ----------------
    void LoadQuestion()
    {
        selectedAnswer = -1;

        questionText.text = questions[currentQuestion];
        progressText.text = (currentQuestion + 1) + "/3";

        for (int i = 0; i < 3; i++)
        {
            answerTexts[i].text = answers[currentQuestion, i];

            int index = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => SelectAnswer(index));
        }

        // Change button text
        if (currentQuestion == questions.Length - 1)
            nextButtonText.text = "Done";
        else
            nextButtonText.text = "Next";
    }

    // ---------------- SELECT ANSWER ----------------
    public void SelectAnswer(int index)
    {
        selectedAnswer = index;
        Debug.Log("Selected: " + index);
    }

    // ---------------- NEXT BUTTON ----------------
    public void Next()
    {
        if (selectedAnswer == -1)
        {
            Debug.Log("❗ Select an answer first!");
            return;
        }

        // Check correctness
        if (selectedAnswer == correctAnswers[currentQuestion])
            score++;

        currentQuestion++;

        if (currentQuestion >= questions.Length)
        {
            FinishQuiz();
        }
        else
        {
            LoadQuestion();
        }
    }

    // ---------------- FINISH ----------------
    void FinishQuiz()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;

        if (score == questions.Length)
        {
            Debug.Log("✅ All correct!");
            if (door != null)
                door.OpenDoor();
        }
        else
        {
            Debug.Log("❌ Try Again!");
        }
    }
}