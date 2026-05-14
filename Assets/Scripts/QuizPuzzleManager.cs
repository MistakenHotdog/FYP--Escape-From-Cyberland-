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
        // Hide all at start
        for (int i = 0; i < questionPanels.Length; i++)
        {
            questionPanels[i].SetActive(false);
        }
    }

    // 🔥 OPEN QUIZ
    public void OpenPuzzle()
    {
        Debug.Log("QUIZ OPENED");

        currentQuestion = 0;
        score = 0;
        answered = false;

        Time.timeScale = 0f;

        // 🔥 FORCE FIRST QUESTION
        questionPanels[0].SetActive(true);
    }

    // 🔥 ANSWER SELECTED
    public void SelectAnswer(bool correct)
    {
        if (answered) return;

        answered = true;

        if (correct)
        {
            score++;
            Debug.Log("CORRECT");
        }
        else
        {
            Debug.Log("WRONG");
        }

        Invoke(nameof(NextQuestion), 0.2f);
    }

    // 🔥 NEXT QUESTION
    void NextQuestion()
    {
        questionPanels[currentQuestion].SetActive(false);

        currentQuestion++;

        if (currentQuestion >= questionPanels.Length)
        {
            FinishQuiz();
            return;
        }

        questionPanels[currentQuestion].SetActive(true);

        answered = false;
    }

    // 🔥 FINISH
    void FinishQuiz()
    {
        Time.timeScale = 1f;

        foreach (GameObject panel in questionPanels)
        {
            panel.SetActive(false);
        }

        if (score >= 3)
        {
            Debug.Log("QUIZ PASSED");

            if (door != null)
                door.OpenDoor();

            if (scanner != null)
                scanner.MarkCompleted();
        }
        else
        {
            Debug.Log("QUIZ FAILED");
        }
    }
}