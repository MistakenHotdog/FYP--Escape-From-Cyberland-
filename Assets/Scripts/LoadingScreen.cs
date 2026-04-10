using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("The name of the scene to load after this screen.")]
    public string sceneToLoad = SceneNames.GameScene1;

    [Header("UI References")]
    public Slider progressBar;      // Slider UI element
    public Text loadingText;        // Legacy Text UI element
    public CanvasGroup fadePanel;   // CanvasGroup attached to a black Image for fade

    [Header("Fade Settings")]
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;

    private void Start()
    {
        StartCoroutine(HandleLoading());
    }

    IEnumerator HandleLoading()
    {
        // Step 1: Fade in from black
        yield return StartCoroutine(Fade(1, 0, fadeInTime));

        // Step 2: Begin async scene loading
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null) progressBar.value = progress;
            if (loadingText != null) loadingText.text = (progress * 100f).ToString("F0") + "%";

            // When ready (almost done)
            if (progress >= 1f)
            {
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(Fade(0, 1, fadeOutTime));  // Fade out to black
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    IEnumerator Fade(float start, float end, float duration)
    {
        if (fadePanel == null) yield break;
        float elapsed = 0f;
        fadePanel.alpha = start;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        fadePanel.alpha = end;
    }
}
