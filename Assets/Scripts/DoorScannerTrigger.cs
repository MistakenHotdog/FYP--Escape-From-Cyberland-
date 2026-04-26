using UnityEngine;

public class DoorScannerTrigger : MonoBehaviour
{
    public GameObject hackButton;
    public PasswordPuzzleManager puzzle;

    private bool isCompleted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCompleted)
        {
            hackButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hackButton.SetActive(false);
        }
    }

    public void StartHack()
    {
        if (isCompleted) return;

        hackButton.SetActive(false);
        puzzle.OpenPuzzle();
    }

    // 🔥 Called when puzzle is solved
    public void MarkCompleted()
    {
        isCompleted = true;

        // Make sure button is hidden forever
        if (hackButton != null)
            hackButton.SetActive(false);

        // OPTIONAL: disable trigger completely
        GetComponent<Collider>().enabled = false;
    }
}