using UnityEngine;

public class QuizDoorScannerTrigger : MonoBehaviour
{
    public GameObject hackButton;
    public QuizPuzzleManager quizPuzzle;

    private bool isCompleted = false;

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

    public void StartHack()
    {
        if (isCompleted) return;

        if (hackButton != null)
            hackButton.SetActive(false);

        if (quizPuzzle != null)
            quizPuzzle.OpenPuzzle();
    }

    public void MarkCompleted()
    {
        isCompleted = true;

        if (hackButton != null)
            hackButton.SetActive(false);

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }
}