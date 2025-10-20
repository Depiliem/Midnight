using UnityEngine;
using UnityEngine.SceneManagement; // kalau mau reload scene

public class VoidZone : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;  // drag PlayerHealth dari hero
    public Transform respawnPoint;     // opsional — kalau mau respawn
    public bool respawnInsteadOfDeath = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.name == "hero")
        {
            Debug.Log("☠️ Player jatuh ke void!");

            if (respawnInsteadOfDeath && respawnPoint != null)
            {
                // Kalau kamu mau respawn aja
                other.transform.position = respawnPoint.position;
                if (playerHealth != null)
                    playerHealth.TakeDamage(25); // optional damage saat jatuh
            }
            else
            {
                // Kalau mau langsung mati
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(playerHealth.currentHealth);
                }
                else
                {
                    // fallback: reload scene
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }
    }
}
