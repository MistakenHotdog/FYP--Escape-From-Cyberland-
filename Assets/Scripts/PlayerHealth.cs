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
    private bool isDead = false;

    [Header("Audio")]
    public AudioSource hitSoundSource;
    public AudioClip hitSound;

    [Header("UI Reference")]
    public TerminationUI terminationUI;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (hitSoundSource != null && hitSound != null)
        {
            hitSoundSource.PlayOneShot(hitSound);
        }

        if (damageFlashImage != null)
            StartCoroutine(DamageFlash());

        if (currentHealth < 0.5f)
        {
            currentHealth = 0f;
            if (healthBar != null)
                healthBar.value = 0f;
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        if (isFlashing) yield break;
        isFlashing = true;

        damageFlashImage.color = new Color(1, 0, 0, 0.6f);

        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0.6f, 0f, elapsed / flashDuration);
            damageFlashImage.color = new Color(1, 0, 0, alpha);
            yield return null;
        }

        damageFlashImage.color = new Color(1, 0, 0, 0);
        isFlashing = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();

        if (damageFlashImage != null)
            damageFlashImage.color = new Color(1, 0, 0, 0);

        AlarmSystem alarm = FindObjectOfType<AlarmSystem>();
        if (alarm != null) alarm.StopAlarm();

        if (terminationUI != null)
        {
            terminationUI.ShowGameOver();
        }

        Time.timeScale = 0f;
        enabled = false;
    }
}
