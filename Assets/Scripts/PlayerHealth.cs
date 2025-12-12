using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public Image damageFlashImage;
    public float flashDuration = 0.2f;
    private bool isFlashing = false;

    public Slider healthBar;
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Audio")]
    public AudioSource hitSoundSource;
    public AudioClip hitSound;

    [Header("UI Reference")]
    public TerminationUI terminationUI;   // <-- ADD THIS

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth;

        if (hitSoundSource != null && hitSound != null)
        {
            hitSoundSource.PlayOneShot(hitSound);
        }

        StartCoroutine(DamageFlash());


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        if (isFlashing) yield break; // prevents overlapping flashes
        isFlashing = true;

        // Instant full red flash
        damageFlashImage.color = new Color(1, 0, 0, 0.6f);

        // Fade out
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0.6f, 0f, elapsed / flashDuration);
            damageFlashImage.color = new Color(1, 0, 0, alpha);
            yield return null;
        }

        damageFlashImage.color = new Color(1, 0, 0, 0);
        isFlashing = false;
    }

    void Die()
    {
        Debug.Log("Player died!");

        // 🟥 SHOW TERMINATION (GAME OVER UI)
        if (terminationUI != null)
        {
            terminationUI.ShowGameOver();
        }

        // Freeze player movement (optional)
        Time.timeScale = 0f;
    }
}
