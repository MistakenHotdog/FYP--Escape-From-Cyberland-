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
    private Graphic[] graphics;

    private void Start()
    {
        graphics = panel.GetComponentsInChildren<Graphic>(true);

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

            if (GameplaySessionLogger.Instance != null)
            {
                GameplaySessionLogger.Instance.EndSession(true);
            }

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