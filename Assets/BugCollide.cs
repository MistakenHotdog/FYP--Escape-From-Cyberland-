using UnityEngine;

public class BugCollide : MonoBehaviour
{
    public ErrorPopupManager popupManager;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && popupManager != null)
        {
            popupManager.TriggerBugPopup();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && popupManager != null)
        {
            popupManager.TriggerBugPopup();
        }
    }
}
