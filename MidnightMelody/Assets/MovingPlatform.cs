using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 moveOffset = new Vector3(0, 0, 5f); // jarak gerak maju mundur
    public float speed = 2f;                           // kecepatan gerak
    public bool isLooping = true;                      // agar terus bolak-balik

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingToTarget = true;

    private void Start()
    {
        startPos = transform.position;
        targetPos = startPos + moveOffset;
    }

    private void Update()
    {
        if (isLooping)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position,
                movingToTarget ? targetPos : startPos, step);

            // ubah arah kalau sudah sampai
            if (Vector3.Distance(transform.position, movingToTarget ? targetPos : startPos) < 0.05f)
            {
                movingToTarget = !movingToTarget;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Jika hero naik ke atas platform
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Jika hero turun
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
