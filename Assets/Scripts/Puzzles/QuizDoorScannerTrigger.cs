using UnityEngine;

public class QuizDoorScannerTrigger : MonoBehaviour
{
    [Header("UI")]
    public GameObject hackButton;

    [Header("Quiz")]
    public QuizPuzzleManager quizPuzzle;

    private bool isCompleted = false;

    private void Start()
    {
        if (hackButton != null)
            hackButton.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isCompleted) return;

        if (hackButton != null)
            hackButton.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (hackButton != null)
            hackButton.SetActive(false);
    }

    // 🔥 Called by Hack Button
    public void StartHack()
    {
        if (isCompleted) return;

        if (hackButton != null)
            hackButton.SetActive(false);

        if (quizPuzzle != null)
            quizPuzzle.OpenPuzzle();
    }

    // 🔥 Called after successful quiz
    public void MarkCompleted()
    {
        isCompleted = true;

        if (hackButton != null)
            hackButton.SetActive(false);

        // Disable trigger forever
        Collider col = GetComponent<Collider>();

        if (col != null)
            col.enabled = false;
    }
}