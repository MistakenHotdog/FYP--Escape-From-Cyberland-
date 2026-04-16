using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // 🔥 IMPORTANT

public class FakeRewardPopupManager : MonoBehaviour
{
    [Header("Popup UI")]
    public GameObject popupPanel;
    public Button claimButton;
    public Button closeButton;
    public TMP_Text messageText; // ✅ TMP

    [Header("Feedback UI")]
    public GameObject feedbackTextGO;
    public TMP_Text feedbackText; // ✅ TMP
    public float feedbackDuration = 1.5f;

    [Header("Gameplay")]
    public float damageAmount = 15f;

    private PlayerHealth playerHealth;
    private bool isActive = false;

    [Header("Messages")]
    public string[] fakeMessages =
    {
        "🎁 Free Reward! Click to Claim",
        "💰 You received 100 Credits!",
        "⚡ Limited Offer! Claim Now!",
        "⚠ Unauthorized Bonus Detected",
        "🔓 Hidden Reward Unlocked!"
    };

    void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (feedbackTextGO != null)
            feedbackTextGO.SetActive(false);

        playerHealth = FindObjectOfType<PlayerHealth>();

        if (claimButton != null)
            claimButton.onClick.AddListener(OnClaimClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);
    }

    public void TriggerFakePopup()
    {
        if (isActive) return;

        isActive = true;

        if (messageText != null && fakeMessages.Length > 0)
        {
            messageText.text = fakeMessages[Random.Range(0, fakeMessages.Length)];
        }

        popupPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void OnClaimClicked()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }

        ClosePopup(); // close first

        ShowFeedback("You got scammed!\n-" + damageAmount + " Health");
    }

    void OnCloseClicked()
    {
        ClosePopup();
    }

    void ClosePopup()
    {
        popupPanel.SetActive(false);
        isActive = false;
        Time.timeScale = 1f;
    }

    void ShowFeedback(string message)
    {
        if (feedbackTextGO == null || feedbackText == null) return;

        feedbackText.text = message;
        feedbackTextGO.SetActive(true);

        StartCoroutine(HideFeedback());
    }

    IEnumerator HideFeedback()
    {
        yield return new WaitForSecondsRealtime(feedbackDuration);
        feedbackTextGO.SetActive(false);
    }
}