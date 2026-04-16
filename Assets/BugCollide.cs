using UnityEngine;

public class BugCollide : MonoBehaviour
{
    public FakeRewardPopupManager fakePopupManager;

    [Header("Cooldown")]
    public float triggerCooldown = 1f;

    private float lastTriggerTime = -999f;

    private void TryTriggerBug()
    {
        if (Time.time - lastTriggerTime < triggerCooldown)
            return;

        lastTriggerTime = Time.time;

        if (fakePopupManager != null)
            fakePopupManager.TriggerFakePopup();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            TryTriggerBug();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            TryTriggerBug();
    }
}