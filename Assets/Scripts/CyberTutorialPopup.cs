using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI; // For CanvasGroup

public class CyberTutorialPopup : MonoBehaviour
{
    public GameObject tutorialPanel;         // Panel
    public TextMeshProUGUI tutorialText;     // TMP Text
    public float typeSpeed = 0.03f;          // Typing speed
    public float displayTimeAfterTyping = 1f;// Extra time panel stays after typing
    public float fadeDuration = 0.5f;        // Fade in/out time

    private string fullText =
        "Cyber Threat Guide\n" +
        "• Avoid drone lasers.\n" +
        "• Stay out of camera vision.\n" +
        "• Watch for phishing traps.\n" +
        "• Enemies attack on sight.\n" +
        "• Virus bugs slow you down.";

    private CanvasGroup canvasGroup;

    void Awake()
    {
        // Add CanvasGroup if not present
        canvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = tutorialPanel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f; // start invisible
        tutorialPanel.SetActive(true); // needed for CanvasGroup to work
    }

    void Start()
    {
        StartCoroutine(ShowTutorial());
    }

    IEnumerator ShowTutorial()
    {
        // Fade In
        yield return StartCoroutine(FadeCanvas(0f, 1f));

        // Typewriter effect
        tutorialText.text = "";
        foreach (char c in fullText)
        {
            tutorialText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        // Wait extra time after typing
        yield return new WaitForSeconds(displayTimeAfterTyping);

        // Fade Out
        yield return StartCoroutine(FadeCanvas(1f, 0f));

        tutorialPanel.SetActive(false);
    }

    IEnumerator FadeCanvas(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
