using System.Collections;
using UnityEngine;

public class LaserGerak : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float laserDistance = 30f;
    [SerializeField] private LayerMask hitMask;

    private RaycastHit hit;

    void Start()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        // Laser langsung aktif selamanya
        lineRenderer.enabled = true;
    }

    void Update()
    {
        // Pastikan lineRenderer aktif
        if (!lineRenderer.enabled) return;

        // Titik awal laser
        lineRenderer.SetPosition(0, transform.position);

        // Tembakkan ray ke depan
        if (Physics.Raycast(transform.position, transform.forward, out hit, laserDistance, hitMask))
        {
            // Titik akhir laser = titik tabrakan
            lineRenderer.SetPosition(1, hit.point);

            // Jika kena hero (tag Player)
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Hero terkena laser!");
                // Misal: aktifkan damage script
                // hit.collider.GetComponent<HeroHealth>()?.TakeDamage(10);
            }
        }
        else
        {
            // Jika tidak mengenai apapun, pancarkan ke depan sejauh laserDistance
            lineRenderer.SetPosition(1, transform.position + transform.forward * laserDistance);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * laserDistance);
    }
}
