using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Fallback scene if no level is selected.")]
    public string defaultScene = SceneNames.GameScene1;

    [Header("UI References")]
    public Slider progressBar;
    public Text loadingText;
    public CanvasGroup fadePanel;

    [Header("Fade Settings")]
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;

    private string sceneToLoad;

    private void Start()
    {
        // 🔥 GET SELECTED LEVEL
        sceneToLoad = PlayerPrefs.GetString("SelectedLevel", defaultScene);

        // Safety guard: never let the LoadingScene load itself (would cause an infinite loop).
        if (string.IsNullOrEmpty(sceneToLoad) || sceneToLoad == SceneNames.LoadingScene)
        {
            Debug.LogWarning($"[LoadingScreen] SelectedLevel was '{sceneToLoad}' which would loop. Falling back to '{defaultScene}'.");
            sceneToLoad = defaultScene;
            PlayerPrefs.SetString("SelectedLevel", defaultScene);
            PlayerPrefs.Save();
        }

        Debug.Log("[LoadingScreen] Loading: " + sceneToLoad);

        StartCoroutine(HandleLoading());
    }

    IEnumerator HandleLoading()
    {
        // Step 1: Fade in
        yield return StartCoroutine(Fade(1, 0, fadeInTime));

        // Step 2: Load selected scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (progressBar != null)
                progressBar.value = progress;

            if (loadingText != null)
                loadingText.text = (progress * 100f).ToString("F0") + "%";

            if (progress >= 1f)
            {
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(Fade(0, 1, fadeOutTime));
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