using UnityEngine;

public class KeycardScannerTrigger : MonoBehaviour
{
    public GameObject scanButton;
    public DoorController door;
    public string requiredKeycardID = "Level2Card";

    private bool playerNearby = false;
    private bool isUnlocked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isUnlocked) return;

        playerNearby = true;

        if (scanButton != null)
            scanButton.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;

        if (scanButton != null)
            scanButton.SetActive(false);
    }

    public void ScanCard()
    {
        if (!playerNearby || isUnlocked) return;

        if (PlayerInventory.Instance != null &&
            PlayerInventory.Instance.HasKeycard(requiredKeycardID))
        {
            Debug.Log("✅ Access Granted");

            if (door != null)
                door.OpenDoor();

            isUnlocked = true;

            if (scanButton != null)
                scanButton.SetActive(false);

            GetComponent<Collider>().enabled = false;
        }
        else
        {
            Debug.Log("❌ Access Denied - Need Keycard");
        }
    }
}