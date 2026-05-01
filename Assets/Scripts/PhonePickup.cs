using UnityEngine;

public class PhonePickup : MonoBehaviour
{
    public GameObject takeButton;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            takeButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            takeButton.SetActive(false);
        }
    }

    public void TakePhone()
    {
        // Give player the phone
        PlayerInventory.Instance.hasPhone = true;

        // Hide button
        takeButton.SetActive(false);

        // Remove phone from world
        gameObject.SetActive(false);

        Debug.Log("📱 Phone Collected!");
    }
}