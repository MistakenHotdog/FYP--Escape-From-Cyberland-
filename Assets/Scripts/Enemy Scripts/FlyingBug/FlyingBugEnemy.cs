using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FlyingBugEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("Slow Motion")]
    public float slowDuration = 1.2f;
    public float slowAmount = 0.35f;

    [Header("Camera Shake")]
    public float shakeDuration = 3.5f;         // 3–4 seconds
    public float shakeMagnitude = 0.05f;       // small positional shake
    public float rotationMagnitude = 12f;      // strong left-right rotation wobble

    [Header("Post Processing (Optional)")]
    public PostProcessVolume postProcessVolume;
    private Vignette vignette;
    private DepthOfField depthOfField;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Auto-find PostProcessVolume if not assigned
        if (postProcessVolume == null)
            postProcessVolume = FindObjectOfType<PostProcessVolume>();

        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGetSettings(out vignette);
            postProcessVolume.profile.TryGetSettings(out depthOfField);

            if (vignette != null)
                vignette.intensity.value = 0f;

            if (depthOfField != null)
                depthOfField.focusDistance.value = 10f;
        }
    }

    void Update()
    {
        if (!player) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );

        transform.LookAt(player);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Start slow motion
        StartCoroutine(SlowDownEffect());

        // Start post-processing effects if available
        if (postProcessVolume != null)
            StartCoroutine(PostProcessingEffect());

        // Camera shake (falling left-right)
        CameraShake shake = Camera.main.GetComponent<CameraShake>();
        if (shake != null)
            StartCoroutine(shake.FallingShake(shakeDuration, shakeMagnitude, rotationMagnitude));

        // Destroy the bug after hitting player
        Destroy(gameObject);
    }

    private IEnumerator SlowDownEffect()
    {
        Time.timeScale = slowAmount;
        Time.fixedDeltaTime = 0.02f * slowAmount;

        yield return new WaitForSecondsRealtime(slowDuration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    private IEnumerator PostProcessingEffect()
    {
        float elapsed = 0f;
        float maxVignette = 0.45f;
        float blurFocus = 0.1f;

        // Apply strong effects immediately
        if (vignette != null) vignette.intensity.value = maxVignette;
        if (depthOfField != null) depthOfField.focusDistance.value = blurFocus;

        // Hold effect for slowDuration
        yield return new WaitForSecondsRealtime(slowDuration);

        // Smoothly fade out effects
        float fadeDuration = 0.3f;
        elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            if (vignette != null)
                vignette.intensity.value = Mathf.Lerp(maxVignette, 0f, elapsed / fadeDuration);

            if (depthOfField != null)
                depthOfField.focusDistance.value = Mathf.Lerp(blurFocus, 10f, elapsed / fadeDuration);

            yield return null;
        }

        if (vignette != null) vignette.intensity.value = 0f;
        if (depthOfField != null) depthOfField.focusDistance.value = 10f;
    }
}
