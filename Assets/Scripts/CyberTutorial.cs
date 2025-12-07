using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CyberTutorial : MonoBehaviour
{
    public CanvasGroup panelCanvas;      // CanvasGroup for fade
    public TMP_Text tutorialText;        // TMP Text object

    [TextArea]
    public string fullText =
        "!!! ALERT: Enemy drones are active!\n" +
        "[!] Stay aware of viruses, malware, and phishing attacks.\n" +
        "[#] Hide behind objects.\n" +
        "[*] Use cyber tools wisely to disable enemies.";

    public float typingSpeed = 0.05f;    // Time between letters
    public float displayTime = 5f;       // Time panel stays after typing

    void Start()
    {
        StartCoroutine(ShowTutorial());
    }

    IEnumerator ShowTutorial()
    {
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

        // Typing effect
        tutorialText.text = "";
        foreach (char c in fullText)
        {
            tutorialText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Wait for displayTime
        yield return new WaitForSeconds(displayTime);

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
