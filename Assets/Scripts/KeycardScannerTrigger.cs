using UnityEngine;

public class KeycardScannerTrigger : MonoBehaviour
{
    public GameObject scanButton;
    public DoorController door;

    [Header("Audio")]
    public AudioSource scanSound;

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
            PlayerInventory.Instance.hasLevel2Keycard)
        {
            Debug.Log("✅ Access Granted");

            // 🔊 Play scan sound
            if (scanSound != null)
            {
                scanSound.Play();
            }

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