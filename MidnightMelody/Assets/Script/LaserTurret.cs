using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    [Header("Laser Settings")]
    public float laserDistance = 50f;          
    public float damage = 25f;                 
    public float damageCooldown = 1f;         

    [Header("References")]
    public LineRenderer lineRenderer;          
    public LayerMask hitMask;                  

    private float lastDamageTime;              

    void Update()
    {
        FireLaser();
    }

    void FireLaser()
    {
        if (lineRenderer == null) return;

        lineRenderer.SetPosition(0, transform.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, laserDistance, hitMask))
        {
            lineRenderer.SetPosition(1, hit.point);

            if (hit.collider.CompareTag("Player"))
            {

                Debug.Log("Player terkena laser!");

                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {

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
