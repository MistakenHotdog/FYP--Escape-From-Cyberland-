using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CyberTutorial : MonoBehaviour
{
    public CanvasGroup panelCanvas;
    public TMP_Text tutorialText;

    [TextArea]
    public string fullText =
        "!!! ALERT: Enemy drones are active!\n" +
        "[!] Stay aware of viruses, malware, and phishing attacks.\n" +
        "[#] Hide behind objects.\n" +
        "[*] Use cyber tools wisely to disable enemies.";

    public float typingSpeed = 0.05f;
    public float displayTime = 3f;

    private bool skipRequested = false;
    private float skipEnabledTime;

    void Start()
    {
        if (panelCanvas == null || tutorialText == null)
        {
            Debug.LogWarning("[CyberTutorial] Missing panelCanvas or tutorialText.");
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

        // Fade in
        panelCanvas.alpha = 0f;
        panelCanvas.gameObject.SetActive(true);
        float fadeInTime = 0.5f;
        for (float t = 0; t < fadeInTime; t += Time.deltaTime)
        {
            panelCanvas.alpha = t / fadeInTime;
            yield return null;
        }
        panelCanvas.alpha = 1f;

        // Typing effect using maxVisibleCharacters (no string allocation)
        tutorialText.text = fullText;
        tutorialText.maxVisibleCharacters = 0;
        for (int i = 0; i <= fullText.Length; i++)
        {
            if (skipRequested) break;
            tutorialText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }
        tutorialText.maxVisibleCharacters = fullText.Length;

        // Wait for displayTime (skippable)
        float waited = 0f;
        while (waited < displayTime && !skipRequested)
        {
            waited += Time.deltaTime;
            yield return null;
        }

        // Fade out
        float fadeOutTime = 0.5f;
        for (float t = 0; t < fadeOutTime; t += Time.deltaTime)
        {
            panelCanvas.alpha = 1 - t / fadeOutTime;
            yield return null;
        }
        panelCanvas.alpha = 0f;
        panelCanvas.gameObject.SetActive(false);
    }
}
