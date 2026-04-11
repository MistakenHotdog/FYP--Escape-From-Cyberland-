using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class TriggerShowButton : MonoBehaviour
{
    [Header("UI")]
    public GameObject actionButtonGO;
    public float fadeDuration = 0.15f;

    [Header("Cutscene / Hack")]
    public MonoBehaviour cutsceneControllerScript;
    public HackWireManager hackManager;

    public bool hideAfterPress = true;

    private Button _button;
    private CanvasGroup _canvasGroup;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Awake()
    {
        if (actionButtonGO == null)
        {
            Debug.LogWarning("[TriggerShowButton] actionButtonGO not assigned.");
            return;
        }

        _canvasGroup = actionButtonGO.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = actionButtonGO.AddComponent<CanvasGroup>();

        _button = actionButtonGO.GetComponent<Button>();

        if (_button != null)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnActionButtonPressed);
        }

        actionButtonGO.SetActive(false);
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // ❌ DO NOT SHOW if already hacked
        if (hackManager != null && hackManager.IsHackCompleted())
            return;

        ShowButton();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        HideButton();
    }

    private void ShowButton()
    {
        if (actionButtonGO == null) return;

        actionButtonGO.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeCanvasGroup(_canvasGroup, _canvasGroup.alpha, 1f, fadeDuration, true));
    }

    private void HideButton()
    {
        if (actionButtonGO == null) return;

        StopAllCoroutines();
        StartCoroutine(FadeCanvasGroup(_canvasGroup, _canvasGroup.alpha, 0f, fadeDuration, false));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration, bool enableInteractOnComplete)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / Mathf.Max(0.0001f, duration));
            cg.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        cg.alpha = to;
        cg.interactable = enableInteractOnComplete && to > 0.5f;
        cg.blocksRaycasts = cg.interactable;

        if (!cg.interactable)
            actionButtonGO.SetActive(false);
    }

    private void OnActionButtonPressed()
    {
        // ❌ BLOCK PRESS if already hacked
        if (hackManager != null && hackManager.IsHackCompleted())
            return;

        if (cutsceneControllerScript != null)
        {
            var method = cutsceneControllerScript.GetType().GetMethod("PlayCutscene");
            if (method != null)
                method.Invoke(cutsceneControllerScript, null);
        }

        if (hideAfterPress)
            HideButton();
    }

    void OnDisable()
    {
        if (actionButtonGO != null)
            actionButtonGO.SetActive(false);
    }
}