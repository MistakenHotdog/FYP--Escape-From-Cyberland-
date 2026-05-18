using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EventButtonController : MonoBehaviour
{
    [Header("Animation & UI")]
    public Animator animator;                 // Animator that plays the event animation
    public string animatorBoolName = "IsPlayingEvent";
    public Slider timerSlider;                // UI Slider used as timer bar
    public float duration = 3f;               // Time it takes to fill

    [Header("Gameplay control (optional)")]
    public MonoBehaviour[] disableDuringEvent;

    private Coroutine runningCoroutine;
    private bool isRunning = false;

    public void StartEvent()
    {
        if (isRunning) return;
        runningCoroutine = StartCoroutine(RunEventCoroutine());
    }

    public void CancelEvent()  // optional
    {
        if (!isRunning) return;
        if (runningCoroutine != null) StopCoroutine(runningCoroutine);
        EndEventCleanup();
    }

    private IEnumerator RunEventCoroutine()
    {
        isRunning = true;

        foreach (var mb in disableDuringEvent)
            if (mb != null) mb.enabled = false;

        if (animator != null)
            animator.SetBool(animatorBoolName, true);

        // Reset slider
        if (timerSlider != null)
            timerSlider.value = 0f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            if (timerSlider != null)
                timerSlider.value = Mathf.Clamp01(elapsed / duration);

            yield return null;
        }

        EndEventCleanup();
    }

    private void EndEventCleanup()
    {
        if (animator != null)
            animator.SetBool(animatorBoolName, false);

        if (timerSlider != null)
            timerSlider.value = 0f;

        foreach (var mb in disableDuringEvent)
            if (mb != null) mb.enabled = true;

        isRunning = false;
        runningCoroutine = null;
    }
}
