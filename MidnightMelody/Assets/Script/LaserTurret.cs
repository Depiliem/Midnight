using System.Collections;
using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private LineRenderer lineRenderer;   // Komponen LineRenderer untuk laser
    [SerializeField] private float laserDistance = 30f;   // Jarak maksimum laser
    [SerializeField] private float damageAmount = 25f;    // Damage per hit
    [SerializeField] private float damageCooldown = 1f;   // Waktu jeda antar damage
    [SerializeField] private LayerMask hitMask;           // Layer yang bisa terkena laser

    private bool canDamage = true;
    private RaycastHit hit;

    void Start()
    {
        // Pastikan LineRenderer ada
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        // Aktifkan laser dari awal
        lineRenderer.enabled = true;
    }

    void Update()
    {
        // Posisi awal laser = posisi turret
        lineRenderer.SetPosition(0, transform.position);

        // Tembakkan ray lurus ke depan
        if (Physics.Raycast(transform.position, transform.forward, out hit, laserDistance, hitMask))
        {
            // Akhir laser = titik benturan
            lineRenderer.SetPosition(1, hit.point);

            // Cek apakah yang kena adalah Player
            if (hit.collider.CompareTag("Player") && canDamage)
            {
                PlayerHealth player = hit.collider.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.TakeDamage((int)damageAmount);
                    StartCoroutine(DamageCooldown());
                }
            }
        }
        else
        {
            // Kalau tidak kena apa pun, laser tetap lurus ke depan
            lineRenderer.SetPosition(1, transform.position + transform.forward * laserDistance);
        }
    }

    IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, transform.forward * laserDistance);
    }
}
