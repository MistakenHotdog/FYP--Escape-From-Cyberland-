using UnityEngine;

public class DoorwayLaser : MonoBehaviour
{
    public bool isOn = true;
    public AudioClip dangerSound;
    public AudioSource audioSource;

    [Header("Damage")]
    public float contactDamage = 15f;

    private AlarmSystem cachedAlarm;
    private Renderer[] childRenderers;
    private Collider[] childColliders;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;

        childRenderers = GetComponentsInChildren<Renderer>();
        childColliders = GetComponentsInChildren<Collider>();
    }

    void Start()
    {
        cachedAlarm = FindObjectOfType<AlarmSystem>();
    }

    public void ToggleLaser()
    {
        isOn = !isOn;

        if (childRenderers != null)
            foreach (Renderer r in childRenderers)
                if (r != null) r.enabled = isOn;

        if (childColliders != null)
            foreach (Collider c in childColliders)
                if (c != null) c.enabled = isOn;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isOn) return;

        if (other.CompareTag("Player"))
        {
            if (cachedAlarm != null)
                cachedAlarm.TriggerAlarm();

            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(contactDamage);

            if (audioSource != null && dangerSound != null)
                audioSource.PlayOneShot(dangerSound);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isOn) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(contactDamage * Time.deltaTime);
        }
    }
}
