using UnityEngine;

public class DoorwayLaser : MonoBehaviour
{
    public bool isOn = true;
    public AudioClip dangerSound;
    public AudioSource audioSource;

    void Awake()
    {
        // Ensure parent has AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound
    }

    public void ToggleLaser()
    {
        isOn = !isOn;

        // Enable/disable all child cube renderers and colliders
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = isOn;

        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = isOn;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isOn) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by doorway laser!");

            // 🚨 TRIGGER ALARM
            FindObjectOfType<AlarmSystem>()?.TriggerAlarm();

            if (audioSource != null && dangerSound != null)
                audioSource.PlayOneShot(dangerSound);
        }
    }
}
