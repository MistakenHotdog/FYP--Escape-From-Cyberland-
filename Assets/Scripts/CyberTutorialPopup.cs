using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class CyberTutorialPopup : MonoBehaviour
{
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;
    public float typeSpeed = 0.03f;
    public float displayTimeAfterTyping = 1f;
    public float fadeDuration = 0.5f;

    private string fullText =
        "Cyber Threat Guide\n" +
        "\u25aa Avoid drone lasers.\n" +
        "\u25aa Stay out of camera vision.\n" +
        "\u25aa Watch for phishing traps.\n" +
        "\u25aa Enemies attack on sight.\n" +
        "\u25aa Virus bugs slow you down.";

    private CanvasGroup canvasGroup;
    private bool skipRequested = false;
    private float skipEnabledTime;

    void Awake()
    {
        if (tutorialPanel == null)
        {
            Debug.LogWarning("[CyberTutorialPopup] Missing tutorialPanel.");
            return;
        }

        canvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = tutorialPanel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        tutorialPanel.SetActive(true);
    }

    void Start()
    {
        if (tutorialPanel == null || tutorialText == null)
        {
            Debug.LogWarning("[CyberTutorialPopup] Missing tutorialPanel or tutorialText.");
            return;
        }
        StartCoroutine(ShowTutorial());
    }

    void Update()
    {
        if (!skipRequested && Time.time > skipEnabledTime)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
                skipRequested = true;
        }
    }

    IEnumerator ShowTutorial()
    {
        skipEnabledTime = Time.time + 0.5f;

        // Fade In
        yield return StartCoroutine(FadeCanvas(0f, 1f));

        // Typewriter effect using maxVisibleCharacters
        tutorialText.text = fullText;
        tutorialText.maxVisibleCharacters = 0;
        for (int i = 0; i <= fullText.Length; i++)
        {
            if (skipRequested) break;
            tutorialText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typeSpeed);
        }
        tutorialText.maxVisibleCharacters = fullText.Length;

        // Wait extra time after typing (skippable)
        float waited = 0f;
        while (waited < displayTimeAfterTyping && !skipRequested)
        {
            waited += Time.deltaTime;
            yield return null;
        }

        // Fade Out
        yield return StartCoroutine(FadeCanvas(1f, 0f));

        tutorialPanel.SetActive(false);
    }

    IEnumerator FadeCanvas(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (canvasGroup != null)
            canvasGroup.alpha = to;
    }
}
