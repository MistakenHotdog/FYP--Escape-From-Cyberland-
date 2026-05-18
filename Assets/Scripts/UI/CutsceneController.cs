using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT
using TMPro;
#endif

[DisallowMultipleComponent]
public class CutsceneController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform cameraTransform;     // Main camera transform (drag Camera.main.transform)
    public Transform cutsceneCamPos;      // Cinematic camera target transform
    [Tooltip("Higher = faster camera movement")]
    public float camMoveSpeed = 2f;

    [Header("Player / Animation")]
    public Animator playerAnimator;
    [Tooltip("Animator trigger name to play the cutscene animation")]
    public string sitTriggerName = "Sit";

    [Header("Cutscene Timing")]
    [Tooltip("Duration that the cutscene runs (also slider fill duration).")]
    public float cutsceneDuration = 3f;
    [Tooltip("Optional small delay after reaching camera position before starting slider/animation.")]
    public float preStartDelay = 0f;

    [Header("UI - Slider")]
    [Tooltip("UI Slider (min=0, max=1). Should be inactive in inspector or hierarchy by default).")]
    public Slider timerSlider;
    public bool showSlider = true;

    [Header("UI - Panel & Text")]
    [Tooltip("Panel GameObject (should contain text). Should be inactive by default in inspector).")]
    public GameObject textPanelGO;
    [Tooltip("CanvasGroup used to fade the panel (will add one if missing).")]
    public CanvasGroup panelCanvasGroup;
    [Tooltip("Fade duration for the panel in/out.")]
    public float panelFadeDuration = 0.25f;
    [TextArea] public string panelText = "Cutscene text...";

    [Header("Hack Position")]
    public Transform hackPoint;
    public Transform lookTarget;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool isHacking = false;

    [Header("Optional text components")]
    public Text legacyUIText;
#if TMP_PRESENT
    public TextMeshProUGUI tmpText;
#endif

    [Header("Gameplay control")]
    public MonoBehaviour[] disableDuringCutscene; // movement, input, camera look, etc.

    [Header("Behaviour")]
    public bool returnCamera = true;

    // internal
    bool isCutscene = false;
    Coroutine runningCoroutine;

    void Reset()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Awake()
    {
        // Ensure panel CanvasGroup exists (if panel assigned)
        if (textPanelGO != null && panelCanvasGroup == null)
        {
            panelCanvasGroup = textPanelGO.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
                panelCanvasGroup = textPanelGO.AddComponent<CanvasGroup>();
        }

        // Ensure UI are hidden at start
        if (timerSlider != null)
        {
            timerSlider.minValue = 0f;
            timerSlider.maxValue = 1f;
            timerSlider.value = 0f;
            timerSlider.gameObject.SetActive(false); // hidden until cutscene begins
        }

        if (textPanelGO != null)
        {
            textPanelGO.SetActive(false); // hidden until cutscene begins
            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.alpha = 0f;
                panelCanvasGroup.interactable = false;
                panelCanvasGroup.blocksRaycasts = false;
            }
        }
    }

    /// <summary>Public: call this to play the cutscene (hook to your button).</summary>
    public void PlayCutscene()
    {
        if (isCutscene) return;
        runningCoroutine = StartCoroutine(CutsceneSequence());
    }

    public void CancelCutscene()
    {
        if (!isCutscene) return;
        if (runningCoroutine != null) StopCoroutine(runningCoroutine);
        CleanupAfterCutscene(true);
    }

    IEnumerator CutsceneSequence()
    {
        isCutscene = true;

        // Disable gameplay scripts
        foreach (var mb in disableDuringCutscene)
            if (mb != null) mb.enabled = false;

        // Save camera start transform
        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;

        // Move camera to cutscene position (if provided)
        if (cutsceneCamPos != null)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime * camMoveSpeed;
                cameraTransform.position = Vector3.Lerp(startPos, cutsceneCamPos.position, Mathf.SmoothStep(0f, 1f, t));
                cameraTransform.rotation = Quaternion.Slerp(startRot, cutsceneCamPos.rotation, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
        }

        // Optional small delay before starting UI/animation
        if (preStartDelay > 0f)
            yield return new WaitForSecondsRealtime(preStartDelay);

        // --- Start: show panel (fade in) and show slider ---
        if (textPanelGO != null)
        {
            // Set text if provided
            if (legacyUIText != null) legacyUIText.text = panelText;
#if TMP_PRESENT
            if (tmpText != null) tmpText.text = panelText;
#endif

            textPanelGO.SetActive(true);
            if (panelCanvasGroup != null)
                yield return StartCoroutine(FadeCanvasGroup(panelCanvasGroup, panelCanvasGroup.alpha, 1f, panelFadeDuration, true));
        }

        if (timerSlider != null && showSlider)
        {
            timerSlider.gameObject.SetActive(true);
            timerSlider.value = 0f;
        }

        // Trigger player animation
        if (playerAnimator != null && !string.IsNullOrEmpty(sitTriggerName))
            playerAnimator.SetBool("IsHacking", true);

        // Fill the slider over cutsceneDuration (unscaled time)
        float elapsed = 0f;
        while (elapsed < cutsceneDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float norm = Mathf.Clamp01(elapsed / Mathf.Max(0.0001f, cutsceneDuration));
            if (timerSlider != null && showSlider) timerSlider.value = norm;
            yield return null;
        }

        // --- End of cutscene: fade out panel and hide slider ---
        if (panelCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(panelCanvasGroup, panelCanvasGroup.alpha, 0f, panelFadeDuration, false));
        }
        if (textPanelGO != null)
        {
            textPanelGO.SetActive(false);
        }

        if (timerSlider != null)
        {
            timerSlider.value = 0f;
            timerSlider.gameObject.SetActive(false);
        }

        // Return camera
        if (returnCamera && cutsceneCamPos != null)
        {
            float t = 0f;
            Vector3 camFromPos = cameraTransform.position;
            Quaternion camFromRot = cameraTransform.rotation;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime * camMoveSpeed;
                cameraTransform.position = Vector3.Lerp(camFromPos, startPos, Mathf.SmoothStep(0f, 1f, t));
                cameraTransform.rotation = Quaternion.Slerp(camFromRot, startRot, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
        }

        // Re-enable gameplay scripts
        foreach (var mb in disableDuringCutscene)
            if (mb != null) mb.enabled = true;

        isCutscene = false;
        runningCoroutine = null;
    }
    public void StartHackSequence()
    {
        if (isHacking) return;

        isHacking = true;

        // Save original position
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Teleport to hack point
        transform.position = hackPoint.position;

        // Face correct direction
        if (lookTarget != null)
        {
            Vector3 dir = lookTarget.position - transform.position;
            dir.y = 0;
            transform.forward = dir;
        }

        // Disable movement scripts
        foreach (var mb in disableDuringCutscene)
            if (mb != null) mb.enabled = false;

        // Start animation
        playerAnimator.SetBool("IsHacking", true);
        Debug.Log("HACK STARTED");
    }

    public void StopHackSequence()
    {
        if (!isHacking) return;

        isHacking = false;

        // Stop animation instantly
        playerAnimator.SetBool("IsHacking", false);

        // Return player to original position
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        // Re-enable movement
        foreach (var mb in disableDuringCutscene)
            if (mb != null) mb.enabled = true;
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration, bool enableInteractOnComplete)
    {
        if (cg == null)
            yield break;

        float elapsed = 0f;
        float dur = Mathf.Max(0.0001f, duration);
        while (elapsed < dur)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / dur));
            yield return null;
        }
        cg.alpha = to;
        cg.interactable = enableInteractOnComplete && to > 0.5f;
        cg.blocksRaycasts = cg.interactable;
    }

    void CleanupAfterCutscene(bool forceReset = false)
    {
        if (timerSlider != null)
        {
            timerSlider.value = 0f;
            timerSlider.gameObject.SetActive(false);
        }

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }

        if (textPanelGO != null && forceReset)
            textPanelGO.SetActive(false);

        foreach (var mb in disableDuringCutscene)
            if (mb != null) mb.enabled = true;

        isCutscene = false;
        runningCoroutine = null;
    }
}
