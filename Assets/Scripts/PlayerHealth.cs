using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider healthBar;
    public float maxHealth = 100f;
    private float currentHealth;

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

        if (currentHealth <= 0)
        {
            Die();
        }
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
