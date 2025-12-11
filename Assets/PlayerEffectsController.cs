using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerEffectsController : MonoBehaviour
{
    [Header("References")]
    public PlayerMove playerMove;
    public Camera playerCamera;
    public PostProcessVolume postProcessVolume;

    [Header("PPv2 Settings")]
    public float vignetteIntensity = 0.45f;
    public float dofFocusDistance = 0.1f;
    public float dofAperture = 0.3f;
    public float dofFocalLength = 80f;

    [Header("Timing")]
    public float fadeDuration = 0.3f; // Adjustable in inspector for fade out

    private Vignette vignette;
    private DepthOfField depthOfField;

    private float defaultVignette = 0f;
    private float defaultFocusDistance = 10f;

    private void Awake()
    {
        // Auto-find playerMove if not assigned
        if (playerMove == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerMove = playerObj.GetComponent<PlayerMove>();
        }

        // Auto-find camera if not assigned
        if (playerCamera == null)
            playerCamera = Camera.main;

        // Find PostProcessVolume object named "PostProcessLayer"
        GameObject ppObj = GameObject.Find("PostProcessLayer");
        if (ppObj != null)
            postProcessVolume = ppObj.GetComponent<PostProcessVolume>();

        if (postProcessVolume != null)
        {
            // Start with weight 0 (effects OFF)
            postProcessVolume.weight = 0f;

            // Clone profile to avoid modifying original
            postProcessVolume.profile = Instantiate(postProcessVolume.sharedProfile);
            var profile = postProcessVolume.profile;

            // Vignette
            if (!profile.TryGetSettings(out vignette))
                vignette = profile.AddSettings<Vignette>();
            vignette.active = true;
            vignette.intensity.overrideState = true;
            vignette.intensity.value = vignetteIntensity;

            // Depth of Field
            if (!profile.TryGetSettings(out depthOfField))
                depthOfField = profile.AddSettings<DepthOfField>();
            depthOfField.active = true;
            depthOfField.focusDistance.overrideState = true;
            depthOfField.focusDistance.value = dofFocusDistance;
            depthOfField.aperture.overrideState = true;
            depthOfField.focalLength.overrideState = true;
            depthOfField.aperture.value = dofAperture;
            depthOfField.focalLength.value = dofFocalLength;

            postProcessVolume.priority = 100f;
        }
    }

    /// <summary>
    /// Call this from any enemy prefab on collision.
    /// </summary>
    public void ApplyHitEffect(float slowAmount, float duration)
    {
        StartCoroutine(ApplyHitEffectRoutine(slowAmount, duration));
    }

    private IEnumerator ApplyHitEffectRoutine(float slowAmount, float duration)
    {
        // 1. Slow player
        if (playerMove != null)
            playerMove.speedMultiplier = slowAmount;

        // 2. Turn on PPv2 effects by setting weight to 1
        if (postProcessVolume != null)
            postProcessVolume.weight = 1f;

        // 3. Keep for duration
        yield return new WaitForSeconds(duration);

        // 4. Fade out weight smoothly using inspector-adjustable fadeDuration
        if (postProcessVolume != null)
        {
            float t = 0f;
            float startWeight = postProcessVolume.weight;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                postProcessVolume.weight = Mathf.Lerp(startWeight, 0f, t / fadeDuration);
                yield return null;
            }

            postProcessVolume.weight = 0f;
        }

        // 5. Restore player speed
        if (playerMove != null)
            playerMove.speedMultiplier = 1f;
    }
}
