using UnityEngine;
using System.Collections;

public class FlyingBugEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float rotationSpeed = 5f;
    public float hoverAmplitude = 0.2f;
    public float hoverFrequency = 5f;

    [Header("Hit Effect")]
    public float slowAmount = 0.5f;     // 0.5 = 50% speed
    public float effectDuration = 1.2f;

    private PlayerEffectsController effectsController;
    private Transform player;
    private bool isDying = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        effectsController = FindObjectOfType<PlayerEffectsController>();

        if (effectsController == null)
            Debug.LogWarning("[FlyingBugEnemy] No PlayerEffectsController found.");
    }

    void Update()
    {
        if (player == null || isDying) return;

        // Direction toward player
        Vector3 direction = (player.position - transform.position).normalized;

        // Movement
        transform.position += direction * speed * Time.deltaTime;

        // Hover effect (optional polish)
        float hover = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        transform.position += new Vector3(0, hover * Time.deltaTime, 0);

        // Smooth rotation (NO snapping)
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDying) return;

        if (!other.CompareTag("Player")) return;

        // Apply slow effect
        if (effectsController != null)
        {
            effectsController.ApplyHitEffect(slowAmount, effectDuration);
        }

        // OPTIONAL: also damage player (recommended for FYP)
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(10f);
        }

        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        isDying = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        float t = 0f;
        Vector3 startScale = transform.localScale;

        while (t < 0.2f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t / 0.2f);
            yield return null;
        }

        Destroy(gameObject);
    }
}