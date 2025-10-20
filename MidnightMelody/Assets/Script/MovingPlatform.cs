using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 moveOffset = new Vector3(0, 0, 5f); // jarak gerak maju mundur
    public float speed = 2f;                           // kecepatan gerak
    public bool isLooping = true;                       // agar terus bolak-balik

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingToTarget = true;
    
    // --- PERBAIKAN: Tambahkan Rigidbody ---
    private Rigidbody rb; 

    private void Start()
    {
        // --- PERBAIKAN: Dapatkan komponen Rigidbody ---
        rb = GetComponent<Rigidbody>();
        // Pastikan Rigidbody di-set ke Is Kinematic di Inspector!
        
        startPos = transform.position; // (rb.position juga bisa)
        targetPos = startPos + moveOffset;
    }

    // --- PERBAIKAN: Pindahkan semua logika ke FixedUpdate() ---
    private void FixedUpdate()
    {
        if (isLooping)
        {
            // --- PERBAIKAN: Gunakan Time.fixedDeltaTime ---
            float step = speed * Time.fixedDeltaTime; 
            
            // Tentukan tujuan saat ini
            Vector3 currentTarget = movingToTarget ? targetPos : startPos;

            // --- PERBAIKAN: Gunakan rb.position dan rb.MovePosition() ---
            Vector3 newPos = Vector3.MoveTowards(rb.position, currentTarget, step);
            rb.MovePosition(newPos);

            // ubah arah kalau sudah sampai
            // (Gunakan rb.position untuk mengecek jarak)
            if (Vector3.Distance(rb.position, currentTarget) < 0.05f)
            {
                movingToTarget = !movingToTarget;
            }
        }
    }

    // Fungsi OnCollisionEnter dan OnCollisionExit sudah benar.
    // 'SetParent' akan bekerja jauh lebih stabil sekarang
    // karena kedua objek (player & platform) bergerak di FixedUpdate.
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