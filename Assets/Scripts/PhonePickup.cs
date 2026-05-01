using UnityEngine;

public class PhonePickup : MonoBehaviour
{
    [Header("UI")]
    public GameObject takeButton;     // "Take" button
    public GameObject phoneUIIcon;    // 📱 UI icon (bottom right)

    private void Start()
    {
        // Make sure UI starts hidden
        if (takeButton != null)
            takeButton.SetActive(false);

        if (phoneUIIcon != null)
            phoneUIIcon.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (takeButton != null)
                takeButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (takeButton != null)
                takeButton.SetActive(false);
        }
    }

    public void TakePhone()
    {
        // Give player the phone
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.hasPhone = true;

        // Hide take button
        if (takeButton != null)
            takeButton.SetActive(false);

        // 🔥 Show phone icon on UI
        if (phoneUIIcon != null)
            phoneUIIcon.SetActive(true);

        // Remove phone from world
        gameObject.SetActive(false);

        Debug.Log("📱 Phone Collected!");
    }
}