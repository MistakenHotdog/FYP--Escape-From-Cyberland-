using UnityEngine;

public class BugCollide : MonoBehaviour
{
    public ErrorPopupManager popupManager;

    [Header("Logging")]
    public float triggerCooldown = 1f;

    private float lastTriggerTime = -999f;

    private void TryTriggerBug()
    {
        if (Time.time - lastTriggerTime < triggerCooldown)
            return;

        lastTriggerTime = Time.time;

        if (GameplaySessionLogger.Instance != null)
            GameplaySessionLogger.Instance.RegisterBugTriggered();

        if (popupManager != null)
            popupManager.TriggerBugPopup();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TryTriggerBug();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TryTriggerBug();
        }
    }
}