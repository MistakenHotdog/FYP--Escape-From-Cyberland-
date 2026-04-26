using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TriggerPanelPopup : MonoBehaviour
{
    [Header("Victory Panel")]
    public GameObject panel;
    public float fadeDuration = 1f;

    [Header("Scene Flow")]
    public string continueScene = SceneNames.LoadingScene;
    public string mainMenuScene = SceneNames.MainMenu;
    public string level2Scene = SceneNames.Level2;

    [Header("Escape Gate")]
    [Tooltip("If true, the escape panel only appears after the hack has been completed.")]
    public bool requireHackCompleted = true;

    private bool hasTriggered = false;
    private Graphic[] graphics;
    private HackWireManager hackManager;

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

        // Cache the hack manager once so we don't search the scene on every trigger.
        hackManager = FindObjectOfType<HackWireManager>();

        if (requireHackCompleted && hackManager == null)
        {
            Debug.LogWarning("[TriggerPanelPopup] requireHackCompleted is true but no HackWireManager was found in the scene. Escape will be allowed without a hack.");
        }
        else
        {
            Debug.Log($"[TriggerPanelPopup] Escape zone '{name}' ready. requireHackCompleted={requireHackCompleted}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasTriggered) return;

        // Gate: escape only allowed after the hack is complete.
        if (requireHackCompleted && hackManager != null && !hackManager.IsHackCompleted())
        {
            Debug.Log("[TriggerPanelPopup] Player reached escape zone, but hack is NOT completed yet — blocked.");
            return;
        }

        hasTriggered = true;
        Debug.Log("[TriggerPanelPopup] Escape conditions met — player reached exit! Showing victory panel.");

        if (GameplaySessionLogger.Instance != null)
        {
            GameplaySessionLogger.Instance.EndSession(true);
            Debug.Log("[TriggerPanelPopup] GameplaySession ended with success=true.");
        }

        panel.SetActive(true);
        StartCoroutine(FadeInPanel());
    }

    private IEnumerator FadeInPanel()
    {
        Debug.Log("[TriggerPanelPopup] Fading in victory panel...");

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

        Debug.Log("[TriggerPanelPopup] Victory panel fully visible.");
    }

    public void LoadContinue()
    {
        Debug.Log($"[TriggerPanelPopup] Loading continue scene: {continueScene}");
        SceneManager.LoadScene(continueScene);
    }

    public void LoadMainMenu()
    {
        Debug.Log($"[TriggerPanelPopup] Loading main menu scene: {mainMenuScene}");
        SceneManager.LoadScene(mainMenuScene);
    }

    public void LoadLevel2()
    {
        Debug.Log($"[TriggerPanelPopup] Loading Level 2 scene: {level2Scene}");
        SceneManager.LoadScene(level2Scene);
    }
}
