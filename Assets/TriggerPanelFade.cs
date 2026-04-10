using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TriggerPanelPopup : MonoBehaviour
{
    public GameObject panel;
    public float fadeDuration = 1f;
    public string continueScene = SceneNames.LoadingScene;
    public string mainMenuScene = SceneNames.MainMenu;

    private bool hasTriggered = false;
    private Graphic[] graphics;   // All UI elements on the panel

    private void Start()
    {
        // Get all UI graphics (Image, Text, TMP, etc.)
        graphics = panel.GetComponentsInChildren<Graphic>(true);

        // Set them all invisible at start
        foreach (var g in graphics)
        {
            Color c = g.color;
            c.a = 0;
            g.color = c;
        }

        panel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            panel.SetActive(true);
            StartCoroutine(FadeInPanel());
        }
    }

    private IEnumerator FadeInPanel()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;

            foreach (var g in graphics)
            {
                Color c = g.color;
                c.a = alpha;
                g.color = c;
            }

            yield return null;
        }

        // Ensure fully visible
        foreach (var g in graphics)
        {
            Color c = g.color;
            c.a = 1;
            g.color = c;
        }
    }

    public void LoadContinue()
    {
        SceneManager.LoadScene(continueScene);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
