using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class TriggerShowButton : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("The Button GameObject (set inactive by default in inspector).")]
    public GameObject actionButtonGO; // whole Button GameObject (so we can activate/deactivate)
    [Tooltip("Optional: fade duration when showing/hiding")]
    public float fadeDuration = 0.15f;

    [Header("Cutscene")]
    [Tooltip("Optional: if left empty, script will try to find a CutsceneController in scene.")]
    public MonoBehaviour cutsceneControllerScript; // drag your CutsceneController component here

    [Tooltip("If this is true, the button will auto-hide after pressing once.")]
    public bool hideAfterPress = true;

    // private
    private Button _button;
    private CanvasGroup _canvasGroup;
    private bool _playerInside = false;

    void Reset()
    {
        // make sure collider is a trigger by default
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

        // Ensure there's a CanvasGroup for fading; add if missing.
        _canvasGroup = actionButtonGO.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = actionButtonGO.AddComponent<CanvasGroup>();

        // grab Button component if present
        _button = actionButtonGO.GetComponent<Button>();
        if (_button == null)
            Debug.LogWarning("[TriggerShowButton] No Button component found on actionButtonGO.");

        // Wire up click: either call PlayCutscene via reflection or by calling the public method if we have the component
        if (_button != null)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnActionButtonPressed);
        }

        // keep button hidden initially
        actionButtonGO.SetActive(false);
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = true;
        ShowButton();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;
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

        if (!cg.interactable) // fully hidden -> deactivate the GO to keep hierarchy tidy
        {
            actionButtonGO.SetActive(false);
        }
    }

    private void OnActionButtonPressed()
    {
        // call PlayCutscene on the CutsceneController if available
        if (cutsceneControllerScript != null)
        {
            // try to call PlayCutscene() using a common pattern:
            var method = cutsceneControllerScript.GetType().GetMethod("PlayCutscene");
            if (method != null)
            {
                method.Invoke(cutsceneControllerScript, null);
            }
            else
            {
                Debug.LogWarning("[TriggerShowButton] assigned script does not have PlayCutscene() method.");
            }
        }
        else
        {
            // Try to auto-find a CutsceneController component in the scene
            var found = FindObjectOfType<MonoBehaviour>();
            if (found != null)
            {
                var type = found.GetType();
                var method = type.GetMethod("PlayCutscene");
                if (method != null)
                {
                    method.Invoke(found, null);
                }
                else
                {
                    Debug.LogWarning("[TriggerShowButton] Could not find a CutsceneController with PlayCutscene(). Please assign cutsceneControllerScript.");
                }
            }
            else
            {
                Debug.LogWarning("[TriggerShowButton] No script assigned and none auto-found.");
            }
        }

        if (hideAfterPress)
        {
            HideButton();
        }
    }

    // Optional: in case player is still inside but you disable/enable the button externally
    void OnDisable()
    {
        if (actionButtonGO != null)
            actionButtonGO.SetActive(false);
    }
}
