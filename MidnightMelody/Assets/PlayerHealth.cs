using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public HealthBar healthBar;  // Drag UI health bar di sini

    [Header("Respawn Settings")]
    public Transform spawnPoint; // Drag SpawnPoint ke sini di Inspector

    private GameOverManager gameOverManager;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        gameOverManager = FindObjectOfType<GameOverManager>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player mati!");
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }
    }

    public void Respawn()
    {
        // Pulihkan nyawa
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);

        // Pindahkan player ke titik spawn
        transform.position = spawnPoint.position;

        // Aktifkan kembali game
        Time.timeScale = 1f;
    }
}
