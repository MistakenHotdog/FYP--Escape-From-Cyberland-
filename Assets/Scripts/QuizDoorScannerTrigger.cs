using UnityEngine;

public class QuizDoorScannerTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public Collider triggerZone;
    public GameObject hackButton;
    public QuizPuzzleManager quizPuzzle;

    [Header("Settings")]
    public bool requirePhone = true;

    private bool isCompleted = false;
    private bool playerInside = false;

    void Update()
    {
        if (isCompleted || player == null || triggerZone == null)
            return;

        Collider playerCol = player.GetComponent<Collider>();

        if (playerCol != null && triggerZone.bounds.Intersects(playerCol.bounds))
        {
            if (!playerInside)
            {
                playerInside = true;
                OnPlayerEnter();
            }
        }
        else
        {
            if (playerInside)
            {
                playerInside = false;
                OnPlayerExit();
            }
        }
    }

    void OnPlayerEnter()
    {
        if (requirePhone && (PlayerInventory.Instance == null || !PlayerInventory.Instance.hasPhone))
        {
            Debug.Log("❌ You need a phone to hack!");
            return;
        }

        if (hackButton != null)
            hackButton.SetActive(true);
    }

    void OnPlayerExit()
    {
        if (hackButton != null)
            hackButton.SetActive(false);
    }

    public void StartHack()
    {
        if (isCompleted) return;

        if (requirePhone && (PlayerInventory.Instance == null || !PlayerInventory.Instance.hasPhone))
        {
            Debug.Log("❌ Cannot hack without phone!");
            return;
        }

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

        if (triggerZone != null)
            triggerZone.enabled = false;
    }
}