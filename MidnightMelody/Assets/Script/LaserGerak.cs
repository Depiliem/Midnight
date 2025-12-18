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
        lineRenderer.enabled = true;
    }

    void Update()
    {
        
        
        if (!lineRenderer.enabled) return;
        lineRenderer.SetPosition(0, transform.position);

        if (Physics.Raycast(transform.position, transform.forward, out hit, laserDistance, hitMask))
        {
            lineRenderer.SetPosition(1, hit.point);

            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Hero terkena laser!");
            
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
