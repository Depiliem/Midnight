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
            Die(); 
        }
        else
        {
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


    private void Die()
    {
        isDead = true;
        Debug.Log("💀 Player mati! Memuat scene GameOver...");

        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("GameOver");
    }

    
}