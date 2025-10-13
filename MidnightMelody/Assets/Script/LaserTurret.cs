using System.Collections;
using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float laserDistance = 30f;
    [SerializeField] private float onTime = 8f;   // nyala 8 detik
    [SerializeField] private float offTime = 3f;  // mati 3 detik
    [SerializeField] private LayerMask hitMask;

    private bool laserActive = false;
    private RaycastHit hit;

    void Start()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.enabled = false;
        StartCoroutine(LaserCycle());
    }

    IEnumerator LaserCycle()
    {
        while (true)
        {
            // nyalakan laser
            laserActive = true;
            lineRenderer.enabled = true;
            yield return new WaitForSeconds(onTime);

            // matikan laser
            laserActive = false;
            lineRenderer.enabled = false;
            yield return new WaitForSeconds(offTime);
        }
    }

    void Update()
    {
        if (!laserActive) return;

        lineRenderer.SetPosition(0, transform.position);

        // tembak ray ke depan
        if (Physics.Raycast(transform.position, transform.forward, out hit, laserDistance, hitMask))
        {
            lineRenderer.SetPosition(1, hit.point);

            // jika kena hero
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Hero terkena laser!");
                // contoh: tambahkan efek damage
                // hit.collider.GetComponent<HeroHealth>()?.TakeDamage(10);
            }
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward * laserDistance);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * laserDistance);
    }
}
