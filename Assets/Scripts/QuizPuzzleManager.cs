using UnityEngine;

public class QuizPuzzleManager : MonoBehaviour
{
    [Header("Question Panels")]
    public GameObject[] questionPanels;

    [Header("Door")]
    public DoorController door;

    [Header("Scanner")]
    public QuizDoorScannerTrigger scanner;

    private int currentQuestion = 0;
    private int score = 0;
    private bool answered = false;

    private void Start()
    {
        // Hide all panels at start
        for (int i = 0; i < questionPanels.Length; i++)
        {
            if (questionPanels[i] != null)
            {
                questionPanels[i].SetActive(false);
            }
        }
    }

    // 🔥 OPEN QUIZ
    public void OpenPuzzle()
    {
        Debug.Log("QUIZ OPENED");

        currentQuestion = 0;
        score = 0;
        answered = false;

        // Pause gameplay
        Time.timeScale = 0f;

        // Hide all first
        for (int i = 0; i < questionPanels.Length; i++)
        {
            if (questionPanels[i] != null)
            {
                questionPanels[i].SetActive(false);
            }
        }

        // Show first question
        if (questionPanels.Length > 0)
        {
            questionPanels[0].SetActive(true);
        }
    }

    // 🔥 ANSWER BUTTON CLICK
    public void SelectAnswer(bool correct)
    {
        // Prevent double clicks
        if (answered) return;

        answered = true;

        if (correct)
        {
            score++;
            Debug.Log("✅ Correct Answer");
        }
        else
        {
            Debug.Log("❌ Wrong Answer");
        }

        // Go instantly to next question
        NextQuestion();
    }

    // 🔥 NEXT QUESTION
    void NextQuestion()
    {
        // Hide current question
        if (questionPanels[currentQuestion] != null)
        {
            questionPanels[currentQuestion].SetActive(false);
        }

        currentQuestion++;

        // Finished all questions
        if (currentQuestion >= questionPanels.Length)
        {
            FinishQuiz();
            return;
        }

        // Show next question
        if (questionPanels[currentQuestion] != null)
        {
            questionPanels[currentQuestion].SetActive(true);
        }

        answered = false;
    }

    // 🔥 FINISH QUIZ
    void FinishQuiz()
    {
        // Resume gameplay
        Time.timeScale = 1f;

        // Hide all panels
        for (int i = 0; i < questionPanels.Length; i++)
        {
            if (questionPanels[i] != null)
            {
                questionPanels[i].SetActive(false);
            }
        }

        // PASS
        if (score >= 3)
        {
            Debug.Log("✅ QUIZ PASSED");

            // Open door
            if (door != null)
            {
                door.OpenDoor();
            }

            // Disable scanner forever
            if (scanner != null)
            {
                scanner.MarkCompleted();
            }
        }
        else
        {
            Debug.Log("❌ QUIZ FAILED");
        }
    }
}