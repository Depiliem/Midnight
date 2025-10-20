using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth;

    [Header("References")]
    public HealthBar healthBar; 
    private Animator animator; 
    private Sc_hero heroScript;    

    private bool isDead = false;
    private float damageAnimTime = 0.3f;

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

        Debug.Log($" Health player sekarang: {currentHealth}");
        
        if (currentHealth <= 0 && !isDead)
        {
            Die(); // Panggil fungsi Die() yang baru
        }
        else
        {
            // Hanya mainkan animasi damage jika belum mati
            if (animator != null)
                animator.SetTrigger("isDamage");
                
            StartCoroutine(ResetDamageState());
        }
    }

    private IEnumerator ResetDamageState()
    {
        yield return new WaitForSeconds(damageAnimTime);
        if (animator != null)
            animator.ResetTrigger("isDamage");
    }

    // --- FUNGSI DIE() YANG DIPERBARUI ---
    private void Die()
    {
        isDead = true;
        Debug.Log("💀 Player mati! Memuat scene GameOver...");

        // --- DIHAPUS ---
        // Kita tidak perlu menghentikan player atau memainkan animasi
        // karena scene akan langsung berganti.
        // if (heroScript != null)
        //     heroScript.enabled = false;
        // var rb = GetComponent<Rigidbody>();
        // if (rb != null)
        //     rb.velocity = Vector3.zero;
        // if (animator != null)
        //     animator.SetBool("isDead", true);
        // StartCoroutine(GoToGameOver());
        // ---------------

        // --- BARU ---
        // Aktifkan kursor agar bisa dipakai di menu GameOver
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Langsung muat scene GameOver
        SceneManager.LoadScene("GameOver");
    }

    // --- DIHAPUS ---
    // Coroutine GoToGameOver() tidak diperlukan lagi
    // private IEnumerator GoToGameOver()
    // {
    //     ...
    // }
}