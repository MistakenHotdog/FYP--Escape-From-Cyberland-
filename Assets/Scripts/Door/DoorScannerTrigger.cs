using UnityEngine;

public class DoorScannerTrigger : MonoBehaviour
{
    public GameObject hackButton;
    public PasswordPuzzleManager puzzle;

    private bool isCompleted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (isCompleted) return;

        // 🔥 Check if player has phone
        if (PlayerInventory.Instance != null && PlayerInventory.Instance.hasPhone)
        {
            hackButton.SetActive(true);
        }
        else
        {
            Debug.Log("❌ You need a phone to hack!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        hackButton.SetActive(false);
    }

    public void StartHack()
    {
        if (isCompleted) return;

        // 🔥 Double check phone requirement
        if (PlayerInventory.Instance == null || !PlayerInventory.Instance.hasPhone)
        {
            Debug.Log("❌ Cannot hack without phone!");
            return;
        }

        hackButton.SetActive(false);
        puzzle.OpenPuzzle();
    }

    // 🔥 Called after puzzle success
    public void MarkCompleted()
    {
        isCompleted = true;

        if (hackButton != null)
            hackButton.SetActive(false);

        // Optional: disable trigger completely
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }
}