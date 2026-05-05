using UnityEngine;

public class KeycardPickup : MonoBehaviour
{
    public string keycardID = "Level2Card";
    public GameObject takeButton;

    private bool playerNearby = false;

    private void Start()
    {
        if (takeButton != null)
            takeButton.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = true;

        if (takeButton != null)
            takeButton.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;

        if (takeButton != null)
            takeButton.SetActive(false);
    }

    public void TakeCard()
    {
        if (!playerNearby) return;

        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.AddKeycard(keycardID);

        if (takeButton != null)
            takeButton.SetActive(false);

        gameObject.SetActive(false);

        Debug.Log("🪪 Card Picked!");
    }
}