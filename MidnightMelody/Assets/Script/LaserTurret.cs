using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    [Header("Laser Settings")]
    public float laserDistance = 50f;          // Jarak maksimum laser
    public float damage = 25f;                 // Damage per tembakan
    public float damageCooldown = 1f;          // Waktu antar damage

    [Header("References")]
    public LineRenderer lineRenderer;          // Komponen LineRenderer untuk laser
    public LayerMask hitMask;                  // Layer untuk mendeteksi target (pastikan Player termasuk di sini)

    private float lastDamageTime;              // Waktu terakhir damage diberikan

    void Update()
    {
        FireLaser();
    }

    void FireLaser()
    {
        if (lineRenderer == null) return;

        // Titik awal laser
        lineRenderer.SetPosition(0, transform.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, laserDistance, hitMask))
        {
            // Titik akhir laser (kena sesuatu)
            lineRenderer.SetPosition(1, hit.point);

            // Jika kena player
            if (hit.collider.CompareTag("Player"))
            {
                // Debug log untuk uji
                Debug.Log("Player terkena laser!");

                // Ambil script PlayerHealth
                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    // Pastikan cooldown damage
                    if (Time.time - lastDamageTime > damageCooldown)
                    {
                        playerHealth.TakeDamage(damage);
                        lastDamageTime = Time.time;
                    }
                }
            }
        }
        else
        {
            // Kalau gak kena apa-apa, buat laser panjang maksimum
            lineRenderer.SetPosition(1, transform.position + transform.forward * laserDistance);
        }
    }

    // Debug garis laser di editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, transform.forward * laserDistance);
    }
}
