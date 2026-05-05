using UnityEngine;

public class QuizDoorScannerTrigger : MonoBehaviour
{
    [Header("UI")]
    public GameObject hackButton;

    [Header("Puzzle")]
    public QuizPuzzleManager quizPuzzle;

    private bool isCompleted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isCompleted) return; // 🔥 prevents button from showing again

        if (hackButton != null)
            hackButton.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (hackButton != null)
            hackButton.SetActive(false);
    }

    public void StartHack()
    {
        if (isCompleted) return;

        if (hackButton != null)
            hackButton.SetActive(false);

        if (quizPuzzle != null)
            quizPuzzle.OpenPuzzle();
    }

    // 🔥 Called after successful puzzle
    public void MarkCompleted()
    {
        isCompleted = true;

        if (hackButton != null)
            hackButton.SetActive(false);

        // Disable trigger completely
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }
}