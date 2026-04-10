using UnityEngine;
using System.Collections;

public class FlyingBugEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float waveFrequency = 5f;
    public float waveAmplitude = 0.5f;

    [Header("Hit Effect")]
    public float slowAmount = 0.35f;
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
            Debug.LogWarning($"[FlyingBugEnemy] {name}: No PlayerEffectsController found.");
    }

    void Update()
    {
        if (player == null || isDying) return;

        // Weaving movement toward player — wave fades out on final approach
        Vector3 basePos = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        Vector3 toPlayer = (player.position - transform.position);
        float dist = toPlayer.magnitude;
        if (dist > 1.5f)
        {
            Vector3 right = Vector3.Cross(Vector3.up, toPlayer.normalized).normalized;
            float waveFade = Mathf.Clamp01((dist - 1.5f) / 3f);
            float wave = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude * waveFade;
            transform.position = basePos + right * wave;
        }
        else
        {
            transform.position = basePos;
        }

        transform.LookAt(player);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDying) return;
        if (!other.CompareTag("Player")) return;

        if (effectsController != null)
            effectsController.ApplyHitEffect(slowAmount, effectDuration);

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
