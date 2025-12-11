using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public Slider healthBar;
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI Reference")]
    public TerminationUI terminationUI;   // Assign your game-over UI script here

    [Header("Damage Feedback")]
    public Image damageOverlay;           // Fullscreen Image (UI) – alpha starts at 0
    public float damageFlashDuration = 0.25f;
    [Range(0f, 1f)] public float damageOverlayMaxAlpha = 0.6f;

    [Header("Audio")]
    public AudioClip hitSFX;
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        // Ensure AudioSource exists
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound

        // Setup overlay so it never blocks touches
        if (damageOverlay != null)
        {
            SetOverlayAlpha(0f);
            damageOverlay.raycastTarget = false;

            // Guarantee overlay does not block joystick
            CanvasGroup cg = damageOverlay.GetComponent<CanvasGroup>();
            if (cg == null) cg = damageOverlay.gameObject.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;
        }
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthBar != null)
            healthBar.value = currentHealth;

        // Show red flash
        if (damageOverlay != null)
        {
            StopCoroutine("DamageFlashCoroutine");
            SetOverlayAlpha(damageOverlayMaxAlpha);
            StartCoroutine(DamageFlashCoroutine());
        }

        // Play hit SFX
        if (hitSFX != null && audioSource != null)
            audioSource.PlayOneShot(hitSFX);

        // Check death
        if (currentHealth <= 0f)
            Die();
    }

    IEnumerator DamageFlashCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < damageFlashDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(damageOverlayMaxAlpha, 0f, elapsed / damageFlashDuration);
            SetOverlayAlpha(alpha);
            yield return null;
        }

        SetOverlayAlpha(0f);
    }

    void SetOverlayAlpha(float a)
    {
        if (damageOverlay == null) return;
        Color c = damageOverlay.color;
        c.a = Mathf.Clamp01(a);
        damageOverlay.color = c;
    }

    void Die()
    {
        Debug.Log("Player Died");

        // Show Game Over UI (simple, direct call)
        if (terminationUI != null)
            terminationUI.ShowGameOver();

        // Pause game AFTER showing UI
        Time.timeScale = 0f;
    }
}
