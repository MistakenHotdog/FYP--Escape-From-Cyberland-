using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class TriggerShowButton : MonoBehaviour
{
    private const string UI_TYPE_KEY = "UIType";
    private const int JOYSTICK_UI = 1;
    private const int BUTTON_UI = 2;
    private const int VOICE_UI = 3;

    [Header("UI")]
    public GameObject actionButtonGO;
    public GameObject voicePromptGO;   // optional parent object for voice text
    public TMP_Text voicePromptText;   // optional text field
    public float fadeDuration = 0.15f;

    [Header("Cutscene / Hack")]
    public MonoBehaviour cutsceneControllerScript;
    public HackWireManager hackManager;

    public bool hideAfterPress = true;

    private Button _button;
    private CanvasGroup _canvasGroup;
    private CanvasGroup _voiceCanvasGroup;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Awake()
    {
        if (actionButtonGO != null)
        {
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

        if (voicePromptGO != null)
        {
            _voiceCanvasGroup = voicePromptGO.GetComponent<CanvasGroup>();
            if (_voiceCanvasGroup == null)
                _voiceCanvasGroup = voicePromptGO.AddComponent<CanvasGroup>();

            voicePromptGO.SetActive(false);
            _voiceCanvasGroup.alpha = 0f;
            _voiceCanvasGroup.interactable = false;
            _voiceCanvasGroup.blocksRaycasts = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (hackManager != null && hackManager.IsHackCompleted())
            return;

        int uiType = PlayerPrefs.GetInt(UI_TYPE_KEY, JOYSTICK_UI);

        if (uiType == VOICE_UI)
        {
            ShowVoicePrompt();

            VoiceCommandManager vcm = FindObjectOfType<VoiceCommandManager>();
            if (vcm != null)
                vcm.SetHackAllowed(true, hackManager);
        }
        else
        {
            ShowButton();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        HideButton();
        HideVoicePrompt();

        VoiceCommandManager vcm = FindObjectOfType<VoiceCommandManager>();
        if (vcm != null)
            vcm.SetHackAllowed(false, null);
    }

    private void ShowButton()
    {
        if (actionButtonGO == null) return;

        actionButtonGO.SetActive(true);
        StopCoroutineSafe();
        StartCoroutine(FadeCanvasGroup(_canvasGroup, _canvasGroup.alpha, 1f, fadeDuration, true, actionButtonGO));
    }

    private void HideButton()
    {
        if (actionButtonGO == null || _canvasGroup == null) return;

        StopCoroutineSafe();
        StartCoroutine(FadeCanvasGroup(_canvasGroup, _canvasGroup.alpha, 0f, fadeDuration, false, actionButtonGO));
    }

    private void ShowVoicePrompt()
    {
        if (voicePromptGO == null) return;

        if (voicePromptText != null)
            voicePromptText.text = "Say \"hack\" to open panel";

        voicePromptGO.SetActive(true);
        StopCoroutineSafe();
        StartCoroutine(FadeCanvasGroup(_voiceCanvasGroup, _voiceCanvasGroup.alpha, 1f, fadeDuration, false, voicePromptGO));
    }

    private void HideVoicePrompt()
    {
        if (voicePromptGO == null || _voiceCanvasGroup == null) return;

        StopCoroutineSafe();
        StartCoroutine(FadeCanvasGroup(_voiceCanvasGroup, _voiceCanvasGroup.alpha, 0f, fadeDuration, false, voicePromptGO));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration, bool enableInteractOnComplete, GameObject targetGO)
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

        if (!cg.interactable && targetGO != null)
            targetGO.SetActive(false);
    }

    private void OnActionButtonPressed()
    {
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

    private void StopCoroutineSafe()
    {
        StopAllCoroutines();
    }

    void OnDisable()
    {
        if (actionButtonGO != null)
            actionButtonGO.SetActive(false);

        if (voicePromptGO != null)
            voicePromptGO.SetActive(false);

        VoiceCommandManager vcm = FindObjectOfType<VoiceCommandManager>();
        if (vcm != null)
            vcm.SetHackAllowed(false, null);
    }
}