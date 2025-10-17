using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth;

    [Header("References")]
    public HealthBar healthBar;      // Drag dari inspector
    private Animator animator;       // referensi animator hero
    private Sc_hero heroScript;      // referensi script hero

    private bool isDead = false;
    private float damageAnimTime = 0.3f; // durasi animasi damage

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);

        animator = GetComponent<Animator>();
        heroScript = GetComponent<Sc_hero>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        Debug.Log($"❤️ Health player sekarang: {currentHealth}");

        // 🔹 Trigger animasi damage
        if (animator != null)
            animator.SetTrigger("isDamage");

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
        else
        {
            StartCoroutine(ResetDamageState());
        }
    }

    private IEnumerator ResetDamageState()
    {
        yield return new WaitForSeconds(damageAnimTime);
        if (animator != null)
            animator.ResetTrigger("isDamage");
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("💀 Player mati!");

        // 🔹 Hentikan pergerakan player
        if (heroScript != null)
            heroScript.enabled = false;

        // 🔹 Matikan input atau kontrol lain jika ada
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = Vector3.zero;

        // 🔹 Trigger animasi kematian
        if (animator != null)
            animator.SetBool("isDead", true);

        // 🔹 Pindah ke GameOver setelah delay
        StartCoroutine(GoToGameOver());
    }

    private IEnumerator GoToGameOver()
    {
        yield return new WaitForSeconds(2.5f); // waktu animasi mati
        SceneManager.LoadScene("GameOver");
    }
}
